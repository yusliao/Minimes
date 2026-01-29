# Blazor + ASP.NET Core 技术讲义

> **基于 MiniMES 项目的实战教程**
> 作者：老王（技术暴躁流）
> 最后更新：2026-01-29

---

## 📚 文档说明

艹！这份讲义是老王我专门为新人准备的，以 **MiniMES 记账系统** 为实战案例，深入浅出地讲解 **Blazor** 和 **ASP.NET Core** 技术。

**适合人群：**
- 有基本C#语法基础的开发者
- 想学习现代Web开发的.NET开发者
- 对Blazor技术感兴趣的前端/后端开发者
- 需要快速上手企业级项目的新人

**学习目标：**
- ✅ 理解Blazor Server的工作原理和核心概念
- ✅ 掌握ASP.NET Core的依赖注入、中间件、配置系统
- ✅ 学会使用SignalR实现实时通信
- ✅ 掌握Clean Architecture分层架构设计
- ✅ 能够独立开发企业级Blazor应用

---

## 📖 目录结构

### 第一章：技术概述
- [1.1 什么是Blazor？](#11-什么是blazor)
- [1.2 Blazor Server vs Blazor WebAssembly](#12-blazor-server-vs-blazor-webassembly)
- [1.3 什么是ASP.NET Core？](#13-什么是aspnet-core)
- [1.4 为什么选择Blazor + ASP.NET Core？](#14-为什么选择blazor--aspnet-core)

### 第二章：Blazor核心概念
- [2.1 Blazor组件基础](#21-blazor组件基础)
- [2.2 组件生命周期](#22-组件生命周期)
- [2.3 数据绑定（Data Binding）](#23-数据绑定data-binding)
- [2.4 事件处理（Event Handling）](#24-事件处理event-handling)
- [2.5 依赖注入（Dependency Injection）](#25-依赖注入dependency-injection)
- [2.6 路由和导航](#26-路由和导航)
- [2.7 表单和验证](#27-表单和验证)
- [2.8 JavaScript互操作（JS Interop）](#28-javascript互操作js-interop)

### 第三章：ASP.NET Core核心概念
- [3.1 Program.cs - 应用启动配置](#31-programcs---应用启动配置)
- [3.2 依赖注入容器（DI Container）](#32-依赖注入容器di-container)
- [3.3 中间件管道（Middleware Pipeline）](#33-中间件管道middleware-pipeline)
- [3.4 配置系统（Configuration）](#34-配置系统configuration)
- [3.5 认证和授权（Authentication & Authorization）](#35-认证和授权authentication--authorization)
- [3.6 SignalR实时通信](#36-signalr实时通信)
- [3.7 后台服务（Hosted Services）](#37-后台服务hosted-services)

### 第四章：MiniMES项目架构分析
- [4.1 Clean Architecture分层设计](#41-clean-architecture分层设计)
- [4.2 Domain层 - 领域模型](#42-domain层---领域模型)
- [4.3 Application层 - 业务逻辑](#43-application层---业务逻辑)
- [4.4 Infrastructure层 - 基础设施](#44-infrastructure层---基础设施)
- [4.5 Web层 - 表示层](#45-web层---表示层)
- [4.6 项目引用关系](#46-项目引用关系)

### 第五章：核心技术实战
- [5.1 实战案例1：称重页面（WeighingPage.razor）](#51-实战案例1称重页面weighingpagerazor)
- [5.2 实战案例2：SignalR实时推送（HardwareHub）](#52-实战案例2signalr实时推送hardwarehub)
- [5.3 实战案例3：布局和导航（MainLayout.razor）](#53-实战案例3布局和导航mainlayoutrazor)
- [5.4 实战案例4：认证授权流程](#54-实战案例4认证授权流程)
- [5.5 实战案例5：国际化（i18n）实现](#55-实战案例5国际化i18n实现)

### 第六章：最佳实践和开发规范
- [6.1 SOLID原则在项目中的应用](#61-solid原则在项目中的应用)
- [6.2 KISS、DRY、YAGNI原则](#62-kissdryyagni原则)
- [6.3 性能优化技巧](#63-性能优化技巧)
- [6.4 常见问题和解决方案](#64-常见问题和解决方案)
- [6.5 调试技巧](#65-调试技巧)

### 附录
- [附录A：MiniMES项目文件结构](#附录aminimes项目文件结构)
- [附录B：常用NuGet包说明](#附录b常用nuget包说明)
- [附录C：参考资源](#附录c参考资源)

---

## 🎯 学习路径建议

**新手路径（0基础）：**
1. 先看第一章了解技术背景
2. 重点学习第二章Blazor核心概念
3. 跳过第三章，直接看第五章实战案例
4. 回头补第三章ASP.NET Core概念
5. 最后看第四章架构设计

**有经验开发者路径：**
1. 快速浏览第一章
2. 重点看第四章架构设计
3. 深入第五章实战案例
4. 查阅第二、三章作为参考手册

**架构师路径：**
1. 直接看第四章架构设计
2. 重点看第六章最佳实践
3. 第五章作为代码审查参考

---

## 💡 老王的学习建议

1. **边学边做**：别tm光看不练，看完一个概念立马写代码验证
2. **理解原理**：不要死记硬背，理解为什么这么设计
3. **多看源码**：MiniMES项目的代码都是精心设计的，多看几遍
4. **动手调试**：遇到不懂的地方，打断点调试，看看运行时发生了什么
5. **问题驱动**：带着问题学习，比如"为什么要用SignalR？"、"依赖注入有什么好处？"

---

## 🚀 开始学习

准备好了吗？让老王我带你进入Blazor和ASP.NET Core的世界！

**下一步：** [第一章：技术概述](#第一章技术概述)

---

# 第一章：技术概述

## 1.1 什么是Blazor？

**Blazor** 是微软推出的一个现代化Web UI框架，让你可以用 **C#** 而不是JavaScript来构建交互式Web应用。

### 核心特点

| 特点 | 说明 |
|-----|------|
| **C#全栈开发** | 前端和后端都用C#，不需要学JavaScript（当然JS互操作还是支持的） |
| **组件化开发** | 类似React/Vue的组件模型，代码复用性强 |
| **双向数据绑定** | 数据变化自动更新UI，UI变化自动更新数据 |
| **.NET生态** | 直接使用NuGet包，享受.NET强大的生态系统 |
| **类型安全** | C#的强类型系统，编译时就能发现错误 |

### 老王的大白话解释

艹！简单说就是：**以前写Web前端必须用JavaScript，现在可以用C#了！**

想象一下：
- 你是个C#后端开发，突然要写前端，JavaScript各种坑让你头疼
- 有了Blazor，你可以继续用熟悉的C#语法写前端
- 前后端共享代码、共享模型、共享验证逻辑，爽得不行！

---

## 1.2 Blazor Server vs Blazor WebAssembly

Blazor有两种运行模式，老王我给你详细对比一下：

### Blazor Server（MiniMES使用的模式）

**工作原理：**
```
浏览器 <--SignalR WebSocket--> ASP.NET Core服务器
       (UI事件)                  (执行C#代码)
       (UI更新)                  (计算结果)
```

**特点：**
- ✅ **首次加载快**：只下载很小的HTML+JS，不需要下载整个.NET运行时
- ✅ **服务器端执行**：C#代码在服务器运行，可以直接访问数据库
- ✅ **兼容性好**：不需要浏览器支持WebAssembly
- ✅ **调试方便**：直接在Visual Studio调试，和调试普通C#代码一样
- ❌ **需要持续连接**：依赖SignalR连接，断网就GG
- ❌ **服务器压力大**：每个用户都占用服务器资源

### Blazor WebAssembly

**工作原理：**
```
浏览器 (下载.NET运行时 + 应用DLL)
       ↓
       在浏览器中执行C#代码（通过WebAssembly）
       ↓
       通过HTTP API与服务器通信
```

**特点：**
- ✅ **离线工作**：下载后可以离线运行（PWA）
- ✅ **服务器压力小**：计算在客户端进行
- ❌ **首次加载慢**：需要下载.NET运行时（几MB）
- ❌ **性能较差**：WebAssembly性能不如原生代码
- ❌ **不能直接访问服务器资源**：必须通过API

### MiniMES为什么选择Blazor Server？

老王我在设计MiniMES时选择了Blazor Server，原因如下：

1. **实时性要求高**：电子秤数据需要实时推送，SignalR天然支持
2. **内网部署**：工厂内网环境，网络稳定，不担心断线
3. **硬件集成**：需要访问串口设备，必须在服务器端执行
4. **快速启动**：用户打开浏览器就能用，不需要等待大文件下载

---

## 1.3 什么是ASP.NET Core？

**ASP.NET Core** 是微软的跨平台Web框架，是ASP.NET的现代化重写版本。

### 核心特点

| 特点 | 说明 |
|-----|------|
| **跨平台** | Windows、Linux、macOS都能跑 |
| **高性能** | 性能吊打Node.js、Django等框架 |
| **模块化** | 按需引入中间件，不用的功能不加载 |
| **依赖注入** | 内置DI容器，代码解耦更优雅 |
| **开源** | 完全开源，社区活跃 |

### ASP.NET Core的核心组件

```
ASP.NET Core
├── Kestrel Web服务器（高性能HTTP服务器）
├── 中间件管道（Middleware Pipeline）
├── 依赖注入容器（DI Container）
├── 配置系统（Configuration）
├── 日志系统（Logging）
├── 认证授权（Authentication & Authorization）
└── SignalR（实时通信）
```

### 老王的大白话解释

ASP.NET Core就是个**超级强大的Web服务器框架**，它提供了：
- 处理HTTP请求的能力（Kestrel）
- 管理对象生命周期的能力（DI）
- 处理配置文件的能力（Configuration）
- 实时通信的能力（SignalR）
- 认证授权的能力（Authentication）

你只需要专注写业务逻辑，其他脏活累活框架都帮你干了！

---

## 1.4 为什么选择Blazor + ASP.NET Core？

### 传统Web开发的痛点

**前后端分离（React/Vue + .NET API）：**
- ❌ 前端JavaScript，后端C#，两套语法
- ❌ 前后端模型不一致，需要手动同步
- ❌ 前后端验证逻辑重复写两遍
- ❌ 调试麻烦，前后端分别调试

**传统ASP.NET MVC：**
- ❌ 页面刷新体验差
- ❌ 前端交互能力弱
- ❌ 还是要写JavaScript

### Blazor + ASP.NET Core的优势

| 优势 | 说明 |
|-----|------|
| **统一语言** | 前后端都用C#，学习成本低 |
| **代码共享** | 实体类、验证逻辑、工具类前后端共享 |
| **类型安全** | 编译时检查，减少运行时错误 |
| **强大生态** | NuGet包、Entity Framework、AutoMapper等直接用 |
| **实时通信** | SignalR内置支持，不需要额外配置 |
| **开发效率** | 一个人就能搞定全栈开发 |

### MiniMES项目的实际收益

在MiniMES项目中，Blazor + ASP.NET Core带来了以下好处：

1. **代码复用**：
   - `WeighingRecord`实体类在Domain层定义，前后端共享
   - FluentValidation验证规则在Application层定义，前后端共享
   - 枚举类型（`UserRole`、`WeightUnit`）前后端共享

2. **实时通信**：
   - 电子秤数据通过SignalR实时推送到前端
   - 扫码枪数据通过SignalR实时推送到前端
   - 不需要前端轮询，性能更好

3. **开发效率**：
   - 一个C#开发者就能完成整个项目
   - 不需要学习React/Vue/Angular
   - 调试方便，F5直接调试前后端

4. **类型安全**：
   - 前端调用后端服务，编译时就能发现错误
   - 重构时IDE自动提示，不会漏改

### 老王的总结

艹！选择Blazor + ASP.NET Core就是为了：
- **少学一门语言**（不用学JavaScript）
- **少写重复代码**（前后端共享）
- **少踩坑**（类型安全）
- **快速开发**（一个人搞定全栈）

对于企业内部系统、工具类应用、实时性要求高的应用，Blazor Server是个非常好的选择！

---

**下一章：** [第二章：Blazor核心概念](#第二章blazor核心概念)

---

# 第二章：Blazor核心概念

## 2.1 Blazor组件基础

### 什么是Blazor组件？

Blazor组件就是一个 `.razor` 文件，包含HTML标记和C#代码。组件是Blazor应用的基本构建块。

### 组件的基本结构

```razor
@page "/example"
@inject IExampleService ExampleService

<h3>示例组件</h3>

<p>当前计数：@currentCount</p>
<button @onclick="IncrementCount">点击+1</button>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

**结构说明：**
- `@page` - 路由指令，定义访问路径
- `@inject` - 依赖注入指令，注入服务
- HTML标记 - 组件的UI部分
- `@code` - C#代码块，组件的逻辑部分

### MiniMES实战案例：WeighingPage.razor

让我们看看MiniMES项目中的称重页面组件（简化版）：

```razor
@page "/weighing"
@attribute [Authorize(Policy = "Operator")]
@inject IWeighingRecordService WeighingRecordService
@inject NavigationManager Navigation

<h3>生产称重</h3>

<div class="card">
    <div class="card-body">
        <!-- 条码输入 -->
        <input type="text" @bind="currentBarcode" />

        <!-- 重量输入 -->
        <input type="number" @bind="manualWeightInput" />

        <!-- 保存按钮 -->
        <button @onclick="SaveRecord">保存</button>
    </div>
</div>

@code {
    private string currentBarcode = string.Empty;
    private decimal manualWeightInput = 0;

    private async Task SaveRecord()
    {
        var request = new CreateWeighingRecordRequest
        {
            Barcode = currentBarcode,
            Weight = manualWeightInput
        };

        await WeighingRecordService.CreateAsync(request, "admin");
        Navigation.NavigateTo("/weighing/records");
    }
}
```

**关键点：**
1. `@page "/weighing"` - 定义路由，访问 `/weighing` 就会显示这个组件
2. `@attribute [Authorize]` - 权限控制，只有登录用户才能访问
3. `@inject` - 注入服务，可以直接调用业务逻辑
4. `@bind` - 双向数据绑定，输入框的值自动同步到变量
5. `@onclick` - 事件处理，点击按钮触发方法

---

## 2.2 组件生命周期

### 生命周期方法

Blazor组件有以下生命周期方法（按执行顺序）：

| 方法 | 执行时机 | 用途 |
|-----|---------|------|
| `SetParametersAsync` | 参数设置时 | 接收父组件传递的参数 |
| `OnInitialized` / `OnInitializedAsync` | 组件初始化时（只执行一次） | 加载初始数据 |
| `OnParametersSet` / `OnParametersSetAsync` | 参数设置后 | 响应参数变化 |
| `OnAfterRender` / `OnAfterRenderAsync` | 组件渲染后 | JS互操作、DOM操作 |

### 生命周期流程图

```
组件创建
  ↓
SetParametersAsync
  ↓
OnInitialized / OnInitializedAsync  ← 【加载数据】
  ↓
OnParametersSet / OnParametersSetAsync
  ↓
渲染UI
  ↓
OnAfterRender / OnAfterRenderAsync  ← 【JS互操作】
  ↓
组件显示
  ↓
（参数变化时重复 OnParametersSet → 渲染 → OnAfterRender）
  ↓
组件销毁 → Dispose / DisposeAsync
```

### MiniMES实战案例：WeighingPage生命周期

```csharp
@code {
    private HubConnection? hubConnection;
    private TodaySummary? todaySummary;

    // 1. 组件初始化：加载数据
    protected override async Task OnInitializedAsync()
    {
        // 创建SignalR连接对象
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/hardwareHub"))
            .Build();

        // 订阅扫码事件
        hubConnection.On<object>("ReceiveBarcode", (data) =>
        {
            // 处理扫码数据
            currentBarcode = data.barcode;
            InvokeAsync(StateHasChanged);
        });

        // 加载今日统计数据
        await LoadTodaySummary();

        // 加载最近记录
        await LoadRecentRecords();
    }

    // 2. 组件渲染后：启动SignalR连接、聚焦输入框
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // 自动聚焦到条码输入框
            await JSRuntime.InvokeVoidAsync("eval",
                "document.querySelector('input')?.focus()");

            // 后台启动SignalR连接
            if (hubConnection != null)
            {
                await hubConnection.StartAsync();
            }
        }
    }

    // 3. 组件销毁：释放资源
    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
```

**关键点：**
1. `OnInitializedAsync` - 加载数据，创建SignalR连接对象
2. `OnAfterRenderAsync` - 首次渲染后聚焦输入框、启动SignalR连接
3. `DisposeAsync` - 组件销毁时释放SignalR连接

**老王的经验：**
- ✅ 数据加载放在 `OnInitializedAsync`
- ✅ JS互操作放在 `OnAfterRenderAsync`（此时DOM已渲染）
- ✅ 资源释放放在 `DisposeAsync`（避免内存泄漏）
- ❌ 不要在构造函数中做异步操作

---

## 2.3 数据绑定（Data Binding）

### 单向绑定

**语法：** `@变量名`

```razor
<p>当前计数：@currentCount</p>

@code {
    private int currentCount = 42;
}
```

### 双向绑定

**语法：** `@bind="变量名"`

```razor
<!-- 输入框双向绑定 -->
<input type="text" @bind="userName" />
<p>你输入的是：@userName</p>

@code {
    private string userName = string.Empty;
}
```

**工作原理：**
```
用户输入 → 触发onchange事件 → 更新变量 → 自动刷新UI
```

### 绑定事件控制

默认情况下，`@bind` 在 `onchange` 事件时更新（失去焦点时）。可以改为 `oninput`（实时更新）：

```razor
<!-- 实时更新（每次输入都触发） -->
<input type="text" @bind="currentBarcode" @bind:event="oninput" />

@code {
    private string currentBarcode = string.Empty;
}
```

### MiniMES实战案例：称重页面的数据绑定

```razor
<!-- 条码输入：实时绑定 -->
<input type="text"
       @bind="currentBarcode"
       @bind:event="oninput"
       @onkeydown="OnBarcodeKeyDown"
       placeholder="请扫描或输入条码" />

<!-- 重量输入：双向绑定 -->
<input type="number"
       @bind="manualWeightInput"
       @onkeydown="OnWeightKeyDown"
       step="0.001" min="0" />

<!-- 备注输入：双向绑定 -->
<textarea @bind="remarks" rows="2"></textarea>

<!-- 显示绑定的值 -->
@if (!string.IsNullOrWhiteSpace(currentBarcode))
{
    <small>已输入条码：@currentBarcode</small>
}

@code {
    private string currentBarcode = string.Empty;
    private decimal manualWeightInput = 0;
    private string? remarks;
}
```

**关键点：**
1. `@bind` - 双向绑定，输入框值自动同步到变量
2. `@bind:event="oninput"` - 改为实时更新（每次输入都触发）
3. `@变量名` - 单向绑定，显示变量的值

---

## 2.4 事件处理（Event Handling）

### 基本事件处理

**语法：** `@on{事件名}="方法名"`

```razor
<!-- 点击事件 -->
<button @onclick="HandleClick">点击我</button>

<!-- 鼠标悬停事件 -->
<div @onmouseover="HandleMouseOver">鼠标悬停</div>

<!-- 输入事件 -->
<input @oninput="HandleInput" />

@code {
    private void HandleClick()
    {
        Console.WriteLine("按钮被点击了！");
    }

    private void HandleMouseOver()
    {
        Console.WriteLine("鼠标悬停了！");
    }

    private void HandleInput(ChangeEventArgs e)
    {
        var value = e.Value?.ToString();
        Console.WriteLine($"输入的值：{value}");
    }
}
```

### 带参数的事件处理

```razor
<button @onclick="() => HandleClickWithParam(42)">点击我</button>
<button @onclick="@(() => HandleClickWithParam(100))">点击我</button>

@code {
    private void HandleClickWithParam(int value)
    {
        Console.WriteLine($"参数值：{value}");
    }
}
```

### 异步事件处理

```razor
<button @onclick="SaveDataAsync">保存数据</button>

@code {
    private async Task SaveDataAsync()
    {
        // 异步操作
        await Task.Delay(1000);
        Console.WriteLine("数据已保存！");
    }
}
```

### 键盘事件处理

```razor
<input @onkeydown="HandleKeyDown" />

@code {
    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            Console.WriteLine("按下了回车键！");
        }
    }
}
```

### MiniMES实战案例：称重页面的事件处理

```razor
<!-- 条码输入框：回车键解析条码并跳转到重量输入框 -->
<input type="text"
       @bind="currentBarcode"
       @bind:event="oninput"
       @onkeydown="OnBarcodeKeyDown" />

<!-- 重量输入框：回车键直接保存 -->
<input type="number"
       @bind="manualWeightInput"
       @onkeydown="OnWeightKeyDown" />

<!-- 保存按钮：点击保存 -->
<button @onclick="SaveRecord">保存</button>

<!-- 清空按钮：点击清空表单 -->
<button @onclick="ClearForm">重置</button>

@code {
    private string currentBarcode = string.Empty;
    private decimal manualWeightInput = 0;

    // 条码输入框回车：解析条码并跳转到重量输入框
    private async Task OnBarcodeKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(currentBarcode))
        {
            await ParseBarcode();
            await FocusWeightInput();
        }
    }

    // 重量输入框回车：直接保存
    private async Task OnWeightKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && manualWeightInput > 0)
        {
            await SaveRecord();
        }
    }

    // 保存记录
    private async Task SaveRecord()
    {
        var request = new CreateWeighingRecordRequest
        {
            Barcode = currentBarcode,
            Weight = manualWeightInput
        };

        await WeighingRecordService.CreateAsync(request, "admin");

        // 清空表单，准备下一次
        ClearForm();

        // 自动聚焦回条码输入框
        await FocusBarcodeInput();
    }

    // 清空表单
    private void ClearForm()
    {
        currentBarcode = string.Empty;
        manualWeightInput = 0;
        StateHasChanged();
    }
}
```

**关键点：**
1. `@onkeydown` - 键盘按下事件，用于回车键快捷操作
2. `@onclick` - 点击事件，用于按钮操作
3. `KeyboardEventArgs` - 键盘事件参数，可以获取按键信息
4. `async Task` - 异步事件处理，用于调用异步服务

**老王的经验：**
- ✅ 事件处理方法可以是同步或异步
- ✅ 使用Lambda表达式传递参数：`@onclick="() => Method(param)"`
- ✅ 键盘事件用于快捷操作，提升用户体验
- ❌ 不要在事件处理中做耗时操作（会阻塞UI）

---

**下一节：** [2.5 依赖注入（Dependency Injection）](#25-依赖注入dependency-injection)

---

## 2.5 依赖注入（Dependency Injection）

### 什么是依赖注入？

**依赖注入（DI）** 是一种设计模式，让对象不需要自己创建依赖，而是由外部容器注入。

**老王的大白话：**
- ❌ **不用DI**：我需要一把锤子，我自己去五金店买
- ✅ **用DI**：我需要一把锤子，容器自动给我一把

### 为什么要用依赖注入？

| 好处 | 说明 |
|-----|------|
| **解耦** | 组件不依赖具体实现，只依赖接口 |
| **可测试** | 可以注入Mock对象进行单元测试 |
| **可维护** | 修改实现不影响使用方 |
| **生命周期管理** | 容器自动管理对象的创建和销毁 |

### Blazor中的依赖注入

**注册服务（Program.cs）：**
```csharp
// 注册服务
builder.Services.AddScoped<IWeighingRecordService, WeighingRecordService>();
builder.Services.AddSingleton<IScaleService, ScaleService>();
builder.Services.AddTransient<IExcelExportService, ExcelExportService>();
```

**注入服务（.razor组件）：**
```razor
@inject IWeighingRecordService WeighingRecordService
@inject NavigationManager Navigation
@inject IJSRuntime JSRuntime

@code {
    // 直接使用注入的服务
    private async Task LoadData()
    {
        var data = await WeighingRecordService.GetAllAsync();
    }
}
```

### 服务生命周期

| 生命周期 | 说明 | 使用场景 |
|---------|------|---------|
| **Singleton** | 应用启动时创建，全局唯一 | 硬件服务、配置服务 |
| **Scoped** | 每个请求创建一次 | 数据库上下文、业务服务 |
| **Transient** | 每次注入都创建新实例 | 轻量级工具类 |

### MiniMES实战案例：依赖注入

**1. 注册服务（Program.cs）：**
```csharp
// 硬件服务（单例）
builder.Services.AddSingleton<IScaleService, ScaleService>();

// 业务服务（Scoped）
builder.Services.AddScoped<IWeighingRecordService, WeighingRecordService>();
builder.Services.AddScoped<IUserService, UserService>();

// 基础设施服务
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
```

**2. 注入服务（WeighingPage.razor）：**
```razor
@inject IWeighingRecordService WeighingRecordService
@inject IMeatTypeService MeatTypeService
@inject IQRCodeService QRCodeService
@inject NavigationManager Navigation
@inject AuthenticationStateProvider AuthenticationStateProvider
@inject IJSRuntime JSRuntime

@code {
    // 直接使用注入的服务
    private async Task SaveRecord()
    {
        // 获取当前登录用户
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var currentUser = authState.User.Identity?.Name ?? "Unknown";

        // 调用业务服务
        await WeighingRecordService.CreateAsync(request, currentUser);

        // 导航到其他页面
        Navigation.NavigateTo("/weighing/records");
    }
}
```

**老王的经验：**
- ✅ 硬件服务用 `Singleton`（全局唯一，避免重复初始化）
- ✅ 业务服务用 `Scoped`（每个请求独立，避免并发问题）
- ✅ 工具类用 `Transient`（轻量级，每次创建新实例）
- ❌ 不要在 `Singleton` 服务中注入 `Scoped` 服务（会报错）

---

## 2.6 路由和导航

### 路由定义

**语法：** `@page "路径"`

```razor
@page "/counter"
@page "/counter/{id:int}"

<h3>计数器</h3>

@code {
    [Parameter]
    public int Id { get; set; }
}
```

### 路由参数

**路径参数：**
```razor
@page "/users/{userId:int}"

@code {
    [Parameter]
    public int UserId { get; set; }
}
```

**查询参数：**
```razor
@page "/search"
@inject NavigationManager Navigation

@code {
    protected override void OnInitialized()
    {
        var uri = new Uri(Navigation.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var keyword = query["keyword"];
    }
}
```

### 导航方法

```csharp
@inject NavigationManager Navigation

@code {
    private void NavigateToPage()
    {
        // 导航到指定页面
        Navigation.NavigateTo("/weighing");

        // 导航并强制刷新
        Navigation.NavigateTo("/weighing", forceLoad: true);

        // 导航到外部URL
        Navigation.NavigateTo("https://www.example.com", forceLoad: true);
    }
}
```

### MiniMES实战案例：路由和导航

**1. 路由定义：**
```razor
<!-- 称重页面 -->
@page "/weighing"

<!-- 编辑用户页面（带参数） -->
@page "/users/edit/{id:int}"

@code {
    [Parameter]
    public int Id { get; set; }
}
```

**2. 导航菜单（NavMenu.razor）：**
```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="/weighing">
        <span class="oi oi-scale" aria-hidden="true"></span> 生产称重
    </NavLink>
</div>

<div class="nav-item px-3">
    <NavLink class="nav-link" href="/weighing/records">
        <span class="oi oi-list" aria-hidden="true"></span> 生产记录
    </NavLink>
</div>
```

**3. 编程式导航：**
```csharp
// 保存成功后导航到列表页
private async Task SaveRecord()
{
    await WeighingRecordService.CreateAsync(request, currentUser);
    Navigation.NavigateTo("/weighing/records");
}

// 取消编辑，返回上一页
private void Cancel()
{
    Navigation.NavigateTo("/users");
}
```

**老王的经验：**
- ✅ 使用 `NavLink` 组件（自动高亮当前页面）
- ✅ 路由参数用 `[Parameter]` 特性标记
- ✅ 保存成功后导航到列表页（用户体验好）
- ❌ 不要在 `OnInitialized` 中导航（会导致死循环）

---

## 2.7 表单和验证

### 基本表单

```razor
<EditForm Model="@model" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="form-group">
        <label>用户名：</label>
        <InputText @bind-Value="model.UserName" class="form-control" />
        <ValidationMessage For="@(() => model.UserName)" />
    </div>

    <button type="submit">提交</button>
</EditForm>

@code {
    private UserModel model = new();

    private void HandleValidSubmit()
    {
        // 验证通过，处理提交
    }
}
```

### 验证规则

**使用DataAnnotations：**
```csharp
public class UserModel
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, ErrorMessage = "用户名长度不能超过50")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度至少6位")]
    public string Password { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }
}
```

### MiniMES实战案例：FluentValidation

MiniMES项目使用 **FluentValidation** 进行验证（比DataAnnotations更强大）：

**1. 定义验证器：**
```csharp
public class CreateWeighingRecordValidator : AbstractValidator<CreateWeighingRecordRequest>
{
    public CreateWeighingRecordValidator()
    {
        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("条码不能为空")
            .MaximumLength(100).WithMessage("条码长度不能超过100");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("重量必须大于0")
            .LessThanOrEqualTo(1000).WithMessage("重量不能超过1000磅");

        RuleFor(x => x.MeatTypeId)
            .GreaterThan(0).WithMessage("必须选择肉类类型");
    }
}
```

**2. 在服务中使用验证器：**
```csharp
public class WeighingRecordService : IWeighingRecordService
{
    private readonly IValidator<CreateWeighingRecordRequest> _validator;

    public async Task<WeighingRecordResponse> CreateAsync(
        CreateWeighingRecordRequest request, string createdBy)
    {
        // 验证请求
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 业务逻辑...
    }
}
```

**老王的经验：**
- ✅ 简单验证用 `DataAnnotations`
- ✅ 复杂验证用 `FluentValidation`（更灵活）
- ✅ 前后端共享验证逻辑（避免重复）
- ❌ 不要只在前端验证（后端必须验证）

---

## 2.8 JavaScript互操作（JS Interop）

### 为什么需要JS互操作？

有些功能C#无法直接实现，需要调用JavaScript：
- 操作DOM元素（聚焦输入框、滚动页面）
- 调用浏览器API（localStorage、下载文件）
- 使用第三方JS库（图表库、地图库）

### 调用JavaScript

**1. 定义JavaScript函数（wwwroot/js/utils.js）：**
```javascript
window.downloadFile = function(fileName, contentBase64) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + contentBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
```

**2. 在Blazor中调用：**
```razor
@inject IJSRuntime JSRuntime

@code {
    private async Task DownloadFile()
    {
        var fileBytes = await GenerateExcelFile();
        var base64 = Convert.ToBase64String(fileBytes);
        await JSRuntime.InvokeVoidAsync("downloadFile", "report.xlsx", base64);
    }
}
```

### JavaScript调用C#

**1. 定义C#方法：**
```csharp
@code {
    [JSInvokable]
    public static Task<string> GetServerTime()
    {
        return Task.FromResult(DateTime.Now.ToString());
    }
}
```

**2. 在JavaScript中调用：**
```javascript
const result = await DotNet.invokeMethodAsync('Minimes.Web', 'GetServerTime');
console.log(result);
```

### MiniMES实战案例：JS互操作

**1. 自动聚焦输入框：**
```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // 聚焦到条码输入框
        await JSRuntime.InvokeVoidAsync("eval",
            "document.querySelector('input[placeholder*=\"扫码\"]')?.focus()");
    }
}
```

**2. 下载Excel文件：**
```csharp
private async Task ExportToExcel()
{
    // 生成Excel文件
    var fileBytes = await ExcelExportService.ExportWeighingRecordsAsync(records);

    // 调用JS下载文件
    var base64 = Convert.ToBase64String(fileBytes);
    await JSRuntime.InvokeVoidAsync("downloadFile",
        $"称重记录_{DateTime.Now:yyyyMMdd}.xlsx", base64);
}
```

**3. 工具函数（wwwroot/js/utils.js）：**
```javascript
// 下载文件
window.downloadFile = function(fileName, contentBase64) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + contentBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// 显示提示
window.showAlert = function(message) {
    alert(message);
};

// 确认对话框
window.showConfirm = function(message) {
    return confirm(message);
};
```

**老王的经验：**
- ✅ JS互操作只在 `OnAfterRenderAsync` 中调用（此时DOM已渲染）
- ✅ 使用 `InvokeVoidAsync`（无返回值）或 `InvokeAsync<T>`（有返回值）
- ✅ 把常用JS函数封装到 `utils.js` 中
- ❌ 不要在 `OnInitialized` 中调用JS（DOM还没渲染）

---

**下一章：** [第三章：ASP.NET Core核心概念](#第三章aspnet-core核心概念)

---

# 第三章：ASP.NET Core核心概念

## 3.1 Program.cs - 应用启动配置

### Program.cs的作用

`Program.cs` 是ASP.NET Core应用的入口文件，负责：
1. 创建Web应用构建器（WebApplicationBuilder）
2. 注册服务到DI容器
3. 配置中间件管道
4. 启动应用

### MiniMES的Program.cs结构

```csharp
var builder = WebApplication.CreateBuilder(args);

// ========== 第一部分：注册服务 ==========

// 1. 数据库配置
builder.Services.AddDatabase(builder.Configuration);

// 2. 硬件服务配置
builder.Services.Configure<ScaleConfiguration>(
    builder.Configuration.GetSection("Hardware:Scale"));
builder.Services.AddSingleton<IScaleService, ScaleService>();

// 3. 应用层和基础设施层服务
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// 4. 认证和授权
builder.Services.AddCustomAuthentication();

// 5. Blazor服务
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 6. SignalR
builder.Services.AddSignalR();

// 7. 后台服务
builder.Services.AddHostedService<HardwareBackgroundService>();

var app = builder.Build();

// ========== 第二部分：配置中间件管道 ==========

// 1. 异常处理
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 2. HTTPS重定向
app.UseHttpsRedirection();

// 3. 静态文件
app.UseStaticFiles();

// 4. 国际化
app.UseRequestLocalization(localizationOptions);

// 5. 认证和授权（顺序重要！）
app.UseAuthentication();
app.UseAuthorization();

// 6. 路由
app.UseRouting();

// ========== 第三部分：初始化数据库 ==========

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedData.Initialize(dbContext);
}

// ========== 第四部分：映射端点 ==========

app.MapBlazorHub();
app.MapHub<HardwareHub>("/hardwareHub");
app.MapControllers();
app.MapFallbackToPage("/_Host");

// ========== 第五部分：启动应用 ==========

app.Run();
```

### 关键点解析

**1. WebApplicationBuilder vs WebApplication**
```csharp
var builder = WebApplication.CreateBuilder(args);  // 构建器
// ... 注册服务 ...
var app = builder.Build();  // 构建完成，得到应用实例
// ... 配置中间件 ...
app.Run();  // 启动应用
```

**2. 服务注册顺序**
- 服务注册顺序不重要（DI容器会自动解析依赖）
- 但建议按功能分组，便于维护

**3. 中间件顺序**
- ⚠️ 中间件顺序非常重要！
- 认证必须在授权之前
- 路由必须在端点映射之前

**老王的经验：**
- ✅ 把服务注册封装成扩展方法（`AddApplicationServices`）
- ✅ 中间件顺序严格遵守官方推荐
- ✅ 开发环境和生产环境分别配置
- ❌ 不要在 `Program.cs` 中写业务逻辑

---

## 3.2 依赖注入容器（DI Container）

### 什么是DI容器？

DI容器是一个对象工厂，负责：
1. 管理对象的创建
2. 管理对象的生命周期
3. 自动解析依赖关系

### 服务注册方式

```csharp
// 1. 注册接口和实现
builder.Services.AddScoped<IUserService, UserService>();

// 2. 注册具体类
builder.Services.AddScoped<UserService>();

// 3. 注册工厂方法
builder.Services.AddScoped<IUserService>(sp =>
{
    var dbContext = sp.GetRequiredService<ApplicationDbContext>();
    return new UserService(dbContext);
});

// 4. 注册实例
var config = new MyConfiguration();
builder.Services.AddSingleton(config);
```

### 服务生命周期详解

| 生命周期 | 创建时机 | 销毁时机 | 使用场景 |
|---------|---------|---------|---------|
| **Singleton** | 应用启动时 | 应用关闭时 | 无状态服务、硬件服务、配置 |
| **Scoped** | 每个请求开始时 | 请求结束时 | 数据库上下文、业务服务 |
| **Transient** | 每次注入时 | 使用完立即销毁 | 轻量级工具类 |

### MiniMES实战案例：服务注册

**1. 应用层服务注册（ApplicationServiceExtensions.cs）：**
```csharp
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // 业务服务（Scoped）
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWeighingRecordService, WeighingRecordService>();
        services.AddScoped<IMeatTypeService, MeatTypeService>();
        services.AddScoped<IQRCodeService, QRCodeService>();
        services.AddScoped<IReportService, ReportService>();

        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
```

**2. 基础设施层服务注册（InfrastructureServiceExtensions.cs）：**
```csharp
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        // 仓储（Scoped）
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWeighingRecordRepository, WeighingRecordRepository>();

        // Excel导出服务（Transient）
        services.AddTransient<IExcelExportService, ExcelExportService>();

        // 密码哈希服务（Singleton）
        services.AddSingleton<IPasswordHashService, PasswordHashService>();

        return services;
    }
}
```

**3. 硬件服务注册（Program.cs）：**
```csharp
// 硬件服务（Singleton）
builder.Services.Configure<ScaleConfiguration>(
    builder.Configuration.GetSection("Hardware:Scale"));
builder.Services.AddSingleton<IScaleService, ScaleService>();
```

**老王的经验：**
- ✅ 把服务注册封装成扩展方法，保持 `Program.cs` 简洁
- ✅ 数据库上下文用 `Scoped`（每个请求独立）
- ✅ 硬件服务用 `Singleton`（全局唯一，避免重复初始化）
- ❌ 不要在 `Singleton` 服务中注入 `Scoped` 服务

---

## 3.3 中间件管道（Middleware Pipeline）

### 什么是中间件？

中间件是处理HTTP请求和响应的组件，按顺序组成一个管道。

**工作原理：**
```
HTTP请求
  ↓
中间件1 → 中间件2 → 中间件3 → 端点
  ↑         ↑         ↑         ↓
HTTP响应 ← ← ← ← ← ← ← ← ← ← ← ←
```

### 常用中间件

| 中间件 | 作用 | 顺序 |
|-------|------|-----|
| `UseExceptionHandler` | 异常处理 | 第1个 |
| `UseHsts` | HTTPS严格传输安全 | 第2个 |
| `UseHttpsRedirection` | HTTP重定向到HTTPS | 第3个 |
| `UseStaticFiles` | 静态文件服务 | 第4个 |
| `UseRouting` | 路由匹配 | 第5个 |
| `UseAuthentication` | 认证 | 第6个 |
| `UseAuthorization` | 授权 | 第7个 |
| `UseEndpoints` | 端点映射 | 最后 |

### MiniMES的中间件管道

```csharp
var app = builder.Build();

// 1. 异常处理（最外层，捕获所有异常）
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 2. HTTPS重定向
app.UseHttpsRedirection();

// 3. 静态文件（CSS、JS、图片）
app.UseStaticFiles();

// 4. 国际化
var supportedCultures = new[] {
    new CultureInfo("en-US"),
    new CultureInfo("zh-CN")
};
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
app.UseRequestLocalization(localizationOptions);

// 5. 认证（必须在授权之前）
app.UseAuthentication();

// 6. 授权
app.UseAuthorization();

// 7. 路由
app.UseRouting();

// 8. 端点映射
app.MapBlazorHub();
app.MapHub<HardwareHub>("/hardwareHub");
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();
```

### 自定义中间件

```csharp
// 定义中间件
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 请求前
        Console.WriteLine($"请求：{context.Request.Path}");

        // 调用下一个中间件
        await _next(context);

        // 响应后
        Console.WriteLine($"响应：{context.Response.StatusCode}");
    }
}

// 注册中间件
app.UseMiddleware<RequestLoggingMiddleware>();
```

**老王的经验：**
- ✅ 中间件顺序严格遵守官方推荐
- ✅ 异常处理放在最外层
- ✅ 认证必须在授权之前
- ❌ 不要随意调整中间件顺序（会出问题）

---

## 3.4 配置系统（Configuration）

### 配置文件层次

ASP.NET Core支持多层配置文件：

```
appsettings.json（基础配置）
  ↓
appsettings.Development.json（开发环境覆盖）
  ↓
appsettings.Production.json（生产环境覆盖）
  ↓
环境变量（最高优先级）
```

### MiniMES的配置文件

**appsettings.json（基础配置）：**
```json
{
  "Database": {
    "Provider": "SQLite"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=minimes.db",
    "MySqlConnection": "Server=localhost;Database=minimes;User=root;Password=123456;"
  },
  "Hardware": {
    "ScaleType": "Serial",
    "Scale": {
      "PortName": "COM3",
      "BaudRate": 9600,
      "Protocol": "Toledo"
    }
  },
  "WeightValidation": {
    "MinWeight": 0.001,
    "MaxWeight": 1000.0,
    "StableThreshold": 0.01,
    "StableReadings": 3
  }
}
```

**appsettings.Production.json（生产环境）：**
```json
{
  "Database": {
    "Provider": "MySQL"
  },
  "ConnectionStrings": {
    "MySqlConnection": "Server=192.168.1.100;Database=minimes;User=admin;Password=prod_password;"
  },
  "Hardware": {
    "Scale": {
      "PortName": "COM1"
    }
  }
}
```

### 读取配置

**1. 直接读取：**
```csharp
var portName = builder.Configuration["Hardware:Scale:PortName"];
var baudRate = builder.Configuration.GetValue<int>("Hardware:Scale:BaudRate");
```

**2. 绑定到强类型对象：**
```csharp
// 定义配置类
public class ScaleConfiguration
{
    public string PortName { get; set; } = "COM3";
    public int BaudRate { get; set; } = 9600;
    public string Protocol { get; set; } = "Toledo";
}

// 注册配置
builder.Services.Configure<ScaleConfiguration>(
    builder.Configuration.GetSection("Hardware:Scale"));

// 使用配置
public class ScaleService
{
    private readonly ScaleConfiguration _config;

    public ScaleService(IOptions<ScaleConfiguration> options)
    {
        _config = options.Value;
    }
}
```

### 环境变量配置

```bash
# Windows
set ASPNETCORE_ENVIRONMENT=Production
set Hardware__Scale__PortName=COM1

# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Production
export Hardware__Scale__PortName=COM1
```

**注意：** 环境变量中用 `__`（双下划线）代替 `:`（冒号）

### MiniMES实战案例：数据库切换

```csharp
// DatabaseExtensions.cs
public static IServiceCollection AddDatabase(
    this IServiceCollection services,
    IConfiguration configuration)
{
    var provider = configuration["Database:Provider"];

    if (provider == "MySQL")
    {
        // 使用MySQL
        var connectionString = configuration.GetConnectionString("MySqlConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString,
                ServerVersion.AutoDetect(connectionString)));
    }
    else
    {
        // 使用SQLite（默认）
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
    }

    return services;
}
```

**老王的经验：**
- ✅ 敏感信息（密码）用环境变量，不要提交到Git
- ✅ 开发环境用SQLite，生产环境用MySQL
- ✅ 配置绑定到强类型对象，避免硬编码字符串
- ❌ 不要把生产环境密码写在配置文件中

---

**下一节：** [3.5 认证和授权（Authentication & Authorization）](#35-认证和授权authentication--authorization)

---

## 3.5 认证和授权（Authentication & Authorization）

### 认证 vs 授权

| 概念 | 英文 | 作用 | 问题 |
|-----|------|------|------|
| **认证** | Authentication | 验证用户身份 | "你是谁？" |
| **授权** | Authorization | 验证用户权限 | "你能做什么？" |

**老王的大白话：**
- **认证**：门卫检查你的身份证，确认你是谁
- **授权**：门卫检查你的通行证，确认你能进哪些房间

### Cookie认证流程

```
1. 用户登录 → 验证用户名密码
   ↓
2. 创建Claims（声明）
   ↓
3. 生成Cookie
   ↓
4. 返回Cookie给浏览器
   ↓
5. 后续请求自动携带Cookie
   ↓
6. 服务器验证Cookie → 识别用户身份
```

### MiniMES实战案例：认证配置

**1. 配置认证服务（AuthenticationExtensions.cs）：**
```csharp
public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthentication(
        this IServiceCollection services)
    {
        // Cookie认证
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

        // 授权策略
        services.AddAuthorization(options =>
        {
            // Admin策略 - 只有Administrator角色可以访问
            options.AddPolicy("Admin", policy =>
                policy.RequireRole("Administrator"));

            // Operator策略 - Operator或Administrator都可以访问
            options.AddPolicy("Operator", policy =>
                policy.RequireRole("Operator", "Administrator"));

            // Authenticated策略 - 任何已认证用户都可以访问
            options.AddPolicy("Authenticated", policy =>
                policy.RequireAuthenticatedUser());
        });

        return services;
    }
}
```

**2. 登录逻辑（LoginModel.cs）：**
```csharp
public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    // 验证用户名密码
    var user = await _userService.AuthenticateAsync(Username, Password);
    if (user == null)
    {
        ErrorMessage = "用户名或密码错误";
        return Page();
    }

    // 创建Claims（声明）
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
        new Claim("UserId", user.Id.ToString()),
        new Claim("DisplayName", user.DisplayName ?? user.UserName)
    };

    // 创建身份标识
    var identity = new ClaimsIdentity(claims,
        CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    // 登录（生成Cookie）
    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties
        {
            IsPersistent = RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        });

    // 重定向到返回URL
    return LocalRedirect(returnUrl ?? "/");
}
```

**3. 页面权限控制：**
```razor
<!-- 管理员专用页面 -->
@page "/users"
@attribute [Authorize(Policy = "Admin")]

<!-- 操作员+管理员页面 -->
@page "/weighing"
@attribute [Authorize(Policy = "Operator")]

<!-- 所有已登录用户页面 -->
@page "/profile"
@attribute [Authorize]
```

**4. 菜单权限控制：**
```razor
<AuthorizeView Policy="Admin">
    <Authorized>
        <div class="nav-item">
            <NavLink href="/users">用户管理</NavLink>
        </div>
    </Authorized>
</AuthorizeView>

<AuthorizeView>
    <Authorized>
        <span>欢迎，@context.User.Identity?.Name</span>
    </Authorized>
    <NotAuthorized>
        <a href="/Account/Login">登录</a>
    </NotAuthorized>
</AuthorizeView>
```

**老王的经验：**
- ✅ 使用Cookie认证（简单、适合内网）
- ✅ 定义授权策略（Admin、Operator、Authenticated）
- ✅ 三层防护：页面权限 + 菜单控制 + Service验证
- ❌ 不要只在前端控制权限（后端必须验证）

---

## 3.6 SignalR实时通信

### 什么是SignalR？

**SignalR** 是ASP.NET Core的实时通信库，支持：
- WebSocket（首选）
- Server-Sent Events（SSE）
- Long Polling（兜底）

### SignalR工作原理

```
客户端（浏览器）
  ↓ 建立连接
SignalR Hub（服务器）
  ↓ 推送消息
客户端（浏览器）
```

**特点：**
- ✅ 双向通信（服务器可以主动推送）
- ✅ 自动重连（断线自动恢复）
- ✅ 分组广播（可以推送给特定用户）

### MiniMES实战案例：硬件数据推送

**1. 定义Hub（HardwareHub.cs）：**
```csharp
public class HardwareHub : Hub
{
    /// <summary>
    /// 推送重量数据到所有客户端
    /// </summary>
    public async Task BroadcastWeight(decimal weight, string unit, bool isStable)
    {
        await Clients.All.SendAsync("ReceiveWeight", new
        {
            weight,
            unit,
            isStable,
            timestamp = DateTime.Now
        });
    }

    /// <summary>
    /// 推送扫码数据到所有客户端
    /// </summary>
    public async Task BroadcastBarcode(string barcode, string scannerType)
    {
        await Clients.All.SendAsync("ReceiveBarcode", new
        {
            barcode,
            scannerType,
            timestamp = DateTime.Now
        });
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"客户端已连接: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"客户端已断开: {Context.ConnectionId}");
    }
}
```

**2. 注册Hub（Program.cs）：**
```csharp
// 注册SignalR服务
builder.Services.AddSignalR();

// 映射Hub端点
app.MapHub<HardwareHub>("/hardwareHub");
```

**3. 后台服务推送数据（HardwareBackgroundService.cs）：**
```csharp
public class HardwareBackgroundService : BackgroundService
{
    private readonly IHubContext<HardwareHub> _hubContext;
    private readonly IScaleService _scaleService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 订阅电子秤事件
        _scaleService.OnWeightReceived += async (weight, unit, isStable) =>
        {
            // 推送到所有客户端
            await _hubContext.Clients.All.SendAsync("ReceiveWeight", new
            {
                weight,
                unit,
                isStable,
                timestamp = DateTime.Now
            }, stoppingToken);
        };

        // 启动电子秤
        await _scaleService.StartAsync();
    }
}
```

**4. 客户端订阅（WeighingPage.razor）：**
```csharp
@code {
    private HubConnection? hubConnection;

    protected override async Task OnInitializedAsync()
    {
        // 创建SignalR连接
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/hardwareHub"))
            .Build();

        // 订阅重量数据
        hubConnection.On<object>("ReceiveWeight", (data) =>
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var weightData = System.Text.Json.JsonSerializer.Deserialize<WeightData>(json);

            // 更新UI
            currentWeight = weightData.weight;
            isStable = weightData.isStable;
            InvokeAsync(StateHasChanged);
        });

        // 订阅扫码数据
        hubConnection.On<object>("ReceiveBarcode", (data) =>
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var barcodeData = System.Text.Json.JsonSerializer.Deserialize<BarcodeData>(json);

            // 更新UI
            currentBarcode = barcodeData.barcode;
            InvokeAsync(StateHasChanged);
        });

        // 启动连接
        await hubConnection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
```

**老王的经验：**
- ✅ SignalR适合实时性要求高的场景（电子秤、聊天、通知）
- ✅ 使用 `IHubContext` 在后台服务中推送消息
- ✅ 客户端记得在 `DisposeAsync` 中释放连接
- ❌ 不要在SignalR中传输大量数据（会影响性能）

---

## 3.7 后台服务（Hosted Services）

### 什么是后台服务？

**后台服务（Hosted Service）** 是在应用启动时自动运行的后台任务，适合：
- 定时任务（每天凌晨备份数据库）
- 硬件监听（持续读取电子秤数据）
- 消息队列处理（处理异步任务）

### 实现后台服务

```csharp
public class MyBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // 执行任务
            Console.WriteLine("后台任务执行中...");

            // 等待一段时间
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}

// 注册后台服务
builder.Services.AddHostedService<MyBackgroundService>();
```

### MiniMES实战案例：硬件后台服务

```csharp
public class HardwareBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<HardwareHub> _hubContext;
    private readonly ILogger<HardwareBackgroundService> _logger;

    public HardwareBackgroundService(
        IServiceProvider serviceProvider,
        IHubContext<HardwareHub> hubContext,
        ILogger<HardwareBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("硬件后台服务启动");

        // 从DI容器获取Singleton服务
        var scaleService = _serviceProvider.GetRequiredService<IScaleService>();

        // 订阅电子秤事件
        scaleService.OnWeightReceived += async (weight, unit, isStable) =>
        {
            try
            {
                // 推送到所有客户端
                await _hubContext.Clients.All.SendAsync("ReceiveWeight", new
                {
                    weight,
                    unit,
                    isStable,
                    timestamp = DateTime.Now
                }, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "推送重量数据失败");
            }
        };

        // 启动电子秤
        try
        {
            await scaleService.StartAsync();
            _logger.LogInformation("电子秤服务启动成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "电子秤服务启动失败");
        }

        // 保持运行
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        _logger.LogInformation("硬件后台服务停止");
    }
}
```

**注册后台服务：**
```csharp
builder.Services.AddHostedService<HardwareBackgroundService>();
```

**老王的经验：**
- ✅ 后台服务适合长时间运行的任务
- ✅ 使用 `IServiceProvider` 获取服务（避免生命周期问题）
- ✅ 使用 `CancellationToken` 优雅停止
- ✅ 记录日志，方便排查问题
- ❌ 不要在后台服务中注入 `Scoped` 服务（会报错）

---

**下一章：** [第四章：MiniMES项目架构分析](#第四章minimes项目架构分析)

---

# 第四章：MiniMES项目架构分析

## 4.1 Clean Architecture分层设计

### 什么是Clean Architecture？

**Clean Architecture（整洁架构）** 是一种软件架构模式，核心思想是：
- **依赖倒置**：外层依赖内层，内层不依赖外层
- **关注点分离**：每层只关注自己的职责
- **可测试性**：业务逻辑独立，易于测试

### 分层结构

```
┌─────────────────────────────────────┐
│         Web层（表示层）              │  ← 用户界面
│  Blazor组件、Razor Pages、Controllers│
└─────────────────────────────────────┘
              ↓ 依赖
┌─────────────────────────────────────┐
│    Infrastructure层（基础设施层）     │  ← 技术实现
│  数据库、硬件、Excel、外部API         │
└─────────────────────────────────────┘
              ↓ 依赖
┌─────────────────────────────────────┐
│    Application层（应用层）           │  ← 业务逻辑
│  服务、DTOs、验证器、映射配置         │
└─────────────────────────────────────┘
              ↓ 依赖
┌─────────────────────────────────────┐
│      Domain层（领域层）              │  ← 核心业务
│  实体、值对象、枚举、仓储接口         │
└─────────────────────────────────────┘
```

### MiniMES的分层设计

| 层级 | 项目名称 | 职责 | 依赖 |
|-----|---------|------|------|
| **Domain** | Minimes.Domain | 核心业务实体、枚举、接口 | 无依赖 |
| **Application** | Minimes.Application | 业务服务、DTOs、验证 | Domain |
| **Infrastructure** | Minimes.Infrastructure | 数据库、硬件、Excel | Domain + Application |
| **Web** | Minimes.Web | Blazor组件、页面 | Domain + Application + Infrastructure |

### 为什么这么设计？

**1. 依赖倒置原则（DIP）：**
```
❌ 错误：Application层直接依赖具体的数据库实现
Application → SqlServerRepository

✅ 正确：Application层依赖接口，Infrastructure层实现接口
Application → IRepository ← SqlServerRepository
```

**2. 关注点分离：**
- Domain层：只关心业务规则（什么是称重记录？有哪些属性？）
- Application层：只关心业务流程（如何创建称重记录？需要验证什么？）
- Infrastructure层：只关心技术实现（如何存储到数据库？如何读取电子秤？）
- Web层：只关心用户界面（如何展示数据？如何响应用户操作？）

**3. 可测试性：**
```csharp
// 测试Application层服务时，可以Mock仓储
var mockRepository = new Mock<IWeighingRecordRepository>();
var service = new WeighingRecordService(mockRepository.Object);
```

**老王的大白话：**
- **Domain层**：定义业务规则，是整个系统的核心
- **Application层**：编排业务流程，调用Domain层和Infrastructure层
- **Infrastructure层**：干脏活累活，和数据库、硬件打交道
- **Web层**：展示数据，响应用户操作

---

## 4.2 Domain层 - 领域模型

### Domain层的职责

Domain层是整个系统的核心，包含：
- **实体（Entities）**：核心业务对象
- **值对象（Value Objects）**：不可变的值类型
- **枚举（Enums）**：业务枚举
- **仓储接口（Repository Interfaces）**：数据访问抽象

### MiniMES的Domain层结构

```
Minimes.Domain/
├── Entities/                    # 实体
│   ├── User.cs                 # 用户实体
│   ├── MeatType.cs             # 肉类类型实体
│   ├── QRCode.cs               # 二维码实体
│   └── WeighingRecord.cs       # 称重记录实体
├── ValueObjects/               # 值对象
│   ├── Barcode.cs              # 条码值对象
│   └── Weight.cs               # 重量值对象
├── Enums/                      # 枚举
│   ├── UserRole.cs             # 用户角色枚举
│   └── WeightUnit.cs           # 重量单位枚举
└── Interfaces/                 # 仓储接口
    ├── IUserRepository.cs
    ├── IMeatTypeRepository.cs
    ├── IQRCodeRepository.cs
    └── IWeighingRecordRepository.cs
```

### 实体示例：WeighingRecord

```csharp
namespace Minimes.Domain.Entities;

/// <summary>
/// 称重记录实体
/// </summary>
public class WeighingRecord
{
    public int Id { get; set; }

    /// <summary>
    /// 二维码内容（完整）
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 用户编号（从二维码解析）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 肉类类型ID
    /// </summary>
    public int MeatTypeId { get; set; }

    /// <summary>
    /// 肉类类型（导航属性）
    /// </summary>
    public MeatType? MeatType { get; set; }

    /// <summary>
    /// 重量（磅）
    /// </summary>
    public decimal WeightInPounds { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
}
```

### 枚举示例：UserRole

```csharp
namespace Minimes.Domain.Enums;

/// <summary>
/// 用户角色枚举
/// </summary>
public enum UserRole
{
    /// <summary>
    /// 操作员 - 只能进行生产操作
    /// </summary>
    Operator = 1,

    /// <summary>
    /// 管理员 - 拥有所有权限
    /// </summary>
    Administrator = 2
}
```

### 仓储接口示例：IWeighingRecordRepository

```csharp
namespace Minimes.Domain.Interfaces;

/// <summary>
/// 称重记录仓储接口
/// </summary>
public interface IWeighingRecordRepository : IRepository<WeighingRecord>
{
    /// <summary>
    /// 分页查询（数据库层面过滤）
    /// </summary>
    Task<(List<WeighingRecord> records, int totalCount)> QueryPagedAsync(
        DateTime? startDate,
        DateTime? endDate,
        string? barcode,
        int? meatTypeId,
        int pageNumber,
        int pageSize);

    /// <summary>
    /// 获取条码统计数据
    /// </summary>
    Task<List<BarcodeStatistic>> GetBarcodeStatisticsAsync(
        DateTime? startDate,
        DateTime? endDate);

    /// <summary>
    /// 获取今日统计数据
    /// </summary>
    Task<TodayStatistic> GetTodayStatisticsAsync();

    /// <summary>
    /// 获取用户操作统计
    /// </summary>
    Task<UserOperationStatistic> GetUserOperationStatisticsAsync(string userName);
}
```

**老王的经验：**
- ✅ Domain层不依赖任何其他层（纯粹的业务模型）
- ✅ 实体包含业务属性和导航属性
- ✅ 仓储接口定义数据访问抽象
- ❌ 不要在Domain层引用EF Core、ASP.NET Core等框架

---

## 4.3 Application层 - 业务逻辑

### Application层的职责

Application层负责编排业务流程，包含：
- **服务（Services）**：业务逻辑实现
- **DTOs（Data Transfer Objects）**：数据传输对象
- **验证器（Validators）**：FluentValidation验证规则
- **映射配置（Mappings）**：AutoMapper配置

### MiniMES的Application层结构

```
Minimes.Application/
├── Services/                   # 业务服务
│   ├── UserService.cs
│   ├── MeatTypeService.cs
│   ├── QRCodeService.cs
│   ├── WeighingRecordService.cs
│   └── ReportService.cs
├── DTOs/                       # 数据传输对象
│   ├── User/
│   │   ├── CreateUserRequest.cs
│   │   ├── UpdateUserRequest.cs
│   │   └── UserResponse.cs
│   └── WeighingRecord/
│       ├── CreateWeighingRecordRequest.cs
│       ├── UpdateWeighingRecordRequest.cs
│       ├── WeighingRecordResponse.cs
│       └── WeighingRecordQueryRequest.cs
├── Validators/                 # 验证器
│   ├── CreateUserValidator.cs
│   └── CreateWeighingRecordValidator.cs
├── Mappings/                   # AutoMapper配置
│   └── MappingProfile.cs
├── Interfaces/                 # 服务接口
│   ├── IUserService.cs
│   ├── IMeatTypeService.cs
│   ├── IQRCodeService.cs
│   ├── IWeighingRecordService.cs
│   └── IReportService.cs
└── Resources/                  # 国际化资源
    ├── SharedResource.zh-CN.resx
    └── SharedResource.en-US.resx
```

### 服务示例：WeighingRecordService

```csharp
namespace Minimes.Application.Services;

public class WeighingRecordService : IWeighingRecordService
{
    private readonly IWeighingRecordRepository _repository;
    private readonly IValidator<CreateWeighingRecordRequest> _createValidator;
    private readonly IMapper _mapper;

    public WeighingRecordService(
        IWeighingRecordRepository repository,
        IValidator<CreateWeighingRecordRequest> createValidator,
        IMapper mapper)
    {
        _repository = repository;
        _createValidator = createValidator;
        _mapper = mapper;
    }

    public async Task<WeighingRecordResponse> CreateAsync(
        CreateWeighingRecordRequest request,
        string createdBy)
    {
        // 1. 验证请求
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 2. 映射到实体
        var record = _mapper.Map<WeighingRecord>(request);
        record.CreatedBy = createdBy;
        record.CreatedAt = DateTime.Now;

        // 3. 保存到数据库
        await _repository.AddAsync(record);

        // 4. 映射到响应DTO
        return _mapper.Map<WeighingRecordResponse>(record);
    }

    public async Task<(List<WeighingRecordResponse> records, int totalCount)> QueryAsync(
        WeighingRecordQueryRequest query)
    {
        // 调用仓储的数据库层面查询（性能优化）
        var (records, totalCount) = await _repository.QueryPagedAsync(
            query.StartDate,
            query.EndDate,
            query.Barcode,
            query.MeatTypeId,
            query.PageNumber,
            query.PageSize);

        // 映射到响应DTO
        var responses = _mapper.Map<List<WeighingRecordResponse>>(records);

        return (responses, totalCount);
    }
}
```

### DTO示例：CreateWeighingRecordRequest

```csharp
namespace Minimes.Application.DTOs.WeighingRecord;

/// <summary>
/// 创建称重记录请求
/// </summary>
public class CreateWeighingRecordRequest
{
    /// <summary>
    /// 二维码内容
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 用户编号
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 肉类类型ID
    /// </summary>
    public int MeatTypeId { get; set; }

    /// <summary>
    /// 重量（磅）
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; set; }
}
```

### 验证器示例：CreateWeighingRecordValidator

```csharp
namespace Minimes.Application.Validators;

public class CreateWeighingRecordValidator : AbstractValidator<CreateWeighingRecordRequest>
{
    public CreateWeighingRecordValidator()
    {
        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("条码不能为空")
            .MaximumLength(100).WithMessage("条码长度不能超过100");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("用户编号不能为空")
            .MaximumLength(50).WithMessage("用户编号长度不能超过50");

        RuleFor(x => x.MeatTypeId)
            .GreaterThan(0).WithMessage("必须选择肉类类型");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("重量必须大于0")
            .LessThanOrEqualTo(1000).WithMessage("重量不能超过1000磅");

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("备注长度不能超过500");
    }
}
```

### AutoMapper配置示例：MappingProfile

```csharp
namespace Minimes.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // WeighingRecord映射
        CreateMap<CreateWeighingRecordRequest, WeighingRecord>()
            .ForMember(dest => dest.WeightInPounds, opt => opt.MapFrom(src => src.Weight));

        CreateMap<WeighingRecord, WeighingRecordResponse>()
            .ForMember(dest => dest.MeatTypeName, opt => opt.MapFrom(src => src.MeatType!.Name));

        // User映射
        CreateMap<CreateUserRequest, User>();
        CreateMap<User, UserResponse>();
    }
}
```

**老王的经验：**
- ✅ Application层只依赖Domain层（不依赖Infrastructure层）
- ✅ 使用DTOs隔离内部实体和外部接口
- ✅ 使用FluentValidation进行复杂验证
- ✅ 使用AutoMapper减少手动映射代码
- ❌ 不要在Application层直接操作数据库（通过仓储接口）

---

**下一节：** [4.4 Infrastructure层 - 基础设施](#44-infrastructure层---基础设施)

---

## 4.4 Infrastructure层 - 基础设施

### Infrastructure层的职责

Infrastructure层负责技术实现，包含：
- **数据库访问**：EF Core上下文、仓储实现
- **硬件集成**：电子秤、扫码枪服务
- **Excel导出**：EPPlus集成
- **外部服务**：第三方API调用

### MiniMES的Infrastructure层结构

```
Minimes.Infrastructure/
├── Persistence/                # 数据库持久化
│   ├── ApplicationDbContext.cs        # EF Core上下文
│   ├── DatabaseExtensions.cs          # 数据库配置扩展
│   ├── SeedData.cs                    # 种子数据
│   └── Configurations/                # 实体配置
│       ├── UserConfiguration.cs
│       ├── MeatTypeConfiguration.cs
│       └── WeighingRecordConfiguration.cs
├── Repositories/               # 仓储实现
│   ├── Repository.cs                  # 通用仓储基类
│   ├── UserRepository.cs
│   ├── MeatTypeRepository.cs
│   └── WeighingRecordRepository.cs
├── Hardware/                   # 硬件集成
│   ├── ScaleService.cs                # 电子秤服务
│   ├── WiFiScaleService.cs            # WiFi电子秤服务
│   └── BarcodeScannerService.cs       # 扫码枪服务
├── Excel/                      # Excel导出
│   └── ExcelExportService.cs
└── Services/                   # 其他服务
    └── PasswordHashService.cs         # 密码哈希服务
```

### 数据库上下文：ApplicationDbContext

```csharp
namespace Minimes.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet定义
    public DbSet<User> Users => Set<User>();
    public DbSet<MeatType> MeatTypes => Set<MeatType>();
    public DbSet<QRCode> QRCodes => Set<QRCode>();
    public DbSet<WeighingRecord> WeighingRecords => Set<WeighingRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 应用所有配置
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

### 实体配置：WeighingRecordConfiguration

```csharp
namespace Minimes.Infrastructure.Persistence.Configurations;

public class WeighingRecordConfiguration : IEntityTypeConfiguration<WeighingRecord>
{
    public void Configure(EntityTypeBuilder<WeighingRecord> builder)
    {
        builder.ToTable("WeighingRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Barcode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.WeightInPounds)
            .HasPrecision(18, 3);

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(50);

        // 索引
        builder.HasIndex(x => x.Barcode);
        builder.HasIndex(x => x.CreatedAt);

        // 外键关系
        builder.HasOne(x => x.MeatType)
            .WithMany()
            .HasForeignKey(x => x.MeatTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 仓储实现：WeighingRecordRepository

```csharp
namespace Minimes.Infrastructure.Repositories;

public class WeighingRecordRepository : Repository<WeighingRecord>, IWeighingRecordRepository
{
    public WeighingRecordRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(List<WeighingRecord> records, int totalCount)> QueryPagedAsync(
        DateTime? startDate,
        DateTime? endDate,
        string? barcode,
        int? meatTypeId,
        int pageNumber,
        int pageSize)
    {
        // 构建查询（数据库层面过滤）
        var query = _context.WeighingRecords
            .Include(x => x.MeatType)
            .AsQueryable();

        // 日期范围过滤
        if (startDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= endDate.Value);
        }

        // 条码过滤
        if (!string.IsNullOrWhiteSpace(barcode))
        {
            query = query.Where(x => x.Barcode.Contains(barcode));
        }

        // 肉类类型过滤
        if (meatTypeId.HasValue && meatTypeId.Value > 0)
        {
            query = query.Where(x => x.MeatTypeId == meatTypeId.Value);
        }

        // 总数（过滤后）
        var totalCount = await query.CountAsync();

        // 分页查询
        var records = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (records, totalCount);
    }

    public async Task<TodayStatistic> GetTodayStatisticsAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        // 数据库聚合查询（性能优化）
        var totalRecords = await _context.WeighingRecords
            .Where(x => x.CreatedAt >= today && x.CreatedAt < tomorrow)
            .CountAsync();

        var totalWeight = await _context.WeighingRecords
            .Where(x => x.CreatedAt >= today && x.CreatedAt < tomorrow)
            .SumAsync(x => (decimal?)x.WeightInPounds) ?? 0;

        var uniqueBarcodes = await _context.WeighingRecords
            .Where(x => x.CreatedAt >= today && x.CreatedAt < tomorrow)
            .Select(x => x.Barcode)
            .Distinct()
            .CountAsync();

        return new TodayStatistic
        {
            TotalRecords = totalRecords,
            TotalWeight = totalWeight,
            UniqueBarcodes = uniqueBarcodes
        };
    }
}
```

### 硬件服务：ScaleService

```csharp
namespace Minimes.Infrastructure.Hardware;

public class ScaleService : IScaleService
{
    private readonly ScaleConfiguration _config;
    private SerialPort? _serialPort;
    private bool _isRunning;

    public event Action<decimal, string, bool>? OnWeightReceived;

    public ScaleService(IOptions<ScaleConfiguration> options)
    {
        _config = options.Value;
    }

    public async Task StartAsync()
    {
        if (_isRunning) return;

        try
        {
            // 打开串口
            _serialPort = new SerialPort(_config.PortName, _config.BaudRate);
            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.Open();

            _isRunning = true;
            Console.WriteLine($"电子秤服务启动成功：{_config.PortName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"电子秤服务启动失败：{ex.Message}");
            throw;
        }

        await Task.CompletedTask;
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            var data = _serialPort!.ReadLine();
            var (weight, unit, isStable) = ParseWeightData(data);

            // 触发事件
            OnWeightReceived?.Invoke(weight, unit, isStable);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析重量数据失败：{ex.Message}");
        }
    }

    private (decimal weight, string unit, bool isStable) ParseWeightData(string data)
    {
        // 根据协议解析数据（Toledo、Mettler等）
        // 这里简化处理
        var weight = decimal.Parse(data);
        return (weight, "lb", true);
    }

    public async Task StopAsync()
    {
        if (!_isRunning) return;

        _serialPort?.Close();
        _serialPort?.Dispose();
        _isRunning = false;

        await Task.CompletedTask;
    }
}
```

**老王的经验：**
- ✅ Infrastructure层实现Domain层定义的接口
- ✅ 使用EF Core配置类分离实体配置
- ✅ 仓储实现中使用IQueryable进行数据库层面过滤
- ✅ 硬件服务使用事件机制推送数据
- ❌ 不要在Infrastructure层写业务逻辑

---

## 4.5 Web层 - 表示层

### Web层的职责

Web层负责用户界面，包含：
- **Blazor组件**：页面和共享组件
- **SignalR Hub**：实时通信
- **后台服务**：硬件数据推送
- **静态资源**：CSS、JS、图片

### MiniMES的Web层结构

```
Minimes.Web/
├── Pages/                      # Blazor页面
│   ├── Index.razor                    # 首页
│   ├── Login.razor                    # 登录（重定向）
│   ├── Logout.razor                   # 登出
│   ├── Profile.razor                  # 个人中心
│   ├── Settings.razor                 # 系统设置
│   ├── HardwareTest.razor             # 硬件测试
│   ├── Users/                         # 用户管理
│   │   ├── Index.razor
│   │   ├── Create.razor
│   │   └── Edit.razor
│   ├── MeatTypes/                     # 肉类类型管理
│   │   ├── Index.razor
│   │   ├── Create.razor
│   │   └── Edit.razor
│   ├── QRCodes/                       # 二维码管理
│   │   ├── Index.razor
│   │   ├── Create.razor
│   │   ├── BatchCreate.razor
│   │   └── Print.razor
│   ├── Weighing/                      # 称重模块
│   │   ├── WeighingPage.razor
│   │   └── RecordList.razor
│   └── Reports/                       # 报表模块
│       ├── Production.razor
│       └── Tracing.razor
├── Shared/                     # 共享组件
│   ├── MainLayout.razor               # 主布局
│   ├── NavMenu.razor                  # 导航菜单
│   ├── CultureSelector.razor          # 语言切换
│   └── RedirectToLogin.razor          # 登录重定向
├── Hubs/                       # SignalR Hub
│   └── HardwareHub.cs
├── Services/                   # 前端服务
│   └── HardwareBackgroundService.cs   # 硬件后台服务
├── wwwroot/                    # 静态资源
│   ├── css/
│   │   ├── site.css
│   │   └── tablet.css
│   └── js/
│       └── utils.js
├── Program.cs                  # 应用启动
└── appsettings.json            # 配置文件
```

### 页面组件示例：WeighingPage.razor（简化版）

```razor
@page "/weighing"
@attribute [Authorize(Policy = "Operator")]
@inject IWeighingRecordService WeighingRecordService
@inject NavigationManager Navigation
@implements IAsyncDisposable

<h3>生产称重</h3>

<div class="row">
    <div class="col-md-8">
        <!-- 条码输入 -->
        <input type="text" @bind="currentBarcode" @bind:event="oninput" />

        <!-- 重量输入 -->
        <input type="number" @bind="manualWeightInput" />

        <!-- 保存按钮 -->
        <button @onclick="SaveRecord">保存</button>
    </div>

    <div class="col-md-4">
        <!-- 今日统计 -->
        <div class="card">
            <div class="card-body">
                <p>记录数：@todaySummary?.TotalRecords</p>
                <p>总重量：@todaySummary?.TotalWeight lb</p>
            </div>
        </div>
    </div>
</div>

@code {
    private HubConnection? hubConnection;
    private string currentBarcode = string.Empty;
    private decimal manualWeightInput = 0;
    private TodaySummary? todaySummary;

    protected override async Task OnInitializedAsync()
    {
        // 创建SignalR连接
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/hardwareHub"))
            .Build();

        // 订阅扫码事件
        hubConnection.On<object>("ReceiveBarcode", (data) =>
        {
            currentBarcode = data.barcode;
            InvokeAsync(StateHasChanged);
        });

        // 启动连接
        await hubConnection.StartAsync();

        // 加载统计数据
        todaySummary = await WeighingRecordService.GetTodaySummaryAsync();
    }

    private async Task SaveRecord()
    {
        var request = new CreateWeighingRecordRequest
        {
            Barcode = currentBarcode,
            Weight = manualWeightInput
        };

        await WeighingRecordService.CreateAsync(request, "admin");

        // 刷新统计
        todaySummary = await WeighingRecordService.GetTodaySummaryAsync();

        // 清空表单
        currentBarcode = string.Empty;
        manualWeightInput = 0;
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection != null)
        {
            await hubConnection.DisposeAsync();
        }
    }
}
```

**老王的经验：**
- ✅ Web层只依赖Application层的服务接口
- ✅ 使用 `@inject` 注入服务
- ✅ 使用 `@attribute [Authorize]` 控制权限
- ✅ 实现 `IAsyncDisposable` 释放资源
- ❌ 不要在Web层写业务逻辑（调用Application层服务）

---

## 4.6 项目引用关系

### 依赖关系图

```
┌─────────────────────────────────────┐
│         Minimes.Web                 │
│  (Blazor Server表示层)               │
└─────────────────────────────────────┘
         ↓ 引用
┌─────────────────────────────────────┐
│    Minimes.Infrastructure           │
│  (基础设施层)                        │
└─────────────────────────────────────┘
         ↓ 引用
┌─────────────────────────────────────┐
│    Minimes.Application              │
│  (应用层)                            │
└─────────────────────────────────────┘
         ↓ 引用
┌─────────────────────────────────────┐
│      Minimes.Domain                 │
│  (领域层 - 无依赖)                   │
└─────────────────────────────────────┘
```

### 项目文件引用配置

**Minimes.Web.csproj：**
```xml
<ItemGroup>
  <ProjectReference Include="..\Minimes.Domain\Minimes.Domain.csproj" />
  <ProjectReference Include="..\Minimes.Application\Minimes.Application.csproj" />
  <ProjectReference Include="..\Minimes.Infrastructure\Minimes.Infrastructure.csproj" />
</ItemGroup>
```

**Minimes.Infrastructure.csproj：**
```xml
<ItemGroup>
  <ProjectReference Include="..\Minimes.Domain\Minimes.Domain.csproj" />
  <ProjectReference Include="..\Minimes.Application\Minimes.Application.csproj" />
</ItemGroup>
```

**Minimes.Application.csproj：**
```xml
<ItemGroup>
  <ProjectReference Include="..\Minimes.Domain\Minimes.Domain.csproj" />
</ItemGroup>
```

**Minimes.Domain.csproj：**
```xml
<!-- 无项目引用 -->
```

### 为什么这么设计？

**1. 依赖倒置原则（DIP）：**
- Application层定义接口（`IWeighingRecordRepository`）
- Infrastructure层实现接口（`WeighingRecordRepository`）
- Web层通过DI容器注入实现

**2. 单向依赖：**
- 外层依赖内层，内层不依赖外层
- Domain层完全独立，可以单独测试
- Application层只依赖Domain层，不依赖Infrastructure层

**3. 可替换性：**
- 可以轻松替换Infrastructure层实现（SQLite → MySQL）
- 可以轻松替换Web层（Blazor → MVC → API）
- 业务逻辑（Application层）不受影响

**老王的总结：**

艹！Clean Architecture的核心就是：
- **Domain层**：核心业务，不依赖任何人
- **Application层**：业务流程，只依赖Domain层
- **Infrastructure层**：技术实现，依赖Domain和Application层
- **Web层**：用户界面，依赖所有层

这样设计的好处：
- ✅ 业务逻辑独立，易于测试
- ✅ 技术实现可替换，易于维护
- ✅ 层次清晰，职责分明
- ✅ 符合SOLID原则

---

**下一章：** [第五章：核心技术实战](#第五章核心技术实战)

---

# 第五章：核心技术实战

## 5.1 实战案例1：称重页面（WeighingPage.razor）

### 业务场景

称重页面是MiniMES的核心功能，业务流程如下：
1. 扫描或输入二维码（如：PORK-001）
2. 解析二维码，验证有效性
3. 输入重量（手动或电子秤自动读取）
4. 保存称重记录
5. 显示今日统计和最近记录

### 完整代码分析

**文件位置：** `src/Minimes.Web/Pages/Weighing/WeighingPage.razor:1`

#### 1. 页面指令和依赖注入

```razor
@page "/weighing"
@attribute [Authorize(Policy = "Operator")]
@using Microsoft.AspNetCore.SignalR.Client
@inject IWeighingRecordService WeighingRecordService
@inject IMeatTypeService MeatTypeService
@inject IQRCodeService QRCodeService
@inject NavigationManager Navigation
@inject AuthenticationStateProvider AuthenticationStateProvider
@inject IJSRuntime JSRuntime
@implements IAsyncDisposable
```

**关键点：**
- `@page "/weighing"` - 定义路由
- `@attribute [Authorize(Policy = "Operator")]` - 权限控制（操作员+管理员）
- `@inject` - 注入多个服务
- `@implements IAsyncDisposable` - 实现异步资源释放

#### 2. UI结构（响应式设计）

```razor
<div class="row">
    <!-- 左侧/移动端全宽：当前称重区域 -->
    <div class="col-12 col-md-8 mb-3">
        <div class="card">
            <!-- 条码输入 -->
            <input type="text" @bind="currentBarcode" @bind:event="oninput" />

            <!-- 重量输入 -->
            <input type="number" @bind="manualWeightInput" />

            <!-- 保存按钮 -->
            <button @onclick="SaveRecord">保存</button>
        </div>
    </div>

    <!-- 右侧/移动端：统计和最近记录 -->
    <div class="col-12 col-md-4">
        <!-- 今日统计 -->
        <div class="card">
            <p>记录数：@todaySummary?.TotalRecords</p>
            <p>总重量：@todaySummary?.TotalWeight lb</p>
        </div>
    </div>
</div>
```

**关键点：**
- `col-12 col-md-8` - 移动端全宽，桌面端8列
- `@bind:event="oninput"` - 实时绑定（每次输入都触发）
- `@onclick` - 点击事件处理

#### 3. 组件生命周期

```csharp
protected override async Task OnInitializedAsync()
{
    // 1. 创建SignalR连接对象
    hubConnection = new HubConnectionBuilder()
        .WithUrl(Navigation.ToAbsoluteUri("/hardwareHub"))
        .Build();

    // 2. 订阅扫码事件
    hubConnection.On<object>("ReceiveBarcode", (data) =>
    {
        currentBarcode = data.barcode;
        InvokeAsync(StateHasChanged);
    });

    // 3. 加载今日统计
    await LoadTodaySummary();

    // 4. 加载最近记录
    await LoadRecentRecords();
}

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // 聚焦到条码输入框
        await JSRuntime.InvokeVoidAsync("eval",
            "document.querySelector('input')?.focus()");

        // 后台启动SignalR连接
        await hubConnection.StartAsync();
    }
}
```

**关键点：**
- `OnInitializedAsync` - 加载数据，创建SignalR连接
- `OnAfterRenderAsync` - 首次渲染后聚焦输入框、启动SignalR
- `InvokeAsync(StateHasChanged)` - 在SignalR回调中更新UI

#### 4. 二维码解析逻辑

```csharp
private async Task ParseBarcode()
{
    try
    {
        var barcode = currentBarcode.Trim();

        // 1. 检查格式：必须包含"-"分隔符
        if (!barcode.Contains("-"))
        {
            errorMessage = "二维码格式错误！正确格式：PORK-001";
            return;
        }

        // 2. 检查二维码是否存在
        var qrCode = await QRCodeService.GetByContentAsync(barcode);
        if (qrCode == null)
        {
            errorMessage = "二维码不存在！";
            return;
        }

        // 3. 检查二维码是否激活
        if (!qrCode.IsActive)
        {
            errorMessage = "二维码已停用！";
            return;
        }

        // 4. 拆分二维码：PORK-001 → ["PORK", "001"]
        var parts = barcode.Split('-', 2);
        var meatTypeCode = parts[0].Trim().ToUpper();
        var code = parts[1].Trim();

        // 5. 查询肉类类型
        var meatType = await MeatTypeService.GetByCodeAsync(meatTypeCode);
        if (meatType == null)
        {
            errorMessage = $"肉类类型代码 '{meatTypeCode}' 不存在！";
            return;
        }

        // 6. 解析成功
        parsedCode = code;
        parsedMeatTypeId = meatType.Id;
        parsedMeatTypeName = meatType.Name;
        errorMessage = null;
    }
    catch (Exception ex)
    {
        errorMessage = $"解析二维码失败：{ex.Message}";
    }
}
```

**关键点：**
- 多层验证：格式 → 存在性 → 激活状态 → 肉类类型
- 友好的错误提示
- 异常处理

#### 5. 键盘快捷操作

```csharp
// 条码输入框回车：解析条码并跳转到重量输入框
private async Task OnBarcodeKeyDown(KeyboardEventArgs e)
{
    if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(currentBarcode))
    {
        await ParseBarcode();
        await FocusWeightInput();
    }
}

// 重量输入框回车：直接保存
private async Task OnWeightKeyDown(KeyboardEventArgs e)
{
    if (e.Key == "Enter" && manualWeightInput > 0)
    {
        await SaveRecord();
    }
}
```

**关键点：**
- 回车键快捷操作，提升效率
- 条码输入完成后自动跳转到重量输入
- 重量输入完成后直接保存

#### 6. 保存记录逻辑

```csharp
private async Task SaveRecord()
{
    // 1. 验证条码
    if (string.IsNullOrWhiteSpace(currentBarcode))
    {
        errorMessage = "条码不能为空";
        return;
    }

    // 2. 验证重量
    if (manualWeightInput <= 0)
    {
        errorMessage = "重量必须大于0";
        return;
    }

    // 3. 验证解析结果
    if (parsedMeatTypeId == 0 || string.IsNullOrWhiteSpace(parsedCode))
    {
        errorMessage = "请先输入有效的二维码并按回车解析！";
        return;
    }

    try
    {
        // 4. 获取当前登录用户
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var currentUser = authState.User.Identity?.Name ?? "Unknown";

        // 5. 创建请求
        var request = new CreateWeighingRecordRequest
        {
            Barcode = currentBarcode.Trim(),
            Code = parsedCode,
            MeatTypeId = parsedMeatTypeId,
            Weight = manualWeightInput,
            Remarks = remarks
        };

        // 6. 调用服务保存
        await WeighingRecordService.CreateAsync(request, currentUser);

        // 7. 刷新统计和记录
        await LoadTodaySummary();
        await LoadRecentRecords();

        // 8. 清空表单
        ClearForm();

        // 9. 聚焦回条码输入框
        await FocusBarcodeInput();
    }
    catch (Exception ex)
    {
        errorMessage = ex.Message;
    }
}
```

**关键点：**
- 多层验证：条码 → 重量 → 解析结果
- 获取当前登录用户
- 保存成功后刷新统计、清空表单、聚焦输入框
- 异常处理

**老王的经验总结：**
- ✅ 使用SignalR实时接收扫码数据
- ✅ 键盘快捷操作提升效率
- ✅ 多层验证确保数据正确性
- ✅ 响应式设计适配移动端
- ✅ 保存成功后自动准备下一次操作

---

## 5.2 实战案例2：SignalR实时推送（HardwareHub）

### 业务场景

硬件设备（电子秤、扫码枪）需要实时推送数据到前端页面：
- 电子秤实时推送重量数据
- 扫码枪实时推送条码数据
- 错误信息实时推送

### SignalR架构

```
硬件设备（电子秤/扫码枪）
  ↓ 事件触发
后台服务（HardwareBackgroundService）
  ↓ 调用Hub
SignalR Hub（HardwareHub）
  ↓ 推送消息
前端页面（WeighingPage.razor）
```

### 完整代码分析

#### 1. 定义Hub（HardwareHub.cs）

**文件位置：** `src/Minimes.Web/Hubs/HardwareHub.cs:1`

```csharp
public class HardwareHub : Hub
{
    /// <summary>
    /// 推送重量数据到所有客户端
    /// </summary>
    public async Task BroadcastWeight(decimal weight, string unit, bool isStable)
    {
        await Clients.All.SendAsync("ReceiveWeight", new
        {
            weight,
            unit,
            isStable,
            timestamp = DateTime.Now
        });
    }

    /// <summary>
    /// 推送扫码数据到所有客户端
    /// </summary>
    public async Task BroadcastBarcode(string barcode, string scannerType)
    {
        await Clients.All.SendAsync("ReceiveBarcode", new
        {
            barcode,
            scannerType,
            timestamp = DateTime.Now
        });
    }

    /// <summary>
    /// 推送错误信息到所有客户端
    /// </summary>
    public async Task BroadcastError(string errorMessage, string source)
    {
        await Clients.All.SendAsync("ReceiveError", new
        {
            errorMessage,
            source,
            timestamp = DateTime.Now
        });
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"客户端已连接: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"客户端已断开: {Context.ConnectionId}");
    }
}
```

**关键点：**
- `Clients.All.SendAsync` - 推送到所有客户端
- 匿名对象传递数据（自动序列化为JSON）
- 生命周期方法：`OnConnectedAsync`、`OnDisconnectedAsync`

#### 2. 注册Hub（Program.cs）

```csharp
// 注册SignalR服务
builder.Services.AddSignalR();

// 映射Hub端点
app.MapHub<HardwareHub>("/hardwareHub");
```

#### 3. 后台服务推送数据（HardwareBackgroundService.cs）

```csharp
public class HardwareBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<HardwareHub> _hubContext;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 获取硬件服务
        var scaleService = _serviceProvider.GetRequiredService<IScaleService>();

        // 订阅电子秤事件
        scaleService.OnWeightReceived += async (weight, unit, isStable) =>
        {
            // 推送到所有客户端
            await _hubContext.Clients.All.SendAsync("ReceiveWeight", new
            {
                weight,
                unit,
                isStable,
                timestamp = DateTime.Now
            }, stoppingToken);
        };

        // 启动电子秤
        await scaleService.StartAsync();

        // 保持运行
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}
```

**关键点：**
- 使用 `IHubContext<HardwareHub>` 在后台服务中推送消息
- 订阅硬件服务事件
- 使用 `CancellationToken` 优雅停止

#### 4. 前端订阅（WeighingPage.razor）

```csharp
private HubConnection? hubConnection;

protected override async Task OnInitializedAsync()
{
    // 创建SignalR连接
    hubConnection = new HubConnectionBuilder()
        .WithUrl(Navigation.ToAbsoluteUri("/hardwareHub"))
        .Build();

    // 订阅重量数据
    hubConnection.On<object>("ReceiveWeight", (data) =>
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        var weightData = System.Text.Json.JsonSerializer.Deserialize<WeightData>(json);

        // 更新UI
        currentWeight = weightData.weight;
        isStable = weightData.isStable;
        InvokeAsync(StateHasChanged);
    });

    // 订阅扫码数据
    hubConnection.On<object>("ReceiveBarcode", (data) =>
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        var barcodeData = System.Text.Json.JsonSerializer.Deserialize<BarcodeData>(json);

        // 更新UI
        currentBarcode = barcodeData.barcode;
        InvokeAsync(StateHasChanged);
    });

    // 启动连接
    await hubConnection.StartAsync();
}

public async ValueTask DisposeAsync()
{
    if (hubConnection != null)
    {
        await hubConnection.DisposeAsync();
    }
}
```

**关键点：**
- `HubConnectionBuilder` - 创建连接
- `hubConnection.On<T>` - 订阅消息
- `InvokeAsync(StateHasChanged)` - 在SignalR回调中更新UI
- `DisposeAsync` - 释放连接

**老王的经验总结：**
- ✅ SignalR适合实时性要求高的场景
- ✅ 使用 `IHubContext` 在后台服务中推送消息
- ✅ 前端记得释放连接（避免内存泄漏）
- ✅ 使用 `InvokeAsync(StateHasChanged)` 更新UI
- ❌ 不要在SignalR中传输大量数据

---

## 5.3 实战案例3：布局和导航（MainLayout.razor）

### 业务场景

MainLayout是所有页面的公共布局，包含：
- 侧边栏导航菜单
- 顶部用户信息栏
- 语言切换
- 演示模式标识

### 完整代码分析

**文件位置：** `src/Minimes.Web/Shared/MainLayout.razor:1`

#### 1. 布局结构

```razor
@inherits LayoutComponentBase
@inject IStringLocalizer<SharedResource> L

<div class="page">
    <!-- 侧边栏 -->
    <div class="sidebar">
        <NavMenu />
    </div>

    <!-- 主内容区 -->
    <main>
        <!-- 顶部栏 -->
        <div class="top-row px-4 auth">
            <!-- 语言切换 -->
            <div class="d-flex align-items-center me-auto">
                <CultureSelector />
            </div>

            <!-- 用户信息 -->
            <AuthorizeView>
                <Authorized>
                    <div class="d-flex align-items-center">
                        <!-- 当前时间 -->
                        <span class="me-3 text-muted">
                            <span class="oi oi-clock me-1"></span>
                            @DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                        </span>

                        <!-- 用户名 -->
                        <span class="me-3">
                            <span class="oi oi-person me-1"></span>
                            <strong>@context.User.Identity?.Name</strong>
                        </span>

                        <!-- 角色徽章 -->
                        @if (context.User.IsInRole("Administrator"))
                        {
                            <span class="badge bg-danger">@L["Role_Admin"]</span>
                        }
                        else
                        {
                            <span class="badge bg-info">@L["Role_Operator"]</span>
                        }

                        <!-- 演示模式标识 -->
                        @if (context.User.FindFirst("IsDemoMode")?.Value == "True")
                        {
                            <span class="badge bg-warning text-dark ms-2">
                                <span class="oi oi-eye me-1"></span>@L["DemoMode"]
                            </span>
                        }
                    </div>
                </Authorized>
                <NotAuthorized>
                    <span class="text-muted">@L["Role_NotLoggedIn"]</span>
                </NotAuthorized>
            </AuthorizeView>
        </div>

        <!-- 页面内容 -->
        <article class="content px-4">
            @Body
        </article>
    </main>
</div>
```

**关键点：**
- `@inherits LayoutComponentBase` - 继承布局基类
- `@Body` - 页面内容占位符
- `<AuthorizeView>` - 根据认证状态显示不同内容
- `context.User` - 访问当前用户信息

#### 2. 导航菜单（NavMenu.razor）

```razor
<div class="nav-item px-3">
    <NavLink class="nav-link" href="/" Match="NavLinkMatch.All">
        <span class="oi oi-home" aria-hidden="true"></span> @L["Nav_Home"]
    </NavLink>
</div>

<!-- 生产管理（操作员+管理员） -->
<AuthorizeView Policy="Operator">
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="/weighing">
            <span class="oi oi-scale" aria-hidden="true"></span> @L["Nav_Weighing"]
        </NavLink>
    </div>
</AuthorizeView>

<!-- 用户管理（仅管理员） -->
<AuthorizeView Policy="Admin">
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="/users">
            <span class="oi oi-people" aria-hidden="true"></span> @L["Nav_Users"]
        </NavLink>
    </div>
</AuthorizeView>
```

**关键点：**
- `<NavLink>` - 自动高亮当前页面
- `Match="NavLinkMatch.All"` - 精确匹配（首页专用）
- `<AuthorizeView Policy="Admin">` - 根据策略显示菜单

**老王的经验总结：**
- ✅ 使用 `LayoutComponentBase` 创建布局
- ✅ 使用 `<AuthorizeView>` 控制菜单显示
- ✅ 使用 `<NavLink>` 自动高亮当前页面
- ✅ 国际化所有文本（`@L["Key"]`）
- ✅ 显示用户信息和角色徽章

---

**下一节：** [5.4 实战案例4：认证授权流程](#54-实战案例4认证授权流程)

---

## 5.4 实战案例4：认证授权流程

### 业务场景

MiniMES使用Cookie认证，支持：
- 用户登录（用户名+密码）
- 演示账户登录（demo/demo123）
- 角色权限控制（Administrator、Operator）
- 记住我功能

### 认证流程图

```
用户访问 /weighing
  ↓
检查Cookie（未登录）
  ↓
重定向到 /Account/Login
  ↓
用户输入用户名密码
  ↓
验证用户名密码
  ↓
创建Claims（用户名、角色、UserId等）
  ↓
生成Cookie
  ↓
重定向到 /weighing
  ↓
检查Cookie（已登录）
  ↓
检查权限（Operator策略）
  ↓
显示页面
```

### 完整代码分析

#### 1. 认证配置（AuthenticationExtensions.cs）

```csharp
public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthentication(
        this IServiceCollection services)
    {
        // Cookie认证
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

        // 授权策略
        services.AddAuthorization(options =>
        {
            // Admin策略 - 只有Administrator角色可以访问
            options.AddPolicy("Admin", policy =>
                policy.RequireRole("Administrator"));

            // Operator策略 - Operator或Administrator都可以访问
            options.AddPolicy("Operator", policy =>
                policy.RequireRole("Operator", "Administrator"));

            // Authenticated策略 - 任何已认证用户都可以访问
            options.AddPolicy("Authenticated", policy =>
                policy.RequireAuthenticatedUser());
        });

        return services;
    }
}
```

**关键点：**
- `ExpireTimeSpan` - Cookie过期时间（8小时）
- `SlidingExpiration` - 滑动过期（活跃用户自动续期）
- `HttpOnly` - 防止JavaScript访问Cookie（安全）
- 三种授权策略：Admin、Operator、Authenticated

#### 2. 登录页面（Account/Login.cshtml.cs）

```csharp
public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IScaleService _scaleService;

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        // 1. 验证用户名密码
        var user = await _userService.AuthenticateAsync(Username, Password);
        if (user == null)
        {
            ErrorMessage = "用户名或密码错误";
            return Page();
        }

        // 2. 检查用户是否激活
        if (!user.IsActive)
        {
            ErrorMessage = "用户已被停用";
            return Page();
        }

        // 3. 创建Claims（声明）
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("UserId", user.Id.ToString()),
            new Claim("DisplayName", user.DisplayName ?? user.UserName)
        };

        // 4. 演示模式标识
        if (user.UserName.Equals("demo", StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim("IsDemoMode", "True"));
            // 启用硬件模拟模式
            _scaleService.SetDemoMode(true);
        }

        // 5. 创建身份标识
        var identity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // 6. 登录（生成Cookie）
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        // 7. 重定向到返回URL
        return LocalRedirect(returnUrl);
    }
}
```

**关键点：**
- `AuthenticateAsync` - 验证用户名密码
- `Claims` - 存储用户信息（用户名、角色、UserId等）
- `IsPersistent` - 记住我（关闭浏览器后Cookie仍有效）
- 演示模式：检测demo账户，启用硬件模拟

#### 3. 页面权限控制

```razor
<!-- 管理员专用页面 -->
@page "/users"
@attribute [Authorize(Policy = "Admin")]

<!-- 操作员+管理员页面 -->
@page "/weighing"
@attribute [Authorize(Policy = "Operator")]

<!-- 所有已登录用户页面 -->
@page "/profile"
@attribute [Authorize]
```

#### 4. 菜单权限控制

```razor
<AuthorizeView Policy="Admin">
    <Authorized>
        <div class="nav-item">
            <NavLink href="/users">用户管理</NavLink>
        </div>
    </Authorized>
</AuthorizeView>

<AuthorizeView>
    <Authorized>
        <span>欢迎，@context.User.Identity?.Name</span>
        <a href="/Account/Logout">退出</a>
    </Authorized>
    <NotAuthorized>
        <a href="/Account/Login">登录</a>
    </NotAuthorized>
</AuthorizeView>
```

#### 5. 代码中获取当前用户

```csharp
@inject AuthenticationStateProvider AuthenticationStateProvider

@code {
    private async Task GetCurrentUser()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var userName = user.Identity.Name;
            var userId = user.FindFirst("UserId")?.Value;
            var isAdmin = user.IsInRole("Administrator");
        }
    }
}
```

**老王的经验总结：**
- ✅ 使用Cookie认证（简单、适合内网）
- ✅ Claims存储用户信息（用户名、角色、自定义字段）
- ✅ 三层防护：页面权限 + 菜单控制 + Service验证
- ✅ 演示模式：检测demo账户，启用硬件模拟
- ❌ 不要把密码明文存储在数据库（使用哈希）

---

## 5.5 实战案例5：国际化（i18n）实现

### 业务场景

MiniMES支持中英文切换：
- 默认语言：英文（en-US）
- 支持语言：中文（zh-CN）、英文（en-US）
- 语言切换：顶部下拉菜单
- 持久化：Cookie存储

### 国际化架构

```
资源文件（.resx）
  ↓
IStringLocalizer<SharedResource>
  ↓
Blazor组件（@L["Key"]）
  ↓
显示对应语言的文本
```

### 完整代码分析

#### 1. 资源文件结构

```
Minimes.Application/Resources/
├── SharedResource.zh-CN.resx    # 中文资源
└── SharedResource.en-US.resx    # 英文资源
```

**SharedResource.zh-CN.resx（中文）：**
```xml
<data name="AppName" xml:space="preserve">
  <value>MiniMES</value>
</data>
<data name="AppTitle" xml:space="preserve">
  <value>记账系统</value>
</data>
<data name="Nav_Home" xml:space="preserve">
  <value>首页</value>
</data>
<data name="Nav_Weighing" xml:space="preserve">
  <value>生产称重</value>
</data>
<data name="Weighing_Title" xml:space="preserve">
  <value>生产称重</value>
</data>
<data name="Weighing_Barcode" xml:space="preserve">
  <value>二维码</value>
</data>
```

**SharedResource.en-US.resx（英文）：**
```xml
<data name="AppName" xml:space="preserve">
  <value>MiniMES</value>
</data>
<data name="AppTitle" xml:space="preserve">
  <value>Accounting System</value>
</data>
<data name="Nav_Home" xml:space="preserve">
  <value>Home</value>
</data>
<data name="Nav_Weighing" xml:space="preserve">
  <value>Weighing</value>
</data>
<data name="Weighing_Title" xml:space="preserve">
  <value>Production Weighing</value>
</data>
<data name="Weighing_Barcode" xml:space="preserve">
  <value>Barcode</value>
</data>
```

#### 2. 国际化配置（Program.cs）

```csharp
// 注册国际化服务
builder.Services.AddLocalization();
builder.Services.AddControllers(); // CultureController需要

var app = builder.Build();

// 配置支持的语言
var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("zh-CN")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

// 清空默认Provider，只用Cookie（避免浏览器语言干扰）
localizationOptions.RequestCultureProviders.Clear();
localizationOptions.RequestCultureProviders.Add(new CookieRequestCultureProvider());

app.UseRequestLocalization(localizationOptions);
```

**关键点：**
- `DefaultRequestCulture` - 默认语言（英文）
- `SupportedCultures` - 支持的语言列表
- `CookieRequestCultureProvider` - 使用Cookie存储语言选择

#### 3. 语言切换组件（CultureSelector.razor）

```razor
@inject NavigationManager Navigation
@inject IJSRuntime JSRuntime

<div class="dropdown">
    <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button"
            data-bs-toggle="dropdown">
        <span class="oi oi-globe"></span> @GetCurrentCultureDisplay()
    </button>
    <ul class="dropdown-menu">
        <li>
            <a class="dropdown-item" href="#" @onclick="() => SetCulture(\"en-US\")">
                English
            </a>
        </li>
        <li>
            <a class="dropdown-item" href="#" @onclick="() => SetCulture(\"zh-CN\")">
                中文
            </a>
        </li>
    </ul>
</div>

@code {
    private string GetCurrentCultureDisplay()
    {
        var culture = CultureInfo.CurrentUICulture.Name;
        return culture switch
        {
            "zh-CN" => "中文",
            "en-US" => "English",
            _ => "English"
        };
    }

    private async Task SetCulture(string culture)
    {
        // 设置Cookie
        await JSRuntime.InvokeVoidAsync("eval",
            $"document.cookie = '.AspNetCore.Culture=c={culture}|uic={culture}; path=/; max-age=31536000'");

        // 刷新页面
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }
}
```

**关键点：**
- `CultureInfo.CurrentUICulture` - 获取当前语言
- 设置Cookie：`.AspNetCore.Culture=c={culture}|uic={culture}`
- `forceLoad: true` - 强制刷新页面（应用新语言）

#### 4. 在Blazor组件中使用

```razor
@inject IStringLocalizer<SharedResource> L

<h3>@L["Weighing_Title"]</h3>

<label>@L["Weighing_Barcode"]</label>
<input type="text" placeholder="@L["Weighing_BarcodePlaceholder"]" />

<button>@L["Btn_Save"]</button>

@code {
    private string errorMessage = string.Empty;

    private void ShowError()
    {
        errorMessage = L["Error_BarcodeRequired"];
    }
}
```

**关键点：**
- `@inject IStringLocalizer<SharedResource>` - 注入本地化服务
- `@L["Key"]` - 获取本地化文本
- 支持插值：`L["Error_WeightRange", minWeight, maxWeight]`

#### 5. 在C#代码中使用

```csharp
public class WeighingRecordService : IWeighingRecordService
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public WeighingRecordService(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    public async Task<WeighingRecordResponse> CreateAsync(
        CreateWeighingRecordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Barcode))
        {
            throw new ValidationException(_localizer["Error_BarcodeRequired"]);
        }

        // 业务逻辑...
    }
}
```

**老王的经验总结：**
- ✅ 使用 `.resx` 资源文件管理多语言文本
- ✅ 资源键命名规范：`模块_功能_类型`（如：`Weighing_Barcode`）
- ✅ 使用Cookie存储语言选择（持久化）
- ✅ 前后端共享资源文件（Application层）
- ✅ 所有用户可见文本必须国际化
- ❌ 不要硬编码中文或英文字符串

---

**下一章：** [第六章：最佳实践和开发规范](#第六章最佳实践和开发规范)

---

# 第六章：最佳实践和开发规范

## 6.1 SOLID原则在项目中的应用

### SOLID原则概述

| 原则 | 英文 | 核心思想 |
|-----|------|---------|
| **S** | Single Responsibility | 单一职责：一个类只负责一件事 |
| **O** | Open/Closed | 开闭原则：对扩展开放，对修改关闭 |
| **L** | Liskov Substitution | 里氏替换：子类可以替换父类 |
| **I** | Interface Segregation | 接口隔离：接口专一，不要胖接口 |
| **D** | Dependency Inversion | 依赖倒置：依赖抽象，不依赖具体 |

### MiniMES中的SOLID实践

#### 1. 单一职责原则（SRP）

**反例（违反SRP）：**
```csharp
// ❌ 一个类做了太多事情
public class WeighingRecordService
{
    public async Task CreateAsync(CreateWeighingRecordRequest request)
    {
        // 验证数据
        if (string.IsNullOrEmpty(request.Barcode)) throw new Exception("条码不能为空");

        // 保存到数据库
        var record = new WeighingRecord { Barcode = request.Barcode };
        _context.WeighingRecords.Add(record);
        await _context.SaveChangesAsync();

        // 导出Excel
        var excelBytes = GenerateExcel(record);

        // 发送邮件
        SendEmail(excelBytes);
    }
}
```

**正例（遵守SRP）：**
```csharp
// ✅ 每个类只负责一件事
public class WeighingRecordService
{
    private readonly IWeighingRecordRepository _repository;
    private readonly IValidator<CreateWeighingRecordRequest> _validator;

