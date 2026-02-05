# 设备框架功能测试指南

**文档版本**: 1.0
**最后更新**: 2026-02-05
**测试范围**: 设备管理Web层SignalR实时推送功能
**项目目录**: `D:\MyDomain\src\AI\minimes`

---

## 📋 测试概述

本文档指导如何手动测试通用设备数据采集框架的Web层SignalR实时推送功能，确保设备状态变化能够实时推送到前端页面。

### 测试目标

验证以下三个核心功能：
1. **设备状态更新推送** - 设备状态变化时实时更新前端显示
2. **设备错误推送** - 设备发生错误时实时显示错误信息
3. **设备列表更新推送** - 设备注册/注销时自动刷新设备列表

### 测试环境要求

- ✅ .NET 8.0 SDK
- ✅ 项目编译成功（零警告零错误）
- ✅ 数据库已初始化（SQLite或MySQL）
- ✅ 浏览器支持WebSocket（Chrome/Edge/Firefox推荐）
- ✅ 至少一个测试用户账号（Operator或Administrator角色）

---

## 🚀 测试准备

### 1. 启动应用程序

```bash
# 进入项目目录
cd D:\MyDomain\src\AI\minimes

# 启动Web应用
dotnet run --project src/Minimes.Web
```

**预期输出**：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 2. 登录系统

1. 打开浏览器访问 `http://localhost:5000`
2. 使用测试账号登录（需要Operator或Administrator角色）
3. 导航到 **设备监控** 页面（`/devices`）

### 3. 打开浏览器开发者工具

按 `F12` 打开开发者工具，切换到 **Console** 标签，用于查看SignalR连接状态和事件日志。

**预期日志**：
```
SignalR连接成功
WebSocket连接已建立
```

---

## 🧪 测试场景

### 场景1：设备状态更新推送

**测试目的**：验证设备状态变化时，前端页面能够实时更新设备状态显示。

#### 测试步骤

1. **打开设备监控页面** (`/devices`)
   - 确认页面显示当前所有已注册的设备
   - 记录某个设备的当前状态（如：Disconnected）

2. **触发设备状态变化**（需要后端代码配合）
   - 方式A：通过设备详情页面点击"连接"按钮
   - 方式B：通过API调用 `POST /api/devices/{deviceId}/connect`
   - 方式C：设备驱动自动连接（如演示模式）

3. **观察前端变化**
   - 设备卡片的状态徽章应该实时更新（如：Disconnected → Connected）
   - 设备卡片的边框颜色应该变化（如：灰色 → 蓝色）
   - 连接时间应该显示当前时间

#### 预期结果

✅ **成功标准**：
- 设备状态徽章颜色变化（Disconnected灰色 → Connected蓝色 → Running绿色）
- 设备卡片左边框颜色变化（Running显示绿色边框）
- 连接时间自动更新
- 页面无需手动刷新

❌ **失败表现**：
- 状态不更新，需要手动刷新页面
- 控制台显示SignalR连接错误
- 状态更新延迟超过1秒

#### 故障排查

如果测试失败，检查以下内容：

1. **SignalR连接状态**
   ```javascript
   // 在浏览器Console中执行
   console.log(hubConnection.state);
   // 应该显示 "Connected"
   ```

2. **后端日志**
   ```bash
   # 查看应用日志，确认DeviceManager是否触发了状态变化事件
   grep "设备状态变化" logs/app.log
   ```

3. **网络请求**
   - 打开开发者工具 → Network → WS（WebSocket）
   - 确认WebSocket连接存在且状态为"101 Switching Protocols"

---

### 场景2：设备错误推送

**测试目的**：验证设备发生错误时，前端页面能够实时显示错误信息。

#### 测试步骤

1. **打开设备监控页面** (`/devices`)
   - 确认页面顶部没有错误提示

2. **触发设备错误**（需要后端代码配合）
   - 方式A：断开设备连接（如拔掉串口线）
   - 方式B：通过API调用触发错误 `POST /api/devices/{deviceId}/simulate-error`
   - 方式C：设备驱动内部错误（如通信超时）

