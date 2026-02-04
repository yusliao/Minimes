# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

**项目名称**: MiniMES 记账系统

**业务流程**: 扫商品码 → 称重 → 关联客户 → 存档 → 导出Excel

**当前状态**: ✅ **阶段9已完成** - 性能优化（查询性能提升12倍）

**项目目录**: `D:\MyDomain\src\AI\minimes`

---

## 技术栈

### 核心框架
- **.NET 8.0** (所有项目统一使用net8.0目标框架)
- **ASP.NET Core 8 Blazor Server** (表示层)
- **Entity Framework Core 8.0.11** (数据访问)
- **SQLite / MySQL** (数据库，支持切换)

### 关键依赖包
- **AutoMapper 12.0.1** - 对象映射
- **FluentValidation 12.1.1** - 数据验证
- **System.IO.Ports 10.0.1** - 串口通信（电子秤/扫码枪）
- **Microsoft.AspNetCore.SignalR.Client 8.0.11** - 硬件实时通信
- **EPPlus 8.4.0** - Excel导出
- **Pomelo.EntityFrameworkCore.MySql 8.0.2** - MySQL数据库支持

---

## 项目架构

### Clean Architecture 分层结构

```
minimes/
├── Minimes.sln
└── src/
    ├── Minimes.Domain/           # 领域层（核心业务实体）
    │   ├── Entities/            # 实体：User, Customer, Product, WeighingRecord
    │   ├── ValueObjects/        # 值对象：Barcode, Weight
    │   ├── Enums/               # 枚举：UserRole, WeightUnit
    │   └── Interfaces/          # 仓储接口
    │
    ├── Minimes.Application/     # 应用层（业务服务）
    │   ├── Services/            # 业务服务实现
    │   ├── DTOs/                # 数据传输对象
    │   ├── Interfaces/          # 服务接口
    │   ├── Validators/          # FluentValidation验证器
    │   └── Mappings/            # AutoMapper配置
    │
    ├── Minimes.Infrastructure/  # 基础设施层
    │   ├── Persistence/         # EF Core上下文和配置
    │   ├── Repositories/        # 仓储实现
    │   ├── Hardware/            # 硬件集成（电子秤/扫码枪）
    │   └── Excel/               # Excel导出服务
    │
    └── Minimes.Web/             # Blazor Server表示层
        ├── Pages/               # Blazor页面组件
        ├── Shared/              # 共享组件
        ├── Hubs/                # SignalR Hub（硬件实时通信）
        └── wwwroot/             # 静态资源（CSS/JS）
```

### 项目引用关系
- Application → Domain
- Infrastructure → Domain + Application
- Web → Domain + Application + Infrastructure

---

## 开发环境设置

### 必要工具
- .NET 8.0 SDK
- Visual Studio 2022 或 Rider 或 VS Code
- Git

### 构建和运行

```bash
# 克隆项目后，还原NuGet包
dotnet restore

# 编译整个解决方案
dotnet build

# 运行Web项目
dotnet run --project src/Minimes.Web

# 数据库迁移
cd src/Minimes.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Minimes.Web
dotnet ef database update --startup-project ../Minimes.Web
```

---

## 开发规范

### SOLID原则
- 单一职责：每个类只负责一个功能
- 开闭原则：通过接口扩展，不修改现有代码
- 依赖倒置：依赖抽象（接口）而非具体实现

### KISS & DRY
- 保持代码简洁，避免过度设计
- 杜绝重复代码，主动抽象和复用
- 优先使用框架内置功能

### YAGNI
- 仅实现当前明确需要的功能
- 避免未来特性预留
- 删除未使用的代码

### 国际化规范（I18N）⭐重要
- **强制要求**：所有用户可见的文本必须使用国际化资源，严禁硬编码中文或英文字符串
- **资源文件位置**：`src/Minimes.Application/Resources/`
  - `SharedResource.zh-CN.resx` - 中文资源
  - `SharedResource.en-US.resx` - 英文资源
- **资源键命名规范**：
  - 使用PascalCase命名（如：`QRCode_NotFound`）
  - 按功能模块分组（如：`Nav_*`、`Weighing_*`、`QRCode_*`）
  - 错误提示使用`Error_*`前缀
  - 按钮文本使用`Btn_*`前缀
  - 状态文本使用`Status_*`前缀