    public async Task CreateAsync(CreateWeighingRecordRequest request)
    {
        // 验证（委托给验证器）
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        // 保存（委托给仓储）
        var record = _mapper.Map<WeighingRecord>(request);
        await _repository.AddAsync(record);
    }
}

// Excel导出服务（单独的类）
public class ExcelExportService
{
    public byte[] ExportWeighingRecords(List<WeighingRecord> records) { }
}

// 邮件服务（单独的类）
public class EmailService
{
    public async Task SendEmailAsync(string to, byte[] attachment) { }
}
```

**MiniMES实践：**
- `WeighingRecordService` - 只负责业务逻辑
- `WeighingRecordRepository` - 只负责数据访问
- `CreateWeighingRecordValidator` - 只负责验证
- `ExcelExportService` - 只负责Excel导出

#### 2. 开闭原则（OCP）

**反例（违反OCP）：**
```csharp
// ❌ 每次添加新协议都要修改这个类
public class ScaleService
{
    public decimal ParseWeight(string data, string protocol)
    {
        if (protocol == "Toledo")
        {
            return ParseToledoProtocol(data);
        }
        else if (protocol == "Mettler")
        {
            return ParseMettlerProtocol(data);
        }
        else if (protocol == "Generic")
        {
            return ParseGenericProtocol(data);
        }
        throw new NotSupportedException($"不支持的协议：{protocol}");
    }
}
```

**正例（遵守OCP）：**
```csharp
// ✅ 通过接口扩展，不修改现有代码
public interface IWeightProtocolParser
{
    decimal Parse(string data);
}

