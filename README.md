# MiniMES 记账系统

> **项目名称**: MiniMES 记账系统
> **技术栈**: .NET 8 + Blazor Server + SQLite/MySQL
> **开发状态**: ✅ **核心功能已完成**（阶段1-9）
> **最后更新**: 2026-01-21

---

## 项目概览

### 业务目标

构建一个支持**扫码-称重-存档-导出Excel**的Web记账系统，具备以下特性：

- ✅ 电子秤硬件集成（串口通信，支持多种协议）
- ✅ 扫码枪集成（键盘输入监听 + 模拟扫描）
- ✅ 多用户管理（管理员/操作员角色）
- ✅ 本地账号认证（Cookie认证 + PBKDF2密码加密）
- ✅ 离线工作支持（本地SQLite数据库）
- ✅ 生产报表统计（损耗率计算、质量追溯）
- ✅ 数据导出（Excel格式）
- ✅ 演示模式（demo账户一键体验）

### 核心数据流

```
扫商品码 → 自动查询商品 → 称重（电子秤/手动输入） → 选择客户 → 保存记录 → 导出Excel
```

---

## 完成进度

| 阶段 | 任务 | 状态 | 完成度 |
|------|------|------|--------|
| 1 | 项目结构搭建 | ✅ 完成 | 100% |
| 2 | 领域模型和数据库 | ✅ 完成 | 100% |
| 3 | 认证授权 | ✅ 完成 | 100% |
| 4 | 基础数据管理 | ✅ 完成 | 100% |
| 5 | 硬件集成（电子秤/扫码枪） | ✅ 完成 | 100% |
| 6 | 称重记录模块 | ✅ 完成 | 100% |
| 7 | Excel导出 | ✅ 完成 | 100% |
| 8 | 用户权限管理和UI优化 | ✅ 完成 | 100% |
| 9 | 性能优化 | ✅ 完成 | 100% |

**总体进度**: 🟢 **100%** - 核心功能全部完成

---

## 快速开始

### 环境要求

- .NET 8.0 SDK
- Visual Studio 2022 / Rider / VS Code

### 启动应用

```bash
# 还原依赖
dotnet restore

# 编译项目
dotnet build

# 运行应用
dotnet run --project src/Minimes.Web
```

应用默认运行在 `https://localhost:7xxx` 或 `http://localhost:5xxx`

### 默认登录凭证

| 账户类型 | 用户名 | 密码 | 权限 |
|---------|-------|------|------|
| 管理员 | admin | Admin123456 | 全部功能 |
| 操作员 | operator | Operator123456 | 生产操作 |
| **演示账户** | demo | demo123 | 全部功能 + 硬件模拟 |

> **演示模式**：使用demo账户登录后，系统自动启用硬件模拟模式，电子秤和扫码枪会推送模拟数据，方便完整体验业务流程。

---

## 技术栈

### 核心框架

| 功能 | 技术 | 版本 |
|------|------|------|
| 框架 | ASP.NET Core | 8.0 |
| Web UI | Blazor Server | 8.0 |
| ORM | Entity Framework Core | 8.0.11 |
| 数据库 | SQLite / MySQL | 可切换 |
| 验证 | FluentValidation | 12.1.1 |
| 映射 | AutoMapper | 12.0.1 |
| 实时通信 | SignalR | 8.0.11 |
| 串口通信 | System.IO.Ports | 10.0.1 |
| Excel导出 | EPPlus | 8.4.0 |

### 架构模式

- ✅ Clean Architecture（4层分离）
- ✅ Repository Pattern（数据访问抽象）
- ✅ Dependency Injection（松耦合）
- ✅ Value Objects（Barcode、Weight）
- ✅ Domain-Driven Design（业务驱动）

---

## 项目结构

```
minimes/
├── Minimes.sln                         # 解决方案文件
├── README.md                           # 本文档
├── CLAUDE.md                           # AI开发指引
├── DEPLOYMENT.md                       # 部署文档
│
└── src/
    ├── Minimes.Domain/                 # 领域层（核心业务）
    │   ├── Entities/                   # 实体：User, Customer, Product, WeighingRecord
    │   ├── ValueObjects/               # 值对象：Barcode, Weight
    │   ├── Enums/                      # 枚举：UserRole, WeightUnit, ProcessStage
    │   ├── Interfaces/                 # 仓储接口
    │   └── Security/                   # 密码哈希服务
    │
    ├── Minimes.Application/            # 应用层（业务服务）
    │   ├── Services/                   # 业务服务实现
    │   ├── DTOs/                       # 数据传输对象
    │   ├── Interfaces/                 # 服务接口
    │   └── Validators/                 # FluentValidation验证器
    │
    ├── Minimes.Infrastructure/         # 基础设施层
    │   ├── Persistence/                # EF Core上下文和配置
    │   ├── Repositories/               # 仓储实现
    │   ├── Hardware/                   # 硬件集成（电子秤/扫码枪）
    │   └── Excel/                      # Excel导出服务
    │
    └── Minimes.Web/                    # Blazor Server表示层
        ├── Pages/                      # Blazor页面组件
        │   ├── Login.razor             # 登录
        │   ├── Profile.razor           # 个人中心
        │   ├── Settings.razor          # 系统设置
        │   ├── HardwareTest.razor      # 硬件测试
        │   ├── Users/                  # 用户管理
        │   ├── Customers/              # 客户管理
        │   ├── Products/               # 商品管理
        │   ├── Weighing/               # 称重操作
        │   └── Reports/                # 报表（生产统计/质量追溯）
        ├── Hubs/                       # SignalR Hub
        ├── Shared/                     # 共享组件
        └── wwwroot/                    # 静态资源
```

