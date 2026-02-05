# Employee模块功能测试指南

## 测试环境准备

### 1. 启动应用
```bash
cd D:\MyDomain\src\AI\minimes
dotnet run --project src/Minimes.Web
```

访问：http://localhost:5001

### 2. 登录系统
- 使用管理员账号登录（只有管理员能访问员工管理）
- 默认管理员账号：admin / admin123

---

## 测试场景1：Employee基础管理（CRUD）

### 1.1 创建Employee

**操作步骤：**
1. 访问 `/employees` 页面
2. 点击"新增员工"按钮
3. 填写以下信息：
   - 员工编码：E001
   - 员工姓名：张三
   - 联系人：张三
   - 联系电话：13800138001
   - 地址：北京市朝阳区
   - 状态：激活
4. 点击"保存"按钮

**预期结果：**
- ✅ 保存成功，显示成功提示
- ✅ 跳转回员工列表页
- ✅ 列表中显示新创建的员工
- ✅ "关联二维码"列显示 0

**数据库验证：**
```sql
SELECT * FROM employees WHERE Code = 'E001';
```

### 1.2 创建更多Employee（为后续测试准备）

重复上述步骤，创建以下员工：
- E002 / 李四 / 13800138002
- E003 / 王五 / 13800138003

**预期结果：**
- ✅ employees表中有3条激活的员工记录

---

## 测试场景2：MeatType联动生成QRCode ⭐核心功能

### 2.1 创建新的MeatType

**前置条件：**
- employees表中有3个激活的员工（E001, E002, E003）

**操作步骤：**
1. 访问 `/meattypes` 页面
2. 点击"新增肉类"按钮
3. 填写以下信息：
   - 肉类代码：BEEF
   - 肉类名称：牛肉
   - 状态：激活
4. 点击"保存"按钮

**预期结果：**
- ✅ MeatType创建成功
- ✅ 自动生成3个QRCode：
  - BEEF-E001
  - BEEF-E002
  - BEEF-E003
- ✅ 所有QRCode的BatchNumber相同，格式：`BATCH-BEEF-20260205123456`
- ✅ 所有QRCode的IsActive = true
- ✅ 所有QRCode的EmployeeCode字段正确填充（E001, E002, E003）

**数据库验证：**
```sql
-- 查看自动生成的QRCode
SELECT Id, Code, MeatTypeId, EmployeeCode, BatchNumber, IsActive
FROM qrcodes
WHERE Code LIKE 'BEEF-%'
ORDER BY Code;

-- 预期结果：
-- BEEF-E001 | MeatTypeId=7 | EmployeeCode=E001 | BatchNumber=BATCH-BEEF-xxx | IsActive=1
-- BEEF-E002 | MeatTypeId=7 | EmployeeCode=E002 | BatchNumber=BATCH-BEEF-xxx | IsActive=1
-- BEEF-E003 | MeatTypeId=7 | EmployeeCode=E003 | BatchNumber=BATCH-BEEF-xxx | IsActive=1
```

### 2.2 验证批次号一致性

**验证点：**
- ✅ 同一次创建MeatType生成的所有QRCode，BatchNumber完全相同
- ✅ 批次号格式：`BATCH-{MeatType.Code}-{yyyyMMddHHmmss}`

---

## 测试场景3：Employee停用级联QRCode ⭐核心功能

### 3.1 停用Employee

**前置条件：**
- Employee E001存在且激活
- E001关联的QRCode（BEEF-E001）存在且激活

**操作步骤：**
1. 访问 `/employees` 页面
2. 找到员工E001（张三）
3. 点击"编辑"按钮
4. 将"状态"改为"停用"
5. 点击"保存"按钮

**预期结果：**
- ✅ Employee E001的IsActive = false
- ✅ 所有EmployeeCode=E001的QRCode自动停用（IsActive = false）
- ✅ 其他Employee的QRCode不受影响（E002, E003的QRCode仍然激活）