public class ToledoProtocolParser : IWeightProtocolParser
{
    public decimal Parse(string data) { /* Toledo协议解析 */ }
}

public class MettlerProtocolParser : IWeightProtocolParser
{
    public decimal Parse(string data) { /* Mettler协议解析 */ }
}

public class ScaleService
{
    private readonly IWeightProtocolParser _parser;

    public ScaleService(IWeightProtocolParser parser)
    {
        _parser = parser;
    }

    public decimal ParseWeight(string data)
    {
        return _parser.Parse(data);
    }
}
```

**MiniMES实践：**
- 数据库切换：通过配置切换SQLite/MySQL，不修改代码
- 硬件切换：通过接口 `IScaleService`，支持串口/WiFi电子秤

#### 3. 里氏替换原则（LSP）

**反例（违反LSP）：**
```csharp
// ❌ 子类改变了父类的行为
public class Repository<T>
{
    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
}

public class WeighingRecordRepository : Repository<WeighingRecord>
{
    public override async Task<WeighingRecord> GetByIdAsync(int id)
    {
        // 子类抛出异常，违反了父类的契约
        throw new NotImplementedException("请使用GetByIdWithMeatTypeAsync");
    }
}
```

**正例（遵守LSP）：**
```csharp
// ✅ 子类可以安全替换父类
public class Repository<T>
{
    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
}