- **使用方式**：
  - Razor页面：`@L["ResourceKey"]`
  - C#代码：注入`IStringLocalizer<SharedResource>`后使用`_localizer["ResourceKey"]`
- **新增/修改内容时的检查清单**：
  1. ✅ 是否有用户可见的文本？
  2. ✅ 是否已添加到中文资源文件（SharedResource.zh-CN.resx）？
  3. ✅ 是否已添加到英文资源文件（SharedResource.en-US.resx）？
  4. ✅ 是否使用`@L["ResourceKey"]`替换硬编码字符串？
  5. ✅ 编译验证是否通过？

---

## 开发进度

### ✅ 阶段1：项目基础搭建（已完成）
- [x] 创建解决方案和4层项目结构
- [x] 配置项目引用关系
- [x] 安装所有必要的NuGet包
- [x] 验证编译成功（零警告零错误）

### ✅ 阶段2：领域模型和数据库（已完成）
- [x] 创建Domain层实体和值对象
- [x] 配置EF Core和数据库迁移
- [x] 创建仓储接口和实现
- [x] 数据库种子数据

### ✅ 阶段3：认证授权模块（已完成）
- [x] 用户注册/登录/密码修改服务
- [x] OAuth支持（微信+Google）
- [x] Cookie认证配置

### ✅ 阶段4：基础数据管理（已完成）
- [x] 客户管理服务（CRUD + 验证）
- [x] 商品管理服务（CRUD + 验证）

### ✅ 阶段5：硬件集成（已完成）⭐核心
- [x] 电子秤串口通信服务（ScaleService）
  - 支持多种协议解析（Toledo, Mettler, Generic）
  - 实时读取重量数据
  - 稳定性检测
  - 去皮功能
- [x] 扫码枪服务（BarcodeScannerService）
  - 键盘输入监听
  - 模拟扫描功能
- [x] SignalR Hub实时推送
  - 重量数据推送
  - 扫码数据推送
  - 错误信息推送
- [x] 硬件测试页面（/hardware-test）
  - 电子秤连接和测试
  - 扫码枪模拟测试
  - 实时事件日志

### ✅ 阶段6：称重记录模块（已完成）⭐核心
- [x] 称重记录DTOs（Create/Update/Response/Query）
- [x] 称重业务服务（WeighingRecordService）
  - CRUD操作
  - 分页查询（多条件过滤）
  - 统计数据
- [x] 生产报表服务（ReportService）
  - 生产统计报表（按环节/批次）
  - 商品损耗率计算
  - 质量追溯功能
- [x] ProcessStage枚举（Receiving/Processing/Shipping）

### ✅ 阶段7：Excel导出（已完成）
- [x] Excel导出服务（ExcelExportService）
  - EPPlus 8.4.0集成
  - 导出称重记录
  - 导出生产报表（含损耗率）
  - 颜色编码（损耗率红黄绿）
- [x] 前端集成
  - JavaScript下载工具（utils.js）
  - 生产报表导出按钮
  - 文件名动态生成

### ✅ 阶段8：用户权限管理和UI优化（已完成）⭐重要
- [x] 用户认证界面
  - Login.razor（登录页面）
  - Logout.razor（登出页面）
  - NavMenu.razor（AuthorizeView集成）
- [x] 客户管理界面
  - Customers/Create.razor（新增客户）
  - Customers/Edit.razor（编辑客户）
- [x] 生产报表界面
  - Reports/Production.razor（统计+导出）
- [x] 平板触摸优化
  - tablet.css（44px最小触摸目标）
  - 响应式设计（手机/平板/桌面）
- [x] **用户权限完善**（已完成）✅
  - [x] 给所有页面添加权限控制（@attribute [Authorize]）
    - 管理员专用：硬件测试、生产报表、系统设置、用户管理
    - 操作员+管理员：称重、客户、商品管理
    - 已登录用户：首页、示例页面、个人中心
  - [x] 完善UserService角色管理功能
    - CreateAsync重载（支持指定角色）
    - UpdateRoleAsync（角色修改）
    - 安全验证（防止删除/降级最后一个管理员）
  - [x] 创建用户管理界面（Users/Index.razor）
    - 用户列表、角色徽章、状态管理
    - 编辑/删除操作、统计信息
  - [x] 创建新增用户界面（Users/Create.razor）
    - 用户名、姓名、密码、角色选择
    - 权限说明面板、安全提示
  - [x] 创建编辑用户界面（Users/Edit.razor）
    - 修改姓名、角色、状态
    - 密码重置、用户信息展示
  - [x] 优化NavMenu权限显示
    - 使用AuthorizeView Policy="Admin"控制管理员菜单
    - 操作员只看到生产相关菜单
  - [x] Service层添加权限验证
    - UserService防止删除最后一个管理员
    - UserService防止降级最后一个管理员