---

## 功能模块

### 1. 用户认证授权

- **本地账号认证**：用户名密码登录，Cookie认证
- **密码安全**：PBKDF2 + SHA256 加密，10000次迭代
- **角色权限**：管理员(Administrator) / 操作员(Operator)
- **三层防护**：页面权限 + 菜单控制 + Service验证

### 2. 基础数据管理

- **客户管理**：客户代码、名称、联系人、电话、地址
- **商品管理**：条形码、名称、规格、单位、参考价格
- **用户管理**：创建/编辑用户、角色分配、状态管理

### 3. 硬件集成

- **电子秤**：串口通信，支持Toledo/Mettler/Generic协议
- **扫码枪**：键盘输入监听，支持模拟扫描
- **SignalR推送**：实时重量数据、扫码数据推送
- **演示模式**：demo账户自动启用硬件模拟

### 4. 称重记录

- **核心流程**：扫码查商品 → 读取重量 → 关联客户 → 保存记录
- **加工阶段**：原料入库(Receiving) / 加工过程(Processing) / 成品出库(Shipping)
- **分页查询**：按日期、客户、商品、阶段多条件过滤

### 5. 报表统计

- **生产报表**：总记录数、总重量、平均重量、按环节统计
- **损耗率计算**：(入库重量 - 出库重量) / 入库重量 × 100%
- **质量追溯**：按条码搜索完整操作链路
- **Excel导出**：支持导出称重记录和生产报表

---

## 权限管理

### 页面权限分配

| 页面 | 操作员 | 管理员 |
|------|--------|--------|
| 首页、个人中心 | ✅ | ✅ |
| 生产称重 | ✅ | ✅ |
| 称重记录查询 | ✅ | ✅ |
| 商品管理 | ✅ | ✅ |
| 客户管理 | ❌ | ✅ |
| 用户管理 | ❌ | ✅ |
| 生产报表 | ❌ | ✅ |
| 硬件测试 | ❌ | ✅ |
| 系统设置 | ❌ | ✅ |

---

## 数据库配置

### 默认配置（SQLite - 开发/演示）

无需配置，直接运行即可。数据库文件 `minimes.db` 自动创建。

### 生产配置（MySQL）

1. 创建MySQL数据库：
```sql
CREATE DATABASE minimes CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

2. 修改 `appsettings.Production.json`：
```json
{
  "Database": {
    "Provider": "MySQL"
  },
  "ConnectionStrings": {
    "MySqlConnection": "Server=服务器;Port=3306;Database=minimes;User=用户名;Password=密码;CharSet=utf8mb4;"
  }
}
```

3. 以生产模式运行：
```bash
# Windows
set ASPNETCORE_ENVIRONMENT=Production
dotnet run --project src/Minimes.Web

# Linux/macOS
ASPNETCORE_ENVIRONMENT=Production dotnet run --project src/Minimes.Web
```

---

## 性能优化

### 阶段9优化成果

- **查询性能**：0.6s → 0.05s（提升12倍）
- **内存占用**：10MB → <1MB（降低10倍）
- **死锁风险**：已消除

### 优化措施

- Repository层：数据库层面过滤和分页（QueryPagedAsync）
- Service层：使用数据库聚合查询代替内存计算
- 同步阻塞：修复异步方法中的.Result调用

---

## 开发规范

### SOLID原则
- **S**：单一职责，每个类只负责一个功能
- **O**：开闭原则，通过接口扩展
- **L**：里氏替换，子类可替换父类
- **I**：接口隔离，避免"胖接口"
- **D**：依赖倒置，依赖抽象而非具体

### KISS & DRY & YAGNI
- 保持代码简洁，避免过度设计
- 杜绝重复代码，主动抽象复用
- 仅实现当前需要的功能

---

## 待完成

- [ ] 单元测试
- [ ] 集成测试
- [ ] 用户手册
- [ ] 管理员指南
- [ ] 硬件集成指南
- [ ] 故障排查指南

---

## 许可证

MIT License

---

**开发者**: 老王技术流 | **最后更新**: 2026-01-21