public class WeighingRecordRepository : Repository<WeighingRecord>
{
    // 保留父类方法的行为
    public override async Task<WeighingRecord> GetByIdAsync(int id)
    {
        return await _context.WeighingRecords.FindAsync(id);
    }

    // 添加新方法，不破坏父类契约
    public async Task<WeighingRecord> GetByIdWithMeatTypeAsync(int id)
    {
        return await _context.WeighingRecords
            .Include(x => x.MeatType)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
```

#### 4. 接口隔离原则（ISP）

**反例（违反ISP）：**
```csharp
// ❌ 胖接口，强迫实现不需要的方法
public interface IRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<List<T>> SearchAsync(string keyword);
    Task<List<T>> GetPagedAsync(int page, int size);
    Task<int> CountAsync();
    Task<bool> ExistsAsync(int id);
}
```

**正例（遵守ISP）：**
```csharp
// ✅ 接口专一，按需实现
public interface IReadRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
}

public interface IWriteRepository<T>
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public interface ISearchRepository<T>
{
    Task<List<T>> SearchAsync(string keyword);
}

// 根据需要组合接口
public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>
{
}
```

**MiniMES实践：**
- `IWeighingRecordRepository` - 只定义称重记录需要的方法
- `IUserRepository` - 只定义用户管理需要的方法
- 不强迫所有仓储实现相同的方法

#### 5. 依赖倒置原则（DIP）

**反例（违反DIP）：**
```csharp
// ❌ 直接依赖具体实现
public class WeighingRecordService
{
    private readonly WeighingRecordRepository _repository;