3. **观察前端变化**
   - 页面顶部应该显示红色错误提示框
   - 错误信息格式：`[设备ID] 错误描述`
   - 设备卡片状态徽章变为红色（Error）

#### 预期结果

✅ **成功标准**：
- 页面顶部显示红色Alert框
- 错误信息包含设备ID和错误描述
- 设备状态徽章变为红色"Error"
- 错误计数器（ErrorCount）增加

❌ **失败表现**：
- 错误信息不显示
- 错误信息格式错误
- 设备状态未变为Error

---

### 场景3：设备列表更新推送

**测试目的**：验证设备注册/注销时，前端页面能够自动刷新设备列表。

#### 测试步骤

1. **打开设备监控页面** (`/devices`)
   - 记录当前设备数量（如：3个设备）

2. **注册新设备**（需要后端代码配合）
   - 方式A：通过API调用 `POST /api/devices/register`
   - 方式B：启动新的设备驱动实例
   - 方式C：通过管理界面添加设备

3. **观察前端变化**
   - 设备列表应该自动刷新
   - 新设备卡片应该出现在列表中
   - 设备数量增加

4. **注销设备**（需要后端代码配合）
   - 方式A：通过API调用 `DELETE /api/devices/{deviceId}`
   - 方式B：停止设备驱动实例
   - 方式C：通过管理界面删除设备

5. **观察前端变化**
   - 设备列表应该自动刷新
   - 被删除的设备卡片应该消失
   - 设备数量减少

#### 预期结果

✅ **成功标准**：
- 设备注册后，列表自动刷新显示新设备
- 设备注销后，列表自动刷新移除设备
- 页面无需手动刷新
- 刷新过程流畅无闪烁

❌ **失败表现**：
- 列表不自动刷新
- 需要手动刷新页面才能看到变化
- 刷新过程中页面闪烁或卡顿

---

## 🔍 高级测试场景

### 场景4：多浏览器窗口同步测试

**测试目的**：验证SignalR推送能够同步到所有连接的客户端。

#### 测试步骤

1. **打开两个浏览器窗口**
   - 窗口A：设备监控页面 (`/devices`)
   - 窗口B：设备详情页面 (`/devices/{deviceId}`)

2. **在窗口A触发设备状态变化**
   - 点击某个设备的"连接"按钮

3. **观察两个窗口的变化**
   - 窗口A：设备列表中的状态应该更新
   - 窗口B：设备详情页面的状态应该同步更新

#### 预期结果

✅ **成功标准**：
- 两个窗口的设备状态同步更新
- 更新延迟小于500ms
- 无需手动刷新

---

### 场景5：SignalR自动重连测试

**测试目的**：验证SignalR连接断开后能够自动重连。

#### 测试步骤

1. **打开设备监控页面** (`/devices`)
   - 确认SignalR连接正常

2. **模拟网络中断**
   - 方式A：在开发者工具中切换到Offline模式
   - 方式B：重启Web应用（Ctrl+C后重新启动）

3. **恢复网络连接**
   - 方式A：在开发者工具中切换回Online模式
   - 方式B：等待Web应用重启完成

4. **观察SignalR重连**
   - 查看浏览器Console日志
   - 应该显示"SignalR正在重连..."和"SignalR重连成功"

#### 预期结果

✅ **成功标准**：
- SignalR自动重连成功
- 重连后设备状态推送恢复正常
- 用户无需手动刷新页面

---

## 📊 测试结果记录

### 测试报告模板

```markdown
## 功能测试报告

**测试日期**: 2026-02-05
**测试人员**: [姓名]
**测试环境**: Windows 11 / Chrome 120

### 测试结果汇总

| 测试场景 | 测试结果 | 备注 |
|---------|---------|------|
| 场景1：设备状态更新推送 | ✅ 通过 | 状态更新实时，无延迟 |
| 场景2：设备错误推送 | ✅ 通过 | 错误信息显示正确 |
| 场景3：设备列表更新推送 | ✅ 通过 | 列表自动刷新 |
| 场景4：多浏览器窗口同步 | ✅ 通过 | 多窗口同步正常 |
| 场景5：SignalR自动重连 | ✅ 通过 | 重连机制工作正常 |

### 发现的问题

1. [问题描述]
   - **严重程度**: 高/中/低
   - **复现步骤**: [详细步骤]
   - **预期结果**: [应该发生什么]
   - **实际结果**: [实际发生了什么]

### 测试结论

- [ ] 所有测试通过，功能正常
- [ ] 部分测试失败，需要修复
- [ ] 测试无法进行，环境问题
```