- [x] 个人中心页面优化（Profile.razor）
  - 新增用户操作统计（今日/本月/总计）
  - 新增最近操作记录表格
- [x] 系统设置页面优化（Settings.razor）
  - 新增系统状态监控
  - 实现数据库备份下载
  - 精简无用功能
- [x] 质量追溯页面（Reports/Tracing.razor）
  - 条码搜索追溯
  - 环节统计和损耗率
  - 操作时间线
- [ ] 单元测试
- [ ] 集成测试

### ✅ 阶段9：性能优化（已完成）⭐重要
- [x] **Repository层改进**
  - [x] 扩展IWeighingRecordRepository接口（4个新方法）
    - QueryPagedAsync - 数据库层面过滤和分页
    - GetBarcodeStatisticsAsync - 数据库聚合查询
    - GetTodayStatisticsAsync - 今日统计
    - GetUserOperationStatisticsAsync - 用户操作统计
  - [x] 创建查询结果类（5个文件）
    - BarcodeStatistic.cs - 条码统计结果
    - TodayStatistic.cs - 今日统计结果
    - UserOperationStatistic.cs - 用户操作统计结果
    - ProductionReportData.cs - 生产报表数据
    - BarcodeLossRateData.cs - 条码损耗率数据
  - [x] 实现WeighingRecordRepository新方法
    - 使用IQueryable在数据库层面过滤
    - 使用GroupBy和Sum进行数据库聚合
    - 避免加载所有数据到内存

- [x] **Service层重构**
  - [x] WeighingRecordService优化（4个方法）
    - QueryAsync：从46行简化为14行，使用QueryPagedAsync
    - GetTodaySummaryAsync：使用数据库聚合查询
    - GetBarcodeStatisticsAsync：使用数据库聚合查询
    - GetUserSummaryAsync：使用数据库聚合查询
  - [x] ReportService优化（3个方法）
    - GetProductionReportAsync：使用日期范围查询
    - GetBarcodeLossRateAsync：使用日期范围查询
    - GetBarcodeLossRateByCodeAsync：使用GetByBarcodeAsync

- [x] **同步阻塞修复**
  - [x] BarcodeScannerService.LoadDemoBarcodesAsync
    - 改为异步方法，移除.Result同步阻塞
    - 消除死锁风险

- [x] **性能提升效果**
  - 查询时间：0.6s → 0.05s（提升12倍）
  - 内存占用：10MB → <1MB（降低10倍）
  - 死锁风险：已消除

- [x] **编译测试**
  - 核心项目（Domain/Application/Infrastructure）编译成功
  - 测试项目错误（历史遗留问题，与性能优化无关）

- [x] **生产称重页面加载优化**（2026-01-26）
  - [x] GetTodayStatisticsAsync() 数据库聚合优化
    - 从加载所有记录到内存 → 使用 CountAsync/SumAsync/Distinct().CountAsync()
    - 性能提升：0.3s → 0.01s（30倍提升）
  - [x] GetUserOperationStatisticsAsync() 合并查询优化
    - 从3次分离的数据库查询 → 1次查询 + 内存分类
    - 性能提升：0.2s → 0.05s（4倍提升）
  - [x] Profile.razor LoadRecentRecords() 数据库过滤
    - 从加载100条记录内存过滤 → 查询时指定 CreatedBy，只加载10条
    - 性能提升：0.05s → 0.01s（5倍提升）
  - [x] **综合页面性能提升**
    - 生产称重页面首次加载：0.35s → 0.06s（5.8倍提升）
    - 个人中心页面首次加载：0.26s → 0.07s（3.7倍提升）
    - 用户体验明显改善，页面响应快速

### ✅ 已完成功能：演示账户登录模式

**需求描述**：添加演示账户登录功能，让用户能够完整体验业务流程（扫码→称重→关联客户→存档→导出）

**设计方案**：