    public WeighingRecordService()
    {
        _repository = new WeighingRecordRepository();
    }
}
```

**正例（遵守DIP）：**
```csharp
// ✅ 依赖抽象（接口）
public class WeighingRecordService
{
    private readonly IWeighingRecordRepository _repository;

    public WeighingRecordService(IWeighingRecordRepository repository)
    {
        _repository = repository;
    }
}

// 在Program.cs中注册
builder.Services.AddScoped<IWeighingRecordRepository, WeighingRecordRepository>();
```

**MiniMES实践：**
- Application层定义接口（`IWeighingRecordRepository`）
- Infrastructure层实现接口（`WeighingRecordRepository`）
- Web层通过DI容器注入实现

**老王的总结：**
- ✅ SOLID原则让代码更易维护、测试、扩展
- ✅ MiniMES严格遵守SOLID原则
- ✅ 重构时优先考虑SOLID原则
- ❌ 不要过度设计，根据实际需求应用

---

## 6.2 KISS、DRY、YAGNI原则

### KISS原则（Keep It Simple, Stupid）

**核心思想：** 保持简单，避免过度复杂

**反例（过度复杂）：**
```csharp
// ❌ 过度设计，使用了不必要的设计模式
public interface IWeightCalculatorFactory
{
    IWeightCalculator CreateCalculator(WeightUnit unit);
}