---

## 🛠️ 故障排查指南

### 问题1：SignalR连接失败

**症状**：
- 浏览器Console显示"SignalR连接失败"
- 设备状态不更新

**排查步骤**：

1. **检查Web应用是否正常运行**
   ```bash
   curl http://localhost:5000/hardwareHub
   # 应该返回404（Hub不支持GET请求，但说明服务正常）
   ```

2. **检查SignalR Hub注册**
   ```csharp
   // 在Program.cs中确认以下代码存在
   app.MapHub<HardwareHub>("/hardwareHub");
   ```

3. **检查浏览器WebSocket支持**
   ```javascript
   // 在浏览器Console中执行
   console.log('WebSocket' in window);
   // 应该显示 true
   ```

---

### 问题2：设备状态更新不实时

**症状**：
- 设备状态变化后，前端显示延迟或不更新
- 需要手动刷新页面才能看到变化

**排查步骤**：

1. **检查DeviceManager是否注入了IDeviceNotificationService**
   ```csharp
   // 在DeviceManager构造函数中确认
   public DeviceManager(
       ILogger<DeviceManager> logger,
       DeviceLogManager logManager,
       IDeviceNotificationService? notificationService) // 确认这个参数存在
   ```

2. **检查事件处理是否调用了推送方法**
   ```csharp
   // 在OnDeviceStatusChanged中确认
   _ = _notificationService?.NotifyDeviceStatusUpdateAsync(...);
   ```

3. **检查前端事件监听器是否正确注册**
   ```csharp
   // 在Index.razor中确认
   hubConnection.On<DeviceStatusUpdateEvent>("ReceiveDeviceStatusUpdate", evt => { ... });
   ```

---

### 问题3：设备错误不显示

**症状**：
- 设备发生错误，但前端没有显示错误信息

**排查步骤**：

1. **检查DeviceManager错误事件处理**
   ```csharp
   // 在OnDeviceErrorOccurred中确认
   _ = _notificationService?.NotifyDeviceErrorAsync(...);
   ```

2. **检查前端错误监听器**
   ```csharp
   // 在Index.razor中确认
   hubConnection.On<DeviceErrorEvent>("ReceiveDeviceError", evt => { ... });
   ```

3. **检查错误信息是否正确设置**
   ```csharp
   // 在Index.razor中确认
   errorMessage = $"[{evt.deviceId}] {evt.errorMessage}";
   ```

---

## 📝 测试注意事项

1. **测试环境隔离**
   - 使用独立的测试数据库，避免影响生产数据
   - 使用测试账号，不要使用生产账号

2. **测试数据准备**
   - 确保至少有2-3个测试设备
   - 确保设备处于不同状态（Connected、Disconnected、Running、Error）

3. **浏览器兼容性**
   - 推荐使用Chrome或Edge浏览器
   - 避免使用IE浏览器（不支持WebSocket）

4. **网络环境**
   - 确保本地网络稳定
   - 避免使用VPN或代理（可能影响WebSocket连接）

5. **日志记录**
   - 测试过程中保留浏览器Console日志
   - 保留后端应用日志
   - 截图记录关键测试步骤

---

## 🎯 测试完成标准

功能测试通过的标准：

- ✅ 所有5个测试场景全部通过
- ✅ SignalR连接稳定，无频繁断线
- ✅ 设备状态更新实时，延迟小于500ms
- ✅ 错误信息显示准确，格式正确
- ✅ 设备列表自动刷新，无需手动操作
- ✅ 多浏览器窗口同步正常
- ✅ SignalR自动重连机制工作正常

---

**文档结束**