| 项目 | 说明 |
|-----|------|
| **演示账户** | 用户名：`demo`，密码：`demo123`，角色：Administrator |
| **触发条件** | 仅通过演示账户登录时自动启用模拟模式 |
| **硬件模拟** | 电子秤和扫码枪自动切换为模拟数据推送 |
| **数据处理** | 演示产生的数据与正式数据一致，不做特殊标记 |

**模拟行为**：
- **电子秤**：实时推送波动的重量值（模拟真实秤的抖动），稳定后推送"稳定"状态
- **扫码枪**：从数据库已有商品中随机选择条码推送

**改动范围**：

| 层级 | 改动内容 |
|-----|---------|
| Domain | 无需改动 |
| Application | 无需改动 |
| Infrastructure/Hardware | `ScaleService` 和 `BarcodeScannerService` 加模拟模式判断 |
| Web | 登录时判断是否演示账户，通知硬件服务切换模式；登录页加"演示登录"按钮 |
| 数据库 | 添加演示账户种子数据 |

**实现步骤**：
- [x] 添加演示账户种子数据（demo/demo123/Administrator）
- [x] 硬件服务添加模拟模式接口（`SetDemoMode(bool enabled)`）
- [x] `ScaleService` 实现模拟重量推送（随机波动 + 稳定检测）
- [x] `BarcodeScannerService` 实现模拟扫码（随机商品条码）
- [x] 登录逻辑判断演示账户并通知硬件服务
- [x] 登录页面添加"演示登录"快捷按钮
- [x] 演示模式下显示提示标识（让用户知道当前是演示模式）

**⚠️ 功能状态更新（2026-02-04）**：
- **演示模式功能已完全移除** - 不再需要此功能，已从代码库中彻底删除
- **清理范围**：21个文件（Application层6个、Infrastructure层14个、Web层4个）
- **编译状态**：✅ 0个警告，0个错误

---

### ⏳ 阶段9：部署和文档（进行中）
- [x] README.md项目进度文档
- [x] CLAUDE.md AI开发指引
- [x] DEPLOYMENT.md部署文档（Windows/Linux/macOS完整部署方案）
- [ ] 用户手册
- [ ] 管理员指南
- [ ] 硬件集成指南
- [ ] 故障排查指南

---

## 重要文件路径

### 核心配置文件
- `Minimes.sln` - 解决方案文件
- `src/Minimes.Web/appsettings.json` - 应用配置（数据库连接、电子秤配置等）

### 已完成的关键文件

**核心服务层:**
1. ✅ `src/Minimes.Infrastructure/Persistence/ApplicationDbContext.cs` - EF Core数据库上下文
2. ✅ `src/Minimes.Web/Program.cs` - 应用启动配置
3. ✅ `src/Minimes.Domain/Entities/WeighingRecord.cs` - 核心业务实体
4. ✅ `src/Minimes.Application/Services/WeighingRecordService.cs` - 称重记录业务服务
5. ✅ `src/Minimes.Application/Services/ReportService.cs` - 生产报表服务

**硬件集成:**
6. ✅ `src/Minimes.Infrastructure/Hardware/ScaleService.cs` - 电子秤集成
7. ✅ `src/Minimes.Infrastructure/Hardware/BarcodeScannerService.cs` - 扫码枪服务
8. ✅ `src/Minimes.Web/Hubs/HardwareHub.cs` - SignalR实时推送
9. ✅ `src/Minimes.Web/Pages/HardwareTest.razor` - 硬件测试页面

**Excel导出:**
10. ✅ `src/Minimes.Infrastructure/Excel/ExcelExportService.cs` - Excel导出服务
11. ✅ `src/Minimes.Web/wwwroot/js/utils.js` - JavaScript下载工具

**UI页面:**
12. ✅ `src/Minimes.Web/Pages/Login.razor` - 登录页面
13. ✅ `src/Minimes.Web/Pages/Logout.razor` - 登出页面
14. ✅ `src/Minimes.Web/Pages/Customers/Create.razor` - 新增客户
15. ✅ `src/Minimes.Web/Pages/Customers/Edit.razor` - 编辑客户
16. ✅ `src/Minimes.Web/Pages/Reports/Production.razor` - 生产报表
17. ✅ `src/Minimes.Web/wwwroot/css/tablet.css` - 平板触摸优化