**数据库验证：**
```sql
-- 验证E001的QRCode被停用
SELECT Code, EmployeeCode, IsActive
FROM qrcodes
WHERE EmployeeCode = 'E001';
-- 预期：IsActive = 0

-- 验证其他Employee的QRCode不受影响
SELECT Code, EmployeeCode, IsActive
FROM qrcodes
WHERE EmployeeCode IN ('E002', 'E003');
-- 预期：IsActive = 1
```

### 3.2 删除Employee

**操作步骤：**
1. 访问 `/employees` 页面
2. 找到员工E002（李四）
3. 点击"删除"按钮
4. 确认删除

**预期结果：**
- ✅ Employee E002被删除（或标记为删除）
- ✅ 所有EmployeeCode=E002的QRCode自动停用（IsActive = false）
- ✅ 其他Employee的QRCode不受影响

**数据库验证：**
```sql
-- 验证E002的QRCode被停用
SELECT Code, EmployeeCode, IsActive
FROM qrcodes
WHERE EmployeeCode = 'E002';
-- 预期：IsActive = 0
```

---

## 测试场景4：MeatType删除级联QRCode ⭐核心功能

### 4.1 删除MeatType

**前置条件：**
- MeatType BEEF存在
- BEEF关联的QRCode（BEEF-E001, BEEF-E002, BEEF-E003）存在

**操作步骤：**
1. 访问 `/meattypes` 页面
2. 找到肉类BEEF（牛肉）
3. 点击"删除"按钮
4. 确认删除

**预期结果：**
- ✅ MeatType BEEF被删除
- ✅ 所有MeatTypeId=BEEF的QRCode自动停用（IsActive = false）
- ✅ 其他MeatType的QRCode不受影响

**数据库验证：**
```sql
-- 查找BEEF的MeatTypeId
SELECT Id FROM meattypes WHERE Code = 'BEEF';
-- 假设Id=7

-- 验证BEEF的所有QRCode被停用
SELECT Code, MeatTypeId, IsActive
FROM qrcodes
WHERE MeatTypeId = 7;
-- 预期：所有记录的IsActive = 0
```

---

## 测试场景5：UI显示QRCode数量

### 5.1 员工列表页显示

**操作步骤：**
1. 访问 `/employees` 页面
2. 观察"关联二维码"列

**预期结果：**
- ✅ 每个员工显示关联的QRCode数量（蓝色徽章）
- ✅ E001：显示关联的QRCode数量（包括已停用的）
- ✅ E003：显示关联的QRCode数量（只统计激活的）
- ✅ 数量实时计算，准确无误

### 5.2 员工编辑页显示

**操作步骤：**
1. 访问 `/employees` 页面
2. 点击任意员工的"编辑"按钮
3. 观察页面上的QRCode数量显示

**预期结果：**
- ✅ 页面上显示"关联二维码：X个"
- ✅ 数量与列表页一致

---

## 测试场景6：国际化验证

### 6.1 中文界面

**操作步骤：**
1. 确保浏览器语言设置为中文
2. 访问 `/employees` 页面

**预期结果：**
- ✅ 所有文本显示为中文
- ✅ 菜单项："员工管理"
- ✅ 按钮："新增员工"、"编辑"、"删除"
- ✅ 表头："员工编码"、"员工姓名"、"关联二维码"等

### 6.2 英文界面

**操作步骤：**
1. 修改浏览器语言设置为英文
2. 刷新页面

**预期结果：**
- ✅ 所有文本显示为英文
- ✅ 菜单项："Employee Management"
- ✅ 按钮："Create Employee"、"Edit"、"Delete"
- ✅ 表头："Employee Code"、"Employee Name"、"QR Codes"等

---

## 测试场景7：边界情况测试

### 7.1 创建MeatType时没有激活的Employee

**操作步骤：**
1. 停用所有Employee
2. 创建新的MeatType（例如：LAMB）

**预期结果：**
- ✅ MeatType创建成功
- ✅ 不生成任何QRCode（因为没有激活的Employee）
- ✅ 不报错