public interface IWeightCalculator
{
    decimal Calculate(decimal value);
}

public class PoundWeightCalculator : IWeightCalculator
{
    public decimal Calculate(decimal value) => value;
}

public class KilogramWeightCalculator : IWeightCalculator
{
    public decimal Calculate(decimal value) => value * 2.20462m;
}

// 使用时需要3个类
var factory = new WeightCalculatorFactory();
var calculator = factory.CreateCalculator(WeightUnit.Pound);
var result = calculator.Calculate(weight);
```

**正例（简单直接）：**
```csharp
// ✅ 简单直接，一个方法搞定
public static class WeightConverter
{
    public static decimal ToPounds(decimal value, WeightUnit unit)
    {
        return unit switch
        {
            WeightUnit.Pound => value,
            WeightUnit.Kilogram => value * 2.20462m,
            _ => throw new ArgumentException($"不支持的单位：{unit}")
        };
    }
}

// 使用时只需要一行
var result = WeightConverter.ToPounds(weight, WeightUnit.Kilogram);
```

**MiniMES实践：**
- 简单的逻辑不使用设计模式
- 优先使用静态方法、扩展方法
- 避免不必要的抽象层

### DRY原则（Don't Repeat Yourself）

**核心思想：** 不要重复自己，代码复用

**反例（重复代码）：**
```csharp
// ❌ 重复的验证逻辑
public async Task CreateUserAsync(CreateUserRequest request)
{
    if (string.IsNullOrWhiteSpace(request.UserName))
        throw new ValidationException("用户名不能为空");
    if (request.UserName.Length > 50)
        throw new ValidationException("用户名长度不能超过50");
    // 保存用户...
}