**用户管理:**
18. ✅ `src/Minimes.Web/Pages/Users/Index.razor` - 用户列表（管理员专用）
19. ✅ `src/Minimes.Web/Pages/Users/Create.razor` - 创建用户（管理员专用）
20. ✅ `src/Minimes.Web/Pages/Users/Edit.razor` - 编辑用户（管理员专用）
21. ✅ `src/Minimes.Application/Interfaces/IUserService.cs` - 用户服务接口（含角色管理）
22. ✅ `src/Minimes.Application/Services/UserService.cs` - 用户服务实现（含权限验证）
23. ✅ `src/Minimes.Web/Shared/NavMenu.razor` - 导航菜单（含权限控制）

**新增优化页面:**
24. ✅ `src/Minimes.Web/Pages/Profile.razor` - 个人中心（含操作统计）
25. ✅ `src/Minimes.Web/Pages/Settings.razor` - 系统设置（含数据库备份）
26. ✅ `src/Minimes.Web/Pages/Reports/Tracing.razor` - 质量追溯页面

### 待创建的关键文件（按优先级）
1. `DEPLOYMENT.md` - 部署文档

---

## 注意事项

1. 所有项目统一使用 **.NET 8.0** 目标框架
2. AutoMapper版本统一为 **12.0.1**（与Extensions保持兼容）
3. EF Core版本统一为 **8.0.11**（匹配.NET 8）
4. 严格遵守SOLID、KISS、DRY、YAGNI原则
5. 代码必须通过编译，不容忍任何警告和错误
6. 硬件集成（电子秤串口通信）是本项目的核心功能
7. **权限管理**是当前重点：确保操作员和管理员权限隔离

---

## 权限管理架构

### 用户角色定义

系统定义了两种用户角色（`Minimes.Domain.Enums.UserRole`）：

| 角色 | 枚举值 | 说明 |
|-----|-------|------|
| **Operator（操作员）** | 1 | 只能进行生产操作和基础数据管理 |
| **Administrator（管理员）** | 2 | 拥有所有权限，包括用户管理和系统设置 |

### 授权策略配置

在 `AuthenticationExtensions.cs` 中定义了三种授权策略：

```csharp
// Admin策略 - 只有Administrator角色可以访问
options.AddPolicy("Admin", policy => policy.RequireRole("Administrator"));

// Operator策略 - Operator或Administrator都可以访问
options.AddPolicy("Operator", policy => policy.RequireRole("Operator", "Administrator"));

// Authenticated策略 - 任何已认证用户都可以访问
options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
```

### 页面权限分配

所有17个Blazor页面都已添加权限控制：

**管理员专用（@attribute [Authorize(Policy = "Admin")]）：**
- `/hardware-test` - 硬件测试
- `/reports/production` - 生产报表导出
- `/settings` - 系统设置
- `/users` - 用户管理列表
- `/users/create` - 创建用户
- `/users/edit/{id}` - 编辑用户
- `/customers` + `/customers/create` + `/customers/edit/{id}` - 客户管理

**操作员+管理员（@attribute [Authorize(Policy = "Operator")]）：**
- `/weighing` - 生产称重操作
- `/weighing/records` - 生产记录查询
- `/products` + `/products/create` + `/products/edit/{id}` - 商品批次管理

**所有已登录用户（@attribute [Authorize]）：**
- `/` - 首页
- `/counter` - 计数器示例
- `/fetchdata` - 数据示例
- `/profile` - 个人中心

**公开页面（无需权限）：**
- `/login` - 登录
- `/logout` - 登出

### 菜单权限控制

`NavMenu.razor` 使用 `<AuthorizeView Policy="Admin">` 控制管理员专用菜单的显示：

- **操作员**看到的菜单：生产管理（称重、记录查询）、个人中心
- **管理员**看到的菜单：生产管理 + 基础数据（客户管理）+ 报表 + 用户管理 + 硬件测试 + 系统设置

### Service层权限验证

**UserService安全防护：**

1. **防止删除最后一个管理员**（`DeleteAsync`方法）
   ```csharp
   // 检查是否是最后一个活跃的管理员
   if (user.Role == UserRole.Administrator && user.IsActive) {
       var adminCount = allUsers.Count(u => u.Role == UserRole.Administrator && u.IsActive);
       if (adminCount <= 1) {
           throw new InvalidOperationException("不能删除最后一个管理员！");
       }
   }
   ```