### 7.2 重复的Employee编码

**操作步骤：**
1. 尝试创建编码为E001的员工（已存在）

**预期结果：**
- ✅ 验证失败，显示错误提示："员工编码已存在"
- ✅ 不允许保存

### 7.3 停用已停用的Employee

**操作步骤：**
1. 停用一个已经停用的Employee

**预期结果：**
- ✅ 操作成功
- ✅ 不会重复停用QRCode
- ✅ 不报错

---

## 数据库完整性验证

### 验证脚本

```sql
-- 1. 验证employees表结构
DESCRIBE employees;

-- 2. 验证qrcodes表有EmployeeCode字段
DESCRIBE qrcodes;

-- 3. 验证数据一致性
SELECT
    e.Code AS EmployeeCode,
    e.Name AS EmployeeName,
    e.IsActive AS EmployeeActive,
    COUNT(q.Id) AS QRCodeCount,
    SUM(CASE WHEN q.IsActive = 1 THEN 1 ELSE 0 END) AS ActiveQRCodeCount
FROM employees e
LEFT JOIN qrcodes q ON q.EmployeeCode = e.Code
GROUP BY e.Id, e.Code, e.Name, e.IsActive;

-- 4. 验证级联停用逻辑
-- 停用的Employee应该没有激活的QRCode
SELECT
    e.Code AS EmployeeCode,
    e.IsActive AS EmployeeActive,
    q.Code AS QRCodeCode,
    q.IsActive AS QRCodeActive
FROM employees e
INNER JOIN qrcodes q ON q.EmployeeCode = e.Code
WHERE e.IsActive = 0 AND q.IsActive = 1;
-- 预期：0条记录（停用的Employee不应该有激活的QRCode）
```

---

## 性能测试（可选）

### 大批量Employee测试

**操作步骤：**
1. 创建100个激活的Employee（E001-E100）
2. 创建新的MeatType

**预期结果：**
- ✅ 自动生成100个QRCode
- ✅ 生成时间 < 5秒
- ✅ 所有QRCode的BatchNumber相同
- ✅ 不报错，不超时

---

## 测试完成检查清单

- [ ] 测试1：Employee基础管理（CRUD）
- [ ] 测试2：MeatType联动生成QRCode
- [ ] 测试3：Employee停用级联QRCode
- [ ] 测试4：MeatType删除级联QRCode
- [ ] 测试5：UI显示QRCode数量
- [ ] 测试6：国际化验证
- [ ] 测试7：边界情况测试
- [ ] 数据库完整性验证
- [ ] 性能测试（可选）

---

## 常见问题排查

### 问题1：QRCode没有自动生成

**可能原因：**
- MeatTypeService的GenerateQRCodesForAllEmployeesAsync方法未调用
- 没有激活的Employee

**排查步骤：**
1. 检查日志：`src/Minimes.Web/logs/`
2. 验证Employee是否激活：`SELECT * FROM employees WHERE IsActive = 1;`
3. 检查MeatTypeService.CreateAsync方法是否调用了GenerateQRCodesForAllEmployeesAsync

### 问题2：级联停用不生效

**可能原因：**
- QRCodeRepository的级联方法未正确实现
- EmployeeService的UpdateAsync/DeleteAsync未调用级联方法

**排查步骤：**
1. 检查日志
2. 手动执行SQL验证：
   ```sql
   UPDATE qrcodes SET IsActive = 0 WHERE EmployeeCode = 'E001';
   ```
3. 检查EmployeeService代码

### 问题3：QRCodeCount显示不正确

**可能原因：**
- GetCountByEmployeeCodeAsync方法实现错误
- MapToResponseAsync未调用GetCountByEmployeeCodeAsync

**排查步骤：**
1. 手动执行SQL验证：
   ```sql
   SELECT COUNT(*) FROM qrcodes WHERE EmployeeCode = 'E001';
   ```
2. 检查EmployeeService.MapToResponseAsync方法

---

**测试完成后，请将测试结果反馈给开发团队！**