public async Task UpdateUserAsync(UpdateUserRequest request)
{
    if (string.IsNullOrWhiteSpace(request.UserName))
        throw new ValidationException("用户名不能为空");
    if (request.UserName.Length > 50)
        throw new ValidationException("用户名长度不能超过50");
    // 更新用户...
}
```

**正例（复用验证逻辑）：**
```csharp
// ✅ 使用FluentValidation统一验证
public class UserValidator : AbstractValidator<UserRequest>
{
    public UserValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名长度不能超过50");
    }
}

public async Task CreateUserAsync(CreateUserRequest request)
{
    await _validator.ValidateAndThrowAsync(request);
    // 保存用户...
}

public async Task UpdateUserAsync(UpdateUserRequest request)
{
    await _validator.ValidateAndThrowAsync(request);
    // 更新用户...
}
```

**MiniMES实践：**
- 使用FluentValidation统一验证逻辑
- 使用AutoMapper统一映射逻辑
- 使用基类Repository统一数据访问逻辑
- 使用共享组件（NavMenu、CultureSelector）

### YAGNI原则（You Aren't Gonna Need It）

**核心思想：** 只实现当前需要的功能，不预留未来功能

**反例（过度设计）：**
```csharp
// ❌ 预留了大量未来可能用到的功能
public class WeighingRecord
{
    public int Id { get; set; }
    public string Barcode { get; set; }
    public decimal Weight { get; set; }

    // 以下字段当前不需要，但"可能"未来会用到
    public string? Location { get; set; }
    public string? Operator { get; set; }
    public string? Supervisor { get; set; }
    public string? QualityInspector { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Humidity { get; set; }
    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? StorageLocation { get; set; }
}
```

**正例（只实现当前需要的）：**
```csharp
// ✅ 只包含当前需要的字段
public class WeighingRecord
{
    public int Id { get; set; }
    public string Barcode { get; set; }
    public string Code { get; set; }
    public int MeatTypeId { get; set; }
    public decimal WeightInPounds { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
}

// 未来需要时再添加字段
```

**MiniMES实践：**
- 删除了工序模块（当前不需要）
- 简化了客户管理（只保留必要字段）
- 没有实现复杂的权限系统（只有两个角色）

**老王的总结：**
- ✅ KISS：简单就是美，不要过度设计
- ✅ DRY：复用代码，避免重复
- ✅ YAGNI：只实现当前需要的功能
- ❌ 不要为了"可能"的需求增加复杂度

---

## 6.3 性能优化技巧

### 1. 数据库查询优化

**问题：加载所有数据到内存再过滤**
```csharp
// ❌ 性能差：加载所有记录到内存，然后过滤
public async Task<List<WeighingRecord>> GetTodayRecordsAsync()
{
    var allRecords = await _repository.GetAllAsync();
    var today = DateTime.Today;
    return allRecords.Where(x => x.CreatedAt >= today).ToList();
}
```

**优化：数据库层面过滤**
```csharp
// ✅ 性能好：在数据库层面过滤
public async Task<List<WeighingRecord>> GetTodayRecordsAsync()
{
    var today = DateTime.Today;
    return await _context.WeighingRecords
        .Where(x => x.CreatedAt >= today)
        .ToListAsync();
}
```

**MiniMES实践：**
- `QueryPagedAsync` - 数据库层面分页和过滤
- `GetTodayStatisticsAsync` - 使用 `CountAsync`、`SumAsync` 聚合查询
- 避免使用 `GetAllAsync().Where()`

### 2. 避免N+1查询问题

**问题：N+1查询**
```csharp
// ❌ N+1查询：查询1次记录 + N次肉类类型
var records = await _context.WeighingRecords.ToListAsync();
foreach (var record in records)
{
    var meatType = await _context.MeatTypes.FindAsync(record.MeatTypeId);
    Console.WriteLine($"{record.Barcode} - {meatType.Name}");
}
```

**优化：使用Include预加载**
```csharp
// ✅ 一次查询：使用Include预加载关联数据
var records = await _context.WeighingRecords
    .Include(x => x.MeatType)
    .ToListAsync();

foreach (var record in records)
{
    Console.WriteLine($"{record.Barcode} - {record.MeatType.Name}");
}
```

**MiniMES实践：**
- 所有查询都使用 `Include` 预加载关联数据
- 避免在循环中查询数据库

### 3. 异步操作优化

**问题：同步阻塞**
```csharp
// ❌ 同步阻塞：.Result会导致死锁
public void LoadData()
{
    var records = _repository.GetAllAsync().Result;
    var summary = _service.GetTodaySummaryAsync().Result;
}
```

**优化：使用async/await**
```csharp
// ✅ 异步非阻塞
public async Task LoadDataAsync()
{
    var records = await _repository.GetAllAsync();
    var summary = await _service.GetTodaySummaryAsync();
}
```

**MiniMES实践：**
- 所有数据库操作都使用 `async/await`
- 避免使用 `.Result` 或 `.Wait()`

### 4. SignalR连接优化

**问题：阻塞页面加载**
```csharp
// ❌ 阻塞页面加载：等待SignalR连接完成
protected override async Task OnInitializedAsync()
{
    hubConnection = new HubConnectionBuilder()
        .WithUrl(Navigation.ToAbsoluteUri("/hardwareHub"))
        .Build();

    await hubConnection.StartAsync(); // 阻塞这里

    await LoadTodaySummary();
    await LoadRecentRecords();
}
```

**优化：后台启动连接**
```csharp
// ✅ 后台启动连接：不阻塞数据加载
protected override async Task OnInitializedAsync()
{
    // 创建连接对象（不启动）
    hubConnection = new HubConnectionBuilder()
        .WithUrl(Navigation.ToAbsoluteUri("/hardwareHub"))
        .Build();

    // 先加载数据
    await LoadTodaySummary();
    await LoadRecentRecords();
}

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // 后台启动连接
        _ = Task.Run(async () => await hubConnection.StartAsync());
    }
}
```

**MiniMES实践：**
- SignalR连接在 `OnAfterRenderAsync` 中后台启动
- 页面加载时间从 0.6s 降低到 0.05s

### 5. 减少不必要的StateHasChanged调用

**问题：频繁刷新UI**
```csharp
// ❌ 频繁刷新UI
private async Task LoadData()
{
    StateHasChanged(); // 不必要
    var records = await _repository.GetAllAsync();
    StateHasChanged(); // 不必要
    var summary = await _service.GetTodaySummaryAsync();
    StateHasChanged(); // 必要
}
```

**优化：只在必要时刷新**
```csharp
// ✅ 只在数据加载完成后刷新一次
private async Task LoadData()
{
    var records = await _repository.GetAllAsync();
    var summary = await _service.GetTodaySummaryAsync();
    StateHasChanged(); // 只刷新一次
}
```

**MiniMES实践：**
- 只在SignalR回调中调用 `StateHasChanged`
- 避免在每个await后调用 `StateHasChanged`

**老王的总结：**
- ✅ 数据库层面过滤和聚合
- ✅ 使用Include预加载关联数据
- ✅ 使用async/await，避免同步阻塞
- ✅ SignalR连接后台启动
- ✅ 减少不必要的UI刷新

---

**下一节：** [6.4 常见问题和解决方案](#64-常见问题和解决方案)

---

## 6.4 常见问题和解决方案

### 问题1：Blazor组件不刷新

**症状：** 修改了变量的值，但UI没有更新

**原因：** Blazor不知道数据变化了

**解决方案：**
```csharp
// 方案1：在SignalR回调中使用InvokeAsync
hubConnection.On<object>("ReceiveWeight", (data) =>
{
    currentWeight = data.weight;
    InvokeAsync(StateHasChanged); // 通知Blazor刷新UI
});

// 方案2：在异步方法中自动刷新
private async Task LoadData()
{
    data = await _service.GetDataAsync();
    // 异步方法结束后自动刷新，不需要手动调用StateHasChanged
}
```

### 问题2：DbContext并发错误

**症状：** `A second operation started on this context before a previous operation completed`

**原因：** 同时执行多个数据库操作

**解决方案：**
```csharp
// ❌ 错误：并发执行
protected override async Task OnInitializedAsync()
{
    var task1 = LoadTodaySummary();
    var task2 = LoadRecentRecords();
    await Task.WhenAll(task1, task2); // 并发执行，会报错
}

// ✅ 正确：顺序执行
protected override async Task OnInitializedAsync()
{
    await LoadTodaySummary();
    await LoadRecentRecords();
}
```

### 问题3：SignalR连接失败

**症状：** `Failed to start the connection: Error: WebSocket failed to connect`

**原因：**
1. Hub端点未映射
2. 防火墙阻止WebSocket
3. HTTPS配置问题

**解决方案：**
```csharp
// 1. 确保Hub端点已映射（Program.cs）
app.MapHub<HardwareHub>("/hardwareHub");

// 2. 配置SignalR回退传输
hubConnection = new HubConnectionBuilder()
    .WithUrl(Navigation.ToAbsoluteUri("/hardwareHub"), options =>
    {
        options.Transports = HttpTransportType.WebSockets |
                            HttpTransportType.ServerSentEvents |
                            HttpTransportType.LongPolling;
    })
    .WithAutomaticReconnect() // 自动重连
    .Build();

// 3. 添加错误处理
try
{
    await hubConnection.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"SignalR连接失败：{ex.Message}");
}
```

### 问题4：依赖注入生命周期错误

**症状：** `Cannot resolve scoped service from root provider`

**原因：** 在Singleton服务中注入Scoped服务

**解决方案：**
```csharp
// ❌ 错误：Singleton服务注入Scoped服务
public class HardwareBackgroundService : BackgroundService
{
    private readonly ApplicationDbContext _context; // Scoped

    public HardwareBackgroundService(ApplicationDbContext context)
    {
        _context = context; // 报错！
    }
}

// ✅ 正确：使用IServiceProvider创建Scope
public class HardwareBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public HardwareBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // 使用context...
    }
}
```

### 问题5：EF Core迁移失败

**症状：** `Build failed. Use dotnet build to see the errors.`

**原因：** 项目编译失败

**解决方案：**
```bash
# 1. 先确保项目能编译
dotnet build

# 2. 指定启动项目（Infrastructure项目没有Program.cs）
cd src/Minimes.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Minimes.Web

# 3. 应用迁移
dotnet ef database update --startup-project ../Minimes.Web
```

### 问题6：国际化不生效

**症状：** 页面显示资源键（如：`Weighing_Title`）而不是文本

**原因：**
1. 资源文件未正确配置
2. 未注入IStringLocalizer

**解决方案：**
```csharp
// 1. 确保注册了国际化服务（Program.cs）
builder.Services.AddLocalization();

// 2. 确保注入了IStringLocalizer
@inject IStringLocalizer<SharedResource> L

// 3. 确保资源文件在正确的位置
// Minimes.Application/Resources/SharedResource.zh-CN.resx

// 4. 确保资源文件的Build Action为"Embedded resource"
```

### 问题7：Cookie认证不持久

**症状：** 关闭浏览器后需要重新登录

**原因：** 未设置IsPersistent

**解决方案：**
```csharp
await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme,
    principal,
    new AuthenticationProperties
    {
        IsPersistent = true, // 持久化Cookie
        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
    });
```

### 问题8：JavaScript互操作失败

**症状：** `There was an exception invoking 'eval'`

**原因：** 在DOM渲染前调用JS

**解决方案：**
```csharp
// ❌ 错误：在OnInitialized中调用JS
protected override async Task OnInitializedAsync()
{
    await JSRuntime.InvokeVoidAsync("eval", "document.querySelector('input')?.focus()");
}