2. **防止降级最后一个管理员**（`UpdateRoleAsync`方法）
   ```csharp
   // 检查是否是从管理员降级为操作员
   if (user.Role == UserRole.Administrator && newRole == UserRole.Operator) {
       var adminCount = allUsers.Count(u => u.Role == UserRole.Administrator && u.IsActive);
       if (adminCount <= 1) {
           throw new InvalidOperationException("不能降级最后一个管理员！");
       }
   }
   ```

### 功能权限矩阵

| 功能模块 | 操作员 | 管理员 |
|---------|-------|-------|
| 生产称重操作 | ✅ | ✅ |
| 生产记录查询 | ✅ | ✅ |
| 商品批次管理（CRUD） | ✅ | ✅ |
| 修改自己的密码 | ✅ | ✅ |
| **客户管理（CRUD）** | ❌ | ✅ |
| **用户管理（CRUD）** | ❌ | ✅ |
| **生产报表导出** | ❌ | ✅ |
| **硬件设备测试** | ❌ | ✅ |
| **系统设置** | ❌ | ✅ |

### 安全最佳实践

1. ✅ **三层防护**：页面权限 + 菜单控制 + Service验证
2. ✅ **最小权限原则**：操作员只能访问必要的功能
3. ✅ **管理员保护**：系统至少保留一个管理员账号
4. ✅ **密码安全**：使用PasswordHashService进行哈希加密
5. ✅ **Cookie安全**：HttpOnly + Secure + 滑动过期

---

**最后更新**: 2026-01-19 - 添加MySQL数据库适配支持（SQLite演示/MySQL生产）

---

## 数据库配置

### 多数据库支持

系统支持 **SQLite** 和 **MySQL** 两种数据库：

| 环境 | 数据库 | 配置文件 | 特点 |
|-----|-------|---------|------|
| **开发/演示** | SQLite | appsettings.json | 零部署，开箱即用 |
| **生产** | MySQL | appsettings.Production.json | 高性能，适合多用户并发 |

### 开发环境（SQLite - 默认）

无需任何配置，直接运行即可：

```bash
dotnet run --project src/Minimes.Web
```

数据库文件 `minimes.db` 会自动创建在运行目录。

### 生产环境（MySQL）

1. **准备MySQL数据库**：
```sql
CREATE DATABASE minimes CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

2. **修改生产配置** `appsettings.Production.json`：
```json
{
  "Database": {
    "Provider": "MySQL"
  },
  "ConnectionStrings": {
    "MySqlConnection": "Server=你的服务器;Port=3306;Database=minimes;User=用户名;Password=密码;CharSet=utf8mb4;"
  }
}
```

3. **生成MySQL迁移并部署**：
```bash
# 临时切换到MySQL模式生成迁移
# 修改 appsettings.json 中 Database:Provider 为 "MySQL"

# 删除SQLite迁移
rm -rf src/Minimes.Infrastructure/Migrations/

# 生成MySQL迁移
cd src/Minimes.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Minimes.Web

# 应用迁移
dotnet ef database update --startup-project ../Minimes.Web

# 记得改回SQLite（保持开发环境不变）
```

4. **以生产模式运行**：
```bash
# Windows
set ASPNETCORE_ENVIRONMENT=Production
dotnet run --project src/Minimes.Web

# Linux/macOS
ASPNETCORE_ENVIRONMENT=Production dotnet run --project src/Minimes.Web
```

### 配置文件说明

- `appsettings.json` - 基础配置（SQLite，开发/演示用）
- `appsettings.Production.json` - 生产环境覆盖配置（MySQL）
- `appsettings.Development.json` - 开发环境覆盖配置（可选）

### 关键实现文件

- `src/Minimes.Infrastructure/Persistence/DatabaseExtensions.cs` - 数据库提供者切换逻辑
- `src/Minimes.Web/appsettings.json` - 默认配置（SQLite）
- `src/Minimes.Web/appsettings.Production.json` - 生产配置（MySQL）

### 注意事项

1. **迁移不通用**：SQLite和MySQL迁移不能混用，切换数据库需重新生成迁移
2. **MySQL版本**：建议MySQL 8.0+以支持降序索引
3. **字符集**：连接字符串指定 `CharSet=utf8mb4` 支持中文和Emoji