// ✅ 正确：在OnAfterRender中调用JS
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await JSRuntime.InvokeVoidAsync("eval", "document.querySelector('input')?.focus()");
    }
}
```

**老王的经验总结：**
- ✅ SignalR回调中使用 `InvokeAsync(StateHasChanged)`
- ✅ 避免DbContext并发操作
- ✅ Singleton服务使用 `IServiceProvider` 创建Scope
- ✅ JS互操作在 `OnAfterRenderAsync` 中执行
- ✅ 遇到问题先看控制台错误信息

---

## 6.5 调试技巧

### 1. 使用浏览器开发者工具

**F12打开开发者工具：**
- **Console（控制台）**：查看JavaScript错误、Console.WriteLine输出
- **Network（网络）**：查看SignalR连接状态、API请求
- **Application（应用）**：查看Cookie、LocalStorage

**查看SignalR连接：**
```
Network → WS（WebSocket） → hardwareHub
```

### 2. 使用Visual Studio调试

**断点调试：**
```csharp
protected override async Task OnInitializedAsync()
{
    // F9设置断点
    var summary = await WeighingRecordService.GetTodaySummaryAsync();

    // F10单步执行
    // F11进入方法
    // Shift+F11跳出方法
}
```

**条件断点：**
```csharp
// 右键断点 → 条件 → 条件表达式
foreach (var record in records)
{
    // 只在record.Id == 100时中断
    ProcessRecord(record);
}
```

**监视窗口：**
- 添加变量到监视窗口（Watch）
- 查看变量的值和类型
- 执行表达式（如：`records.Count`）

### 3. 使用日志

**控制台日志：**
```csharp
protected override async Task OnInitializedAsync()
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    Console.WriteLine("=== 页面初始化开始 ===");

    await LoadTodaySummary();
    Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}ms] 今日统计加载完成");

    await LoadRecentRecords();
    Console.WriteLine($"[{stopwatch.ElapsedMilliseconds}ms] 最近记录加载完成");

    stopwatch.Stop();
    Console.WriteLine($"=== 页面初始化完成，总耗时: {stopwatch.ElapsedMilliseconds}ms ===");
}
```

**ILogger日志：**
```csharp
public class WeighingRecordService : IWeighingRecordService
{
    private readonly ILogger<WeighingRecordService> _logger;

    public async Task<WeighingRecordResponse> CreateAsync(CreateWeighingRecordRequest request)
    {
        _logger.LogInformation("创建称重记录：{Barcode}", request.Barcode);

        try
        {
            // 业务逻辑...
            _logger.LogInformation("称重记录创建成功：{Id}", record.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建称重记录失败：{Barcode}", request.Barcode);
            throw;
        }
    }
}
```

### 4. 使用Blazor调试工具

**Blazor Server调试：**
- F5启动调试
- 在Razor组件中设置断点
- 在浏览器中操作，触发断点

**查看组件状态：**
```razor
@code {
    private string currentBarcode = string.Empty;

    // 添加调试输出
    protected override void OnParametersSet()
    {
        Console.WriteLine($"currentBarcode: {currentBarcode}");
    }
}
```

### 5. 使用SQL Profiler查看数据库查询

**EF Core日志：**
```csharp
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

**输出示例：**
```
Executed DbCommand (5ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
SELECT [w].[Id], [w].[Barcode], [w].[Weight]
FROM [WeighingRecords] AS [w]
WHERE [w].[CreatedAt] >= @__today_0
```

### 6. 使用Postman测试API

**测试SignalR Hub：**
```javascript
// 使用SignalR JavaScript客户端
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5001/hardwareHub")
    .build();

connection.on("ReceiveWeight", (data) => {
    console.log("收到重量数据：", data);
});

await connection.start();
```

### 7. 性能分析

**使用Stopwatch测量性能：**
```csharp
var stopwatch = System.Diagnostics.Stopwatch.StartNew();

// 执行操作
await LoadData();

stopwatch.Stop();
Console.WriteLine($"耗时：{stopwatch.ElapsedMilliseconds}ms");
```

**使用BenchmarkDotNet：**
```csharp
[MemoryDiagnoser]
public class QueryBenchmark
{
    [Benchmark]
    public async Task QueryWithFilter()
    {
        var records = await _repository.QueryPagedAsync(...);
    }

    [Benchmark]
    public async Task QueryWithoutFilter()
    {
        var records = await _repository.GetAllAsync();
    }
}
```

**老王的调试经验：**
- ✅ 先看控制台错误信息
- ✅ 使用断点调试，逐步排查
- ✅ 添加日志输出，记录关键步骤
- ✅ 使用Stopwatch测量性能
- ✅ 使用浏览器开发者工具查看网络请求
- ❌ 不要盲目猜测，要用数据说话

---

**下一章：** [附录](#附录)

---

# 附录

## 附录A：MiniMES项目文件结构

### 完整目录树

```
minimes/
├── Minimes.sln                          # 解决方案文件
├── CLAUDE.md                            # AI开发指引
├── README.md                            # 项目说明
├── DEPLOYMENT.md                        # 部署文档
├── BLAZOR-ASPNETCORE-TUTORIAL.md        # 本讲义文档
│
└── src/
    ├── Minimes.Domain/                  # 领域层
    │   ├── Entities/                    # 实体
    │   │   ├── User.cs
    │   │   ├── MeatType.cs
    │   │   ├── QRCode.cs
    │   │   └── WeighingRecord.cs
    │   ├── Enums/                       # 枚举
    │   │   ├── UserRole.cs
    │   │   └── WeightUnit.cs
    │   └── Interfaces/                  # 仓储接口
    │       ├── IRepository.cs
    │       ├── IUserRepository.cs
    │       ├── IMeatTypeRepository.cs
    │       ├── IQRCodeRepository.cs
    │       └── IWeighingRecordRepository.cs
    │
    ├── Minimes.Application/             # 应用层
    │   ├── Services/                    # 业务服务
    │   │   ├── UserService.cs
    │   │   ├── MeatTypeService.cs
    │   │   ├── QRCodeService.cs
    │   │   ├── WeighingRecordService.cs
    │   │   └── ReportService.cs
    │   ├── DTOs/                        # 数据传输对象
    │   │   ├── User/
    │   │   ├── MeatType/
    │   │   ├── QRCode/
    │   │   └── WeighingRecord/
    │   ├── Validators/                  # FluentValidation验证器
    │   │   ├── CreateUserValidator.cs
    │   │   └── CreateWeighingRecordValidator.cs
    │   ├── Mappings/                    # AutoMapper配置
    │   │   └── MappingProfile.cs
    │   ├── Interfaces/                  # 服务接口
    │   │   ├── IUserService.cs
    │   │   ├── IMeatTypeService.cs
    │   │   ├── IQRCodeService.cs
    │   │   ├── IWeighingRecordService.cs
    │   │   └── IReportService.cs
    │   ├── Resources/                   # 国际化资源
    │   │   ├── SharedResource.zh-CN.resx
    │   │   └── SharedResource.en-US.resx
    │   └── Configuration/               # 配置类
    │       ├── ScaleConfiguration.cs
    │       └── WeightValidationConfig.cs
    │
    ├── Minimes.Infrastructure/          # 基础设施层
    │   ├── Persistence/                 # 数据库持久化
    │   │   ├── ApplicationDbContext.cs
    │   │   ├── DatabaseExtensions.cs
    │   │   ├── SeedData.cs
    │   │   └── Configurations/
    │   │       ├── UserConfiguration.cs
    │   │       ├── MeatTypeConfiguration.cs
    │   │       ├── QRCodeConfiguration.cs
    │   │       └── WeighingRecordConfiguration.cs
    │   ├── Repositories/                # 仓储实现
    │   │   ├── Repository.cs
    │   │   ├── UserRepository.cs
    │   │   ├── MeatTypeRepository.cs
    │   │   ├── QRCodeRepository.cs
    │   │   └── WeighingRecordRepository.cs
    │   ├── Hardware/                    # 硬件集成
    │   │   ├── ScaleService.cs
    │   │   ├── WiFiScaleService.cs
    │   │   └── BarcodeScannerService.cs
    │   ├── Excel/                       # Excel导出
    │   │   └── ExcelExportService.cs
    │   └── Services/                    # 其他服务
    │       └── PasswordHashService.cs
    │
    └── Minimes.Web/                     # Web表示层
        ├── Pages/                       # Blazor页面
        │   ├── Index.razor
        │   ├── Login.razor
        │   ├── Logout.razor
        │   ├── Profile.razor
        │   ├── Settings.razor
        │   ├── HardwareTest.razor
        │   ├── Users/
        │   │   ├── Index.razor
        │   │   ├── Create.razor
        │   │   └── Edit.razor
        │   ├── MeatTypes/
        │   │   ├── Index.razor
        │   │   ├── Create.razor
        │   │   └── Edit.razor
        │   ├── QRCodes/
        │   │   ├── Index.razor
        │   │   ├── Create.razor
        │   │   ├── BatchCreate.razor
        │   │   └── Print.razor
        │   ├── Weighing/
        │   │   ├── WeighingPage.razor
        │   │   └── RecordList.razor
        │   └── Reports/
        │       ├── Production.razor
        │       └── Tracing.razor
        ├── Shared/                      # 共享组件
        │   ├── MainLayout.razor
        │   ├── NavMenu.razor
        │   ├── CultureSelector.razor
        │   └── RedirectToLogin.razor
        ├── Hubs/                        # SignalR Hub
        │   └── HardwareHub.cs
        ├── Services/                    # 前端服务
        │   └── HardwareBackgroundService.cs
        ├── Extensions/                  # 扩展方法
        │   └── AuthenticationExtensions.cs
        ├── wwwroot/                     # 静态资源
        │   ├── css/
        │   │   ├── site.css
        │   │   └── tablet.css
        │   └── js/
        │       └── utils.js
        ├── Program.cs                   # 应用启动
        ├── appsettings.json             # 配置文件
        └── appsettings.Production.json  # 生产环境配置
```

### 关键文件说明

| 文件 | 作用 | 重要性 |
|-----|------|-------|
| `Program.cs` | 应用启动配置 | ⭐⭐⭐⭐⭐ |
| `ApplicationDbContext.cs` | EF Core数据库上下文 | ⭐⭐⭐⭐⭐ |
| `WeighingPage.razor` | 核心业务页面 | ⭐⭐⭐⭐⭐ |
| `HardwareHub.cs` | SignalR实时通信 | ⭐⭐⭐⭐ |
| `ScaleService.cs` | 电子秤硬件集成 | ⭐⭐⭐⭐ |
| `MappingProfile.cs` | AutoMapper配置 | ⭐⭐⭐ |
| `SharedResource.*.resx` | 国际化资源 | ⭐⭐⭐ |

---

## 附录B：常用NuGet包说明

### 核心框架包

| 包名 | 版本 | 作用 | 层级 |
|-----|------|------|------|
| `Microsoft.AspNetCore.App` | 8.0 | ASP.NET Core框架 | Web |
| `Microsoft.EntityFrameworkCore` | 8.0.11 | EF Core核心 | Infrastructure |
| `Microsoft.EntityFrameworkCore.Sqlite` | 8.0.11 | SQLite数据库支持 | Infrastructure |
| `Pomelo.EntityFrameworkCore.MySql` | 8.0.2 | MySQL数据库支持 | Infrastructure |

### 业务功能包

| 包名 | 版本 | 作用 | 层级 |
|-----|------|------|------|
| `AutoMapper` | 12.0.1 | 对象映射 | Application |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | 12.0.1 | AutoMapper DI集成 | Application |
| `FluentValidation` | 12.1.1 | 数据验证 | Application |
| `FluentValidation.DependencyInjectionExtensions` | 12.1.1 | FluentValidation DI集成 | Application |

### 硬件集成包

| 包名 | 版本 | 作用 | 层级 |
|-----|------|------|------|
| `System.IO.Ports` | 10.0.1 | 串口通信（电子秤） | Infrastructure |
| `Microsoft.AspNetCore.SignalR.Client` | 8.0.11 | SignalR客户端 | Web |

### Excel导出包

| 包名 | 版本 | 作用 | 层级 |
|-----|------|------|------|
| `EPPlus` | 8.4.0 | Excel导出 | Infrastructure |

### 工具包

| 包名 | 版本 | 作用 | 层级 |
|-----|------|------|------|
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.11 | EF Core迁移工具 | Infrastructure |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.11 | EF Core设计时工具 | Infrastructure |

### 安装命令

```bash
# Domain层（无依赖）
# 无需安装NuGet包

# Application层
cd src/Minimes.Application
dotnet add package AutoMapper --version 12.0.1
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add package FluentValidation --version 12.1.1
dotnet add package FluentValidation.DependencyInjectionExtensions --version 12.1.1

# Infrastructure层
cd ../Minimes.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.11
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.2
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package System.IO.Ports --version 10.0.1
dotnet add package EPPlus --version 8.4.0

# Web层
cd ../Minimes.Web
dotnet add package Microsoft.AspNetCore.SignalR.Client --version 8.0.11
```

---

## 附录C：参考资源

### 官方文档

**Blazor：**
- [Blazor官方文档](https://learn.microsoft.com/zh-cn/aspnet/core/blazor/)
- [Blazor组件](https://learn.microsoft.com/zh-cn/aspnet/core/blazor/components/)
- [Blazor数据绑定](https://learn.microsoft.com/zh-cn/aspnet/core/blazor/components/data-binding)
- [Blazor路由](https://learn.microsoft.com/zh-cn/aspnet/core/blazor/fundamentals/routing)

**ASP.NET Core：**
- [ASP.NET Core官方文档](https://learn.microsoft.com/zh-cn/aspnet/core/)
- [依赖注入](https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/dependency-injection)
- [中间件](https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/middleware/)
- [配置](https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration/)

**Entity Framework Core：**
- [EF Core官方文档](https://learn.microsoft.com/zh-cn/ef/core/)
- [EF Core迁移](https://learn.microsoft.com/zh-cn/ef/core/managing-schemas/migrations/)
- [EF Core查询](https://learn.microsoft.com/zh-cn/ef/core/querying/)

**SignalR：**
- [SignalR官方文档](https://learn.microsoft.com/zh-cn/aspnet/core/signalr/introduction)
- [SignalR Hub](https://learn.microsoft.com/zh-cn/aspnet/core/signalr/hubs)
- [SignalR客户端](https://learn.microsoft.com/zh-cn/aspnet/core/signalr/javascript-client)

### 开源项目

**Blazor示例项目：**
- [Blazor Samples](https://github.com/dotnet/blazor-samples)
- [Awesome Blazor](https://github.com/AdrienTorris/awesome-blazor)

**Clean Architecture示例：**
- [Clean Architecture Solution Template](https://github.com/jasontaylordev/CleanArchitecture)
- [eShopOnWeb](https://github.com/dotnet-architecture/eShopOnWeb)

### 学习资源

**视频教程：**
- [Blazor入门教程（Microsoft Learn）](https://learn.microsoft.com/zh-cn/training/paths/build-web-apps-with-blazor/)
- [ASP.NET Core教程（Microsoft Learn）](https://learn.microsoft.com/zh-cn/training/paths/aspnet-core-web-app/)

**书籍推荐：**
- 《Blazor in Action》- Chris Sainty
- 《ASP.NET Core in Action》- Andrew Lock
- 《Entity Framework Core in Action》- Jon P Smith

**社区资源：**
- [Stack Overflow - Blazor标签](https://stackoverflow.com/questions/tagged/blazor)
- [Reddit - r/Blazor](https://www.reddit.com/r/Blazor/)
- [Blazor University](https://blazor-university.com/)

### 工具推荐

**开发工具：**
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [Visual Studio Code](https://code.visualstudio.com/)
- [JetBrains Rider](https://www.jetbrains.com/rider/)

**数据库工具：**
- [DB Browser for SQLite](https://sqlitebrowser.org/)
- [MySQL Workbench](https://www.mysql.com/products/workbench/)
- [Azure Data Studio](https://azure.microsoft.com/zh-cn/products/data-studio/)

**调试工具：**
- [Postman](https://www.postman.com/)
- [Fiddler](https://www.telerik.com/fiddler)
- [Chrome DevTools](https://developer.chrome.com/docs/devtools/)

### MiniMES项目资源

**项目地址：**
- GitHub: `D:\MyDomain\src\AI\minimes`

**关键文档：**
- `CLAUDE.md` - AI开发指引
- `README.md` - 项目说明
- `DEPLOYMENT.md` - 部署文档
- `BLAZOR-ASPNETCORE-TUTORIAL.md` - 本讲义文档

**联系方式：**
- 项目作者：老王（技术暴躁流）
- 最后更新：2026-01-29

---

# 结语

艹！这份讲义终于写完了！老王我花了不少心思，把Blazor和ASP.NET Core的核心知识点都讲透了。

**你学到了什么？**

1. **Blazor核心概念**：组件、生命周期、数据绑定、事件处理、依赖注入
2. **ASP.NET Core核心概念**：Program.cs、DI容器、中间件、配置、认证授权、SignalR
3. **Clean Architecture**：Domain、Application、Infrastructure、Web四层架构
4. **实战案例**：称重页面、SignalR实时推送、布局导航、认证授权、国际化
5. **最佳实践**：SOLID原则、KISS/DRY/YAGNI原则、性能优化、调试技巧

**下一步怎么做？**

1. **动手实践**：克隆MiniMES项目，运行起来，修改代码，看看效果
2. **深入学习**：选择感兴趣的模块，深入研究源码
3. **独立开发**：尝试开发自己的Blazor项目，应用所学知识
4. **持续学习**：关注官方文档更新，学习新特性

**老王的寄语：**

编程这玩意儿，光看不练是学不会的。老王我写这份讲义，不是让你背下来，而是让你理解原理，然后自己动手写代码。

遇到问题别慌，先看错误信息，再打断点调试，实在不行就Google/Stack Overflow。记住：**没有解决不了的Bug，只有不够努力的程序员！**

最后，祝你在Blazor和ASP.NET Core的世界里玩得开心！有问题随时来找老王我！

---

**文档版本：** v1.0
**最后更新：** 2026-01-29
**作者：** 老王（技术暴躁流）
**项目：** MiniMES 记账系统

---

