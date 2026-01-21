# 🎯 MiniMES 记账系统 - 项目进度文档

> **项目名称**: MiniMES 记账系统
> **技术栈**: ~~.NET 8 + Blazor Server + SQLite~~ → ⚠️ **.NET 8 + MAUI Blazor Hybrid + SQLite**
> **开发状态**: 🚨 **架构重构中**（V1.0完成7个阶段，开始V2.0 MAUI迁移）
> **最后更新**: 2026-01-12

---

## 📊 项目概览

### 业务目标
构建一个支持**扫码-称重-存档-导出Excel**的~~Web记账系统~~ **移动记账系统（安卓平板）**，具备以下特性：
- ✅ 电子秤硬件集成（~~串口~~ → USB OTG / 蓝牙）
- ✅ 多用户管理（管理员/操作员）
- ~~✅ 本地账号认证 + 第三方OAuth（WeChat、Google）~~ → 🔄 本地认证（SecureStorage）
- ~~✅ 离线工作支持（基于Cookie认证）~~ → ✅ **完全离线独立运行**
- ✅ 数据导出（Excel格式）

### 核心数据流
```
扫商品码 → 自动查询商品 → 称重（USB OTG） → 选择客户 → 保存记录 → 导出Excel
```

### 架构演进 ⚠️

| 版本 | 架构 | 部署方式 | 状态 |
|------|------|----------|------|
| V1.0 | Blazor Server | Windows服务器 + 浏览器 | ✅ 已完成（7/9阶段） |
| V2.0 | MAUI Blazor Hybrid | 安卓平板APK独立运行 | 🔄 重构中 |

**重构原因**：用户需求变更 - 需要安卓平板独立运行（离线、移动使用）

---

## ✅ 完成进度统计

### V1.0 Blazor Server版本（已完成）

| 阶段 | 任务 | 状态 | 完成度 |
|------|------|------|--------|
| 1 | 项目结构搭建 | ✅ 完成 | 100% |
| 2 | 领域模型和数据库 | ✅ 完成 | 100% |
| 3 | 认证授权 | ✅ 完成 | 100% |
| 4 | 基础数据管理 | ✅ 完成 | 100% |
| 5 | 硬件集成（Windows串口） | ✅ 完成 | 100% |
| 6 | 称重记录模块 | ✅ 完成 | 100% |
| 7 | Excel导出 | ✅ 完成 | 100% |
| 8 | UI优化和测试 | ⏸️ 暂停 | 70% |
| 9 | 部署和文档 | ⏸️ 暂停 | 10% |

**V1.0总体进度**: 🟢 **78%** (7/9 阶段完成) - 暂停开发

### V2.0 MAUI Blazor Hybrid版本（重构中）⚠️

| 阶段 | 任务 | 状态 | 完成度 |
|------|------|------|--------|
| Phase 1 | 环境准备和项目创建 | ⏳ 待开始 | 0% |
| Phase 2 | 数据层迁移 | ⏳ 待开始 | 0% |
| Phase 3 | UI迁移（Blazor复用） | ⏳ 待开始 | 0% |
| Phase 4 | 硬件集成重写（USB OTG） | ⏳ 待开始 | 0% |
| Phase 5 | 认证重写（SecureStorage） | ⏳ 待开始 | 0% |
| Phase 6 | 测试和调试 | ⏳ 待开始 | 0% |
| Phase 7 | 打包和部署 | ⏳ 待开始 | 0% |

**V2.0总体进度**: 🔴 **0%** (0/7 阶段完成) - 预计17天完成

---

## 🏗️ 阶段1：项目结构搭建 ✅

### 目标
创建符合Clean Architecture的4层项目结构

### 完成内容
- ✅ 创建Visual Studio解决方案 (`Minimes.sln`)
- ✅ 创建4个核心项目：
  - `Minimes.Domain` - 领域层
  - `Minimes.Application` - 应用层
  - `Minimes.Infrastructure` - 基础设施层
  - `Minimes.Web` - 表示层（Blazor Server）
- ✅ 配置项目引用关系（保证依赖方向正确）
- ✅ 安装NuGet核心包
- ✅ 统一Target Framework为 .NET 8.0

### 关键文件
```
Minimes.sln
src/
├── Minimes.Domain/              # 领域层
├── Minimes.Application/         # 应用层
├── Minimes.Infrastructure/      # 基础设施层
└── Minimes.Web/                 # 表示层（Blazor）
```

---

## 📋 阶段2：领域模型和数据库 ✅

### 目标
定义核心业务实体和数据库架构

### 完成内容

#### Domain Entities（5个核心实体）
| 实体 | 用途 | 关键字段 |
|------|------|---------|
| `User` | 系统用户 | Username(唯一), PasswordHash, FullName, Role, IsActive |
| `Customer` | 客户信息 | Code(唯一), Name, ContactPerson, Phone, Address |
| `Product` | 商品信息 | Barcode(唯一), Name, Specification, Unit, ReferencePrice |
| `WeighingRecord` | 称重记录 | UserId, CustomerId, ProductId, WeightValue, WeighedAt, Remark |
| `UserOAuthAccount` | OAuth账号绑定 | UserId, ProviderType, ProviderUserId, ProviderName, RefreshToken |

#### Enums（3个枚举）
```csharp
UserRole: Operator, Administrator
WeightUnit: Gram, Kilogram, Ton
OAuthProviderType: WeChat, Google, Local
```

#### Value Objects（2个值对象）
```csharp
Barcode: 不可变条形码对象，包含验证逻辑
Weight: 重量+单位，支持单位转换（克、公斤、吨）
```

#### 数据库迁移
- ✅ Migration 1: `InitialCreate` - 创建5张表 + 15个索引
- ✅ Migration 2: `AddOAuthSupport` - 添加OAuth表
- ✅ Migration 3: `AddReferencePrice` - 添加商品参考价格字段

#### 数据库索引策略
```sql
-- 唯一索引（业务键）
Users: Username
Customers: Code
Products: Barcode
UserOAuthAccounts: (ProviderType, ProviderUserId)

-- 性能索引
Products: Name
Customers: Name, IsActive
Users: IsActive
WeighingRecords: (CustomerId, WeighedAt DESC), WeighedAt DESC
```

### 关键文件
```
src/Minimes.Domain/
├── Entities/
│   ├── BaseEntity.cs
│   ├── User.cs
│   ├── Customer.cs
│   ├── Product.cs
│   ├── WeighingRecord.cs
│   └── OAuth/UserOAuthAccount.cs
├── Enums/
│   ├── UserRole.cs
│   ├── WeightUnit.cs
│   └── OAuthProviderType.cs
├── ValueObjects/
│   ├── Barcode.cs
│   └── Weight.cs
└── Interfaces/
    ├── IRepository.cs
    ├── IUserRepository.cs
    ├── ICustomerRepository.cs
    ├── IProductRepository.cs
    ├── IWeighingRecordRepository.cs
    └── IUserOAuthAccountRepository.cs

src/Minimes.Infrastructure/
└── Persistence/
    ├── ApplicationDbContext.cs
    ├── Configurations/
    │   ├── UserConfiguration.cs
    │   ├── CustomerConfiguration.cs
    │   ├── ProductConfiguration.cs
    │   ├── WeighingRecordConfiguration.cs
    │   └── UserOAuthAccountConfiguration.cs
    └── Migrations/
        ├── 20260106062042_InitialCreate.cs
        ├── 20260106064932_AddOAuthSupport.cs
        └── 20260106070932_AddReferencePrice.cs
```

---

## 🔐 阶段3：认证授权 ✅

### 目标
实现本地账号认证 + 第三方OAuth框架

### 3.1 本地账号认证 ✅

#### DTOs
- `RegisterRequest` - 注册请求
- `LoginRequest` - 登录请求
- `LoginResponse` - 登录响应
- `ChangePasswordRequest` - 修改密码请求
- `UserResponse` - 用户信息响应

#### 验证器（FluentValidation）
```csharp
RegisterRequestValidator:
  - 用户名: 4-50字符，[a-zA-Z0-9_]
  - 密码: 6-100字符，必含大小写+数字
  - 确认密码必须匹配

LoginRequestValidator:
  - 用户名/密码必填

ChangePasswordRequestValidator:
  - 新密码强度同注册
  - 新密码≠旧密码
```

#### 密码安全服务
```csharp
PasswordHashService (Domain.Security)
  - 算法: PBKDF2 + SHA256
  - 迭代: 10,000次
  - 盐值: 16字节随机
  - 防护: 恒定时间比较（防时序攻击）
  - 格式: base64(salt):base64(hash)
```

#### 业务服务
```csharp
UserService:
  ✅ CreateAsync - 创建用户（密码哈希）
  ✅ ValidatePasswordAsync - 验证密码
  ✅ ChangePasswordAsync - 修改密码
  ✅ ResetPasswordAsync - 重置密码（管理员）
  ✅ GetByUsernameAsync - 按用户名查询

AuthenticationService:
  ✅ LoginAsync - 登录（返回用户信息）
  ✅ RegisterAsync - 注册（创建新用户）
  ✅ ChangePasswordAsync - 修改密码
```

#### Cookie认证配置
```csharp
AuthenticationExtensions:
  - Cookie名称: MinimesAuth
  - HttpOnly: true（防XSS）
  - Secure: true（HTTPS only）
  - 过期时间: 30天
  - 滑动过期: true（自动延期）
  - 登录路由: /login
  - API错误: 返回401/403而非重定向

授权策略:
  - "Admin" - 仅管理员
  - "Operator" - 操作员+管理员
  - "Authenticated" - 任何已认证用户
```

#### 种子数据
```
默认账户:
  - 管理员: admin / Admin123456
  - 操作员: operator / Operator123456

初始化时自动创建，应用启动时检查
```

### 3.2 第三方OAuth框架 ✅

#### OAuth提供商接口
```csharp
IOAuthProvider:
  - GetAuthorizationUrl() - 生成授权链接
  - GetUserInfoAsync() - 交换Token获取用户信息
  - ProviderName - 提供商名称

OAuthUserInfo:
  - ProviderUserId - 第三方用户ID
  - Name - 用户昵称
  - Avatar - 头像URL
  - RefreshToken - 刷新Token
```

#### 已实现的提供商

**WeChat OAuth**
```csharp
WeChatOAuthProvider:
  - 授权URL: https://open.weixin.qq.com/connect/oauth2/authorize
  - Token交换: /sns/oauth2/access_token
  - 获取信息: /sns/userinfo
  - 返回: openid, nickname, headimgurl
```

**Google OAuth**
```csharp
GoogleOAuthProvider:
  - 授权URL: https://accounts.google.com/o/oauth2/v2/auth
  - Token交换: https://oauth2.googleapis.com/token
  - 获取信息: https://www.googleapis.com/oauth2/v2/userinfo
  - Scope: openid profile email
```

#### 工厂模式
```csharp
OAuthProviderFactory:
  - 根据提供商名称返回对应实例
  - 扩展新提供商无需修改核心代码
```

#### 配置（appsettings.json）
```json
{
  "OAuth": {
    "WeChat": {
      "ClientId": "your_wechat_app_id",
      "ClientSecret": "your_wechat_app_secret"
    },
    "Google": {
      "ClientId": "your_google_client_id",
      "ClientSecret": "your_google_client_secret"
    }
  }
}
```

### 关键文件
```
src/Minimes.Application/
├── DTOs/User/
│   ├── RegisterRequest.cs
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   ├── ChangePasswordRequest.cs
│   └── UserResponse.cs
├── Validators/User/
│   ├── RegisterRequestValidator.cs
│   ├── LoginRequestValidator.cs
│   └── ChangePasswordRequestValidator.cs
├── Interfaces/
│   ├── IUserService.cs
│   ├── IAuthenticationService.cs
│   └── IOAuthProvider.cs
└── Services/
    ├── UserService.cs
    └── AuthenticationService.cs

src/Minimes.Domain/
└── Security/
    └── PasswordHashService.cs

src/Minimes.Infrastructure/
├── Authentication/
│   ├── OAuthProviderFactory.cs
│   ├── WeChatOAuthProvider.cs
│   ├── GoogleOAuthProvider.cs
│   └── OAuthUserInfo.cs
└── Persistence/
    └── SeedData.cs

src/Minimes.Web/
├── Extensions/
│   └── AuthenticationExtensions.cs
└── Program.cs (已集成Cookie认证)
```

---

## 📦 阶段4：基础数据管理 ✅

### 目标
实现客户和商品的完整CRUD服务

### 完成内容

#### 客户管理

**DTOs**
```csharp
CreateCustomerRequest
UpdateCustomerRequest
CustomerResponse
```

**验证器**
```csharp
CreateCustomerRequestValidator:
  - 代码: 4-50字符，[a-zA-Z0-9_-]
  - 名称: 1-100字符必填
  - 联系人: ≤50字符
  - 电话: 支持国内格式验证
  - 地址: ≤200字符

UpdateCustomerRequestValidator:
  - 同上 + ID必填验证
```

**ICustomerService 接口**
```csharp
✅ CreateAsync - 创建客户（代码唯一性检查）
✅ GetByIdAsync - 按ID查询
✅ GetByCodeAsync - 按代码查询（扫码场景）
✅ GetActiveCustomersAsync - 获取活跃客户列表
✅ SearchByNameAsync - 按名称搜索（模糊匹配）
✅ UpdateAsync - 更新客户信息
✅ DeleteAsync - 删除客户
✅ CodeExistsAsync - 代码唯一性检查
✅ GetAllAsync - 获取全部客户
```

**CustomerService 实现**
- 完整的业务逻辑验证
- 代码唯一性自动检查
- 支持批量查询优化

#### 商品管理

**DTOs**
```csharp
CreateProductRequest
UpdateProductRequest
ProductResponse
```

**验证器**
```csharp
CreateProductRequestValidator:
  - 条形码: 1-50字符，[0-9a-zA-Z-]
  - 名称: 1-100字符必填
  - 规格: ≤200字符
  - 单位: 1-20字符必填
  - 参考价格: 如果填写需>0，最多2位小数

UpdateProductRequestValidator:
  - 同上 + ID必填验证
```

**IProductService 接口**
```csharp
✅ CreateAsync - 创建商品（条形码唯一性检查）
✅ GetByIdAsync - 按ID查询
✅ GetByBarcodeAsync - 按条形码查询（扫码枪集成）
✅ GetActiveProductsAsync - 获取活跃商品列表
✅ SearchByNameAsync - 按名称+规格搜索
✅ UpdateAsync - 更新商品信息
✅ DeleteAsync - 删除商品
✅ BarcodeExistsAsync - 条形码唯一性检查
✅ GetAllAsync - 获取全部商品
```

**ProductService 实现**
- 条形码唯一性自动检查
- 支持名称和规格的模糊搜索
- 参考价格精度验证

#### 仓储层（Repository）

**通用Repository**
```csharp
Repository<T>: IRepository<T>
  ✅ GetByIdAsync
  ✅ GetAllAsync
  ✅ AddAsync
  ✅ UpdateAsync
  ✅ DeleteAsync
  ✅ SaveChangesAsync
```

**具体仓储实现**
```csharp
CustomerRepository: Repository<Customer>, ICustomerRepository
  ✅ GetByCodeAsync
  ✅ SearchByNameAsync
  ✅ GetActiveCustomersAsync

ProductRepository: Repository<Product>, IProductRepository
  ✅ GetByBarcodeAsync
  ✅ SearchByNameAsync
  ✅ GetActiveProductsAsync

UserRepository: Repository<User>, IUserRepository
  ✅ GetByUsernameAsync
  ✅ GetActiveUsersAsync
  ✅ UsernameExistsAsync

WeighingRecordRepository: Repository<WeighingRecord>, IWeighingRecordRepository
  ✅ GetByDateRangeAsync
  ✅ GetByCustomerIdAsync
  ✅ GetByProductIdAsync
  ✅ GetByUserIdAsync
  ✅ GetLatestAsync(count)

UserOAuthAccountRepository: Repository<UserOAuthAccount>, IUserOAuthAccountRepository
  ✅ GetByProviderAsync
  ✅ GetByUserIdAsync
  ✅ ExistsAsync
```

#### 依赖注入配置
```csharp
Minimes.Application.DependencyInjection:
  ✅ 注册所有验证器（FluentValidation）
  ✅ 注册所有业务服务（Service）

Minimes.Infrastructure.DependencyInjection:
  ✅ 注册所有仓储（Repository）

Program.cs:
  ✅ builder.Services.AddApplicationServices()
  ✅ builder.Services.AddInfrastructureServices()
  ✅ builder.Services.AddCustomAuthentication()
```

### 关键文件
```
src/Minimes.Application/
├── DTOs/
│   ├── Customer/
│   │   ├── CreateCustomerRequest.cs
│   │   ├── UpdateCustomerRequest.cs
│   │   └── CustomerResponse.cs
│   └── Product/
│       ├── CreateProductRequest.cs
│       ├── UpdateProductRequest.cs
│       └── ProductResponse.cs
├── Validators/
│   ├── Customer/
│   │   ├── CreateCustomerRequestValidator.cs
│   │   └── UpdateCustomerRequestValidator.cs
│   └── Product/
│       ├── CreateProductRequestValidator.cs
│       └── UpdateProductRequestValidator.cs
├── Interfaces/
│   ├── ICustomerService.cs
│   └── IProductService.cs
└── Services/
    ├── CustomerService.cs
    └── ProductService.cs

src/Minimes.Infrastructure/
└── Repositories/
    ├── Repository.cs
    ├── CustomerRepository.cs
    ├── ProductRepository.cs
    ├── UserRepository.cs
    ├── WeighingRecordRepository.cs
    └── UserOAuthAccountRepository.cs
```

---

## ⚙️ 阶段5：硬件集成 ✅

### 目标
集成电子秤和扫码枪硬件，实现实时数据采集

### 完成内容

#### 电子秤串口服务（ScaleService）
```csharp
功能特性:
  ✅ 多协议支持（Toledo、Mettler、Generic）
  ✅ 自动协议检测
  ✅ 实时重量读取
  ✅ 稳定性检测（连续3次相同值）
  ✅ 去皮功能
  ✅ 错误重连机制

配置参数:
  - 串口号: COM1-COM9可配置
  - 波特率: 9600（默认）
  - 数据位: 8
  - 停止位: 1
  - 校验位: None
```

#### 扫码枪服务（BarcodeScannerService）
```csharp
功能特性:
  ✅ 键盘输入监听（模拟扫码枪）
  ✅ 模拟扫描功能（测试用）
  ✅ 条形码格式验证
  ✅ 事件驱动架构
```

#### SignalR实时推送（HardwareHub）
```csharp
实时推送事件:
  ✅ SendWeightData - 推送重量数据
  ✅ SendBarcodeScanned - 推送扫码数据
  ✅ SendError - 推送错误信息
  ✅ SendConnectionStatus - 推送连接状态
```

#### 硬件测试页面（/hardware-test）
```csharp
功能:
  ✅ 电子秤连接测试
  ✅ 实时重量显示
  ✅ 去皮操作
  ✅ 扫码枪模拟测试
  ✅ 实时事件日志
  ✅ 连接状态监控
```

### 关键文件
```
src/Minimes.Infrastructure/
├── Hardware/
│   ├── ScaleService.cs           # 电子秤串口服务
│   ├── BarcodeScannerService.cs  # 扫码枪服务
│   ├── IScaleService.cs          # 电子秤接口
│   └── IBarcodeScannerService.cs # 扫码枪接口

src/Minimes.Web/
├── Hubs/
│   └── HardwareHub.cs            # SignalR Hub
└── Pages/
    └── HardwareTest.razor        # 硬件测试页面

appsettings.json:
{
  "Hardware": {
    "Scale": {
      "PortName": "COM3",
      "BaudRate": 9600,
      "Protocol": "Generic"
    }
  }
}
```

---

## 📊 阶段6：称重记录模块 ✅

### 目标
实现核心业务功能：扫码-称重-关联客户-存档

### 完成内容

#### 称重记录DTOs
```csharp
CreateWeighingRecordRequest
  - ProductId, CustomerId, Weight, ProcessStage, Remark

UpdateWeighingRecordRequest
  - Id + 可更新字段

WeighingRecordResponse
  - 完整记录信息 + 关联对象快照（ProductName, CustomerName）

WeighingRecordQueryRequest
  - 分页查询 + 多条件过滤（日期、客户、商品、阶段）
```

#### 验证器（FluentValidation）
```csharp
CreateWeighingRecordRequestValidator:
  - ProductId: 必填，>0
  - CustomerId: 必填，>0
  - Weight: 必填，>0，最多3位小数
  - ProcessStage: 有效枚举值
  - Remark: ≤500字符
```

#### 称重记录服务（IWeighingRecordService）
```csharp
✅ CreateAsync - 创建称重记录
✅ GetByIdAsync - 按ID查询
✅ UpdateAsync - 更新记录
✅ DeleteAsync - 删除记录
✅ QueryAsync - 分页查询（多条件过滤）
✅ GetLatestAsync - 获取最新N条记录
✅ GetByCustomerAsync - 按客户查询
✅ GetByProductAsync - 按商品查询
✅ GetStatisticsAsync - 统计数据
```

#### 生产报表服务（IReportService）
```csharp
✅ GetProductionReportAsync - 生产统计报表
  - 总记录数、总重量、平均重量
  - 按加工环节统计
  - 按批次统计

✅ GetProductLossRateAsync - 商品损耗率统计
  - 入库重量、加工重量、出库重量
  - 损耗重量、损耗率计算
  - 公式：(入库-出库)/入库 × 100%

✅ GetProductLossRateByIdAsync - 单个商品损耗率
```

#### 加工阶段枚举
```csharp
ProcessStage:
  - Receiving = 0   # 原料入库
  - Processing = 1  # 加工过程
  - Shipping = 2    # 成品出库
```

### 关键文件
```
src/Minimes.Application/
├── DTOs/
│   ├── WeighingRecord/
│   │   ├── CreateWeighingRecordRequest.cs
│   │   ├── UpdateWeighingRecordRequest.cs
│   │   ├── WeighingRecordResponse.cs
│   │   └── WeighingRecordQueryRequest.cs
│   └── Report/
│       ├── ProductionReportRequest.cs
│       ├── ProductionReportResponse.cs
│       └── ProductLossRateResponse.cs
├── Validators/
│   └── WeighingRecord/
│       ├── CreateWeighingRecordRequestValidator.cs
│       └── UpdateWeighingRecordRequestValidator.cs
├── Interfaces/
│   ├── IWeighingRecordService.cs
│   └── IReportService.cs
└── Services/
    ├── WeighingRecordService.cs
    └── ReportService.cs

src/Minimes.Web/Pages/
└── Reports/
    └── Production.razor              # 生产报表页面
```

---

## 📤 阶段7：Excel导出 ✅

### 目标
实现生产数据Excel导出功能

### 完成内容

#### Excel导出服务（IExcelExportService）
```csharp
✅ ExportWeighingRecordsAsync - 导出称重记录列表
✅ ExportProductionReportAsync - 导出生产报表（含损耗率）
✅ ExportProductLossRateAsync - 导出商品损耗率统计
```

#### 实现特性
```csharp
技术栈:
  - EPPlus 8.4.0（免费版）
  - NonCommercial许可

功能:
  ✅ 自动列宽调整
  ✅ 表头样式美化（加粗、背景色）
  ✅ 数据格式化（日期、数字）
  ✅ 损耗率颜色编码
    - 红色: >20%
    - 黄色: 10-20%
    - 绿色: <10%
  ✅ 多Sheet支持（统计汇总 + 详细数据）
```

#### 前端集成
```javascript
utils.js:
  - downloadFile(fileName, base64String)
  - 自动创建临时下载链接
  - Blob + URL.createObjectURL
  - 文件名动态生成：生产报表_20260108_143025.xlsx
```

#### 页面集成
```
生产报表页面（Production.razor）:
  ✅ 导出Excel按钮
  ✅ 禁用状态（无数据时）
  ✅ 触摸友好设计（btn-lg）
  ✅ 成功/失败反馈
```

### 关键文件
```
src/Minimes.Infrastructure/
└── Excel/
    ├── IExcelExportService.cs
    └── ExcelExportService.cs

src/Minimes.Web/
├── wwwroot/js/
│   └── utils.js                  # 下载工具函数
└── Pages/
    ├── _Layout.cshtml            # 引入utils.js
    └── Reports/Production.razor  # Excel导出按钮
```

---

## 🎨 阶段8：UI优化和测试 🔄 70%

### 目标
优化用户界面，实现响应式设计和平板适配

### 已完成内容

#### 用户认证界面
```
✅ Login.razor - 登录页面
  - 响应式布局（col-12 col-sm-10 col-md-8 col-lg-6 col-xl-4）
  - 记住我功能
  - 错误提示
  - 触摸友好设计

✅ Logout.razor - 登出页面
  - 自动注销
  - 重定向到登录页

✅ NavMenu.razor - 导航菜单
  - AuthorizeView集成
  - 用户信息显示
  - 登录/登出链接
```

#### 客户管理界面
```
✅ Customers/Create.razor - 新增客户
  - 表单验证
  - 响应式布局
  - 帮助侧边栏
  - 触摸友好控件（form-control-lg）

✅ Customers/Edit.razor - 编辑客户
  - 只读代码字段（保证数据一致性）
  - 客户信息面板（创建时间、更新时间、状态）
  - 响应式布局
  - 编辑说明侧边栏
```

#### 生产报表界面
```
✅ Reports/Production.razor - 生产报表
  - 统计周期选择（今天/本周/本月/自定义）
  - 加工环节筛选
  - 统计汇总卡片（响应式网格）
  - 按加工环节统计表格
  - 批次维度统计
  - 商品损耗率统计（颜色编码）
  - Excel导出功能
  - 触摸友好设计
```

#### 平板触摸优化（tablet.css）
```css
核心优化:
  ✅ 最小触摸目标: 44px × 44px（WCAG 2.1 AAA标准）
  ✅ 导航菜单: min-height 48px
  ✅ 表单控件: min-height 44px, font-size 16px（防iOS缩放）
  ✅ 按钮: btn-lg类，padding 0.75rem 1rem
  ✅ 表格: 触摸滚动优化（-webkit-overflow-scrolling: touch）
  ✅ hover效果: 触摸设备禁用（@media (hover: none)）
  ✅ 点击反馈: transform scale(0.98)
  ✅ 响应式断点:
    - 手机: <576px
    - 平板竖屏: 768-992px
    - 平板横屏: 992-1200px
    - 桌面: >1200px
```

### 待完成内容
```
⏳ 个人中心页面（Profile.razor）
⏳ 系统设置页面（Settings.razor）
⏳ 质量追溯页面链接
⏳ 单元测试
⏳ 集成测试
```

### 关键文件
```
src/Minimes.Web/
├── Pages/
│   ├── Login.razor              # 登录页面
│   ├── Logout.razor             # 登出页面
│   ├── Customers/
│   │   ├── Create.razor         # 新增客户
│   │   └── Edit.razor           # 编辑客户
│   └── Reports/
│       └── Production.razor     # 生产报表
├── Shared/
│   └── NavMenu.razor            # 导航菜单（AuthorizeView）
└── wwwroot/
    └── css/
        └── tablet.css           # 平板触摸优化样式
```

---

## 📚 阶段9：部署和文档 ⏳ 10%

### 目标
编写部署文档和用户手册

### 已完成内容
```
✅ README.md - 项目进度文档（本文档）
✅ CLAUDE.md - AI开发指引
```

### 待完成内容
```
⏳ DEPLOYMENT.md - 部署文档
⏳ 用户手册
⏳ 管理员指南
⏳ 硬件集成指南
⏳ 故障排查指南
```

---

## 🔧 技术栈总结

### V1.0 Blazor Server技术栈（已完成）
| 功能 | 技术 | 版本 | 状态 |
|------|------|------|------|
| 框架 | ASP.NET Core | 8.0 | ❌ 废弃 |
| Web | Blazor Server | 8.0 | ❌ 废弃 |
| 数据库 | SQLite | Latest | ✅ 保留 |
| ORM | Entity Framework Core | 8.0.11 | ✅ 保留 |
| 验证 | FluentValidation | 12.1.1 | ✅ 保留 |
| 映射 | AutoMapper | 12.0.1 | ✅ 保留 |
| 密码 | PBKDF2 + SHA256 | .NET Built-in | ✅ 保留 |
| 串口通信 | System.IO.Ports | 10.0.1 | ❌ 废弃 |
| 实时推送 | SignalR | 8.0.11 | ❌ 废弃 |
| Excel导出 | EPPlus | 8.4.0 | ✅ 保留 |

### V2.0 MAUI Blazor Hybrid技术栈（重构中）⚠️
| 功能 | 技术 | 版本 | 状态 |
|------|------|------|------|
| 框架 | .NET MAUI | 8.0 | ✅ 新增 |
| UI | MAUI Blazor Hybrid | 8.0 | ✅ 新增 |
| 数据库 | SQLite | Latest | ✅ 保留 |
| ORM | Entity Framework Core | 8.0.11 | ✅ 保留 |
| 验证 | FluentValidation | 12.1.1 | ✅ 保留 |
| 映射 | AutoMapper | 12.0.1 | ✅ 保留 |
| 密码 | PBKDF2 + SHA256 | .NET Built-in | ✅ 保留 |
| 安卓USB | Android.Hardware.Usb | Built-in | ✅ 新增 |
| 认证存储 | SecureStorage | MAUI Built-in | ✅ 新增 |
| Excel导出 | EPPlus | 8.4.0 | ✅ 保留 |

### 架构模式（不变）
- ✅ Clean Architecture（4层分离）
- ✅ Repository Pattern（数据访问抽象）
- ✅ Dependency Injection（松耦合）
- ✅ Factory Pattern（OAuth提供商 → 平台服务工厂）
- ✅ Value Objects（Barcode、Weight）
- ✅ Domain-Driven Design（业务驱动）

### 设计原则遵循（不变）
- ✅ SOLID原则（单一职责、开闭原则、依赖倒置等）
- ✅ KISS原则（保持简单）
- ✅ DRY原则（避免重复）
- ✅ YAGNI原则（只实现必要功能）

---

## 📊 数据库架构

### 表结构速览
```
Users (用户)
  ├─ Id, Username(唯一), PasswordHash
  ├─ FullName, Role (Operator/Admin)
  └─ IsActive, CreatedAt, UpdatedAt

Customers (客户)
  ├─ Id, Code(唯一), Name
  ├─ ContactPerson, Phone, Address
  └─ IsActive, CreatedAt, UpdatedAt

Products (商品)
  ├─ Id, Barcode(唯一), Name
  ├─ Specification, Unit, ReferencePrice
  └─ IsActive, CreatedAt, UpdatedAt

WeighingRecords (称重记录)
  ├─ Id, UserId (FK), CustomerId (FK), ProductId (FK)
  ├─ WeightValue, WeightUnit, WeighedAt
  ├─ Remark
  └─ CreatedAt, UpdatedAt

UserOAuthAccounts (OAuth账号)
  ├─ Id, UserId (FK), ProviderType
  ├─ ProviderUserId(唯一with ProviderType), ProviderName
  ├─ ProviderAvatar, RefreshToken
  └─ CreatedAt, UpdatedAt
```

### 索引优化
```sql
-- 唯一索引（业务键）
Users.UK_Username
Customers.UK_Code
Products.UK_Barcode
UserOAuthAccounts.UK_Provider

-- 性能索引（查询优化）
Customers.IX_Name
Customers.IX_IsActive
Products.IX_Name
Users.IX_IsActive
WeighingRecords.IX_CustomerId_WeighedAt_DESC
WeighingRecords.IX_WeighedAt_DESC
```

---

## 🚀 下一步规划

### V2.0 MAUI迁移计划（按优先级）⚠️

#### Phase 1：环境准备和项目创建（1天）🟢
- [ ] 安装MAUI工作负载（`dotnet workload install maui`）
- [ ] 验证Android SDK安装（API 33+）
- [ ] 创建Minimes.Maui项目（`maui-blazor`模板）
- [ ] 配置项目引用（Domain + Application + Infrastructure）
- [ ] 验证编译成功（Windows + Android目标）

#### Phase 2：数据层迁移（2天）🟢
- [ ] 保留Domain层（无需修改）✅
- [ ] 保留Application层（无需修改）✅
- [ ] 调整Infrastructure层
  - [ ] 移除ASP.NET Identity相关代码
  - [ ] 实现简化的本地认证（SQLite存储）
  - [ ] 移除SignalR相关代码
  - [ ] 移除System.IO.Ports硬件代码
  - [ ] 保留EF Core + SQLite数据访问
  - [ ] 保留Excel导出服务

#### Phase 3：UI迁移（3天）🟡
- [ ] 迁移Blazor页面组件（Login/Customers/Reports/Hardware）
- [ ] 迁移共享组件（NavMenu → MAUI Shell）
- [ ] 迁移静态资源（CSS/JS）

#### Phase 4：硬件集成重写（5天）🔴 高风险
- [ ] 定义跨平台硬件接口（IScaleService/IScannerService）
- [ ] 实现安卓硬件服务（USB OTG + 蓝牙）
- [ ] 实现Windows硬件服务（串口兼容）
- [ ] USB权限请求和设备检测
- [ ] 依赖注入配置（条件注册）

#### Phase 5：认证重写（2天）🟡
- [ ] 移除ASP.NET Cookie认证
- [ ] 实现MAUI本地认证（SecureStorage + AuthenticationStateProvider）
- [ ] 更新AuthorizeView组件

#### Phase 6：测试和调试（3天）🟡
- [ ] Windows桌面测试（全功能）
- [ ] 安卓模拟器测试（基础功能）
- [ ] 安卓真机测试（USB OTG硬件）

#### Phase 7：打包和部署（1天）🟢
- [ ] 配置应用签名（Android KeyStore）
- [ ] 生成APK/AAB包
- [ ] 安装到安卓平板测试
- [ ] 编写MAUI部署文档

**总工作量估算**：17天（3周） | **风险等级**：🔴 高难度

### V1.0 Blazor Server版本（暂停）⏸️
- [x] 登录/登出页面
- [x] 客户管理页面（Create/Edit）
- [x] 生产报表页面
- [x] 平板触摸优化
- ⏸️ 个人中心页面（暂停）
- ⏸️ 系统设置页面（暂停）
- ⏸️ 质量追溯页面链接（暂停）
- ⏸️ 单元测试（暂停）
- ⏸️ 集成测试（暂停）

---

## 💡 重要特性说明

### 1. 离线支持
- ✅ 基于Cookie认证（不依赖session存储）
- ✅ 本地SQLite数据库
- ✅ 完全离线工作能力

### 2. 多用户管理
- ✅ 管理员 vs 操作员
- ✅ 基于角色的授权
- ✅ 操作员审计日志（UserId记录）

### 3. 数据安全
- ✅ PBKDF2密码哈希
- ✅ 防时序攻击
- ✅ Cookie HttpOnly/Secure标志
- ✅ 外键约束（ON DELETE RESTRICT）

### 4. 可扩展设计
- ✅ OAuth提供商工厂模式
- ✅ 值对象模式（Barcode、Weight）
- ✅ Repository抽象层
- ✅ 接口驱动设计

---

## 🔍 编译状态

```bash
✅ 编译成功
✅ 0个警告
✅ 0个错误
✅ 4个项目全部构建通过

编译时间: <2秒
```

---

## 📝 使用说明

### 启动应用
```bash
cd src/Minimes.Web
dotnet run
```

### 默认登录凭证
```
管理员账户:
  用户名: admin
  密码: Admin123456

操作员账户:
  用户名: operator
  密码: Operator123456
```

### 数据库初始化
- 应用启动时自动创建/迁移数据库
- 自动种子数据初始化
- 数据库文件位置: `minimes.db`（SQLite）

---

## 🎯 开发规范

### 代码风格
- ✅ C# 11 新特性（nullable reference types）
- ✅ 异步编程（async/await）
- ✅ 中文注释（XML文档注释）
- ✅ PascalCase命名

### 架构规范
- ✅ Domain → Application → Infrastructure → Web
- ✅ 单向依赖
- ✅ 接口驱动设计
- ✅ DI容器管理

### 数据库规范
- ✅ 迁移命名: `[timestamp]_[Description]`
- ✅ 唯一约束优先
- ✅ 外键ON DELETE RESTRICT
- ✅ 时间戳自动管理

---

## 📚 文件结构完整图

```
D:\MyDomain\src\AI\minimes\
├── Minimes.sln                          # 解决方案文件
├── README.md                             # 本文档
│
├── src/
│   ├── Minimes.Domain/                  # 领域层
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── User.cs
│   │   │   ├── Customer.cs
│   │   │   ├── Product.cs
│   │   │   ├── WeighingRecord.cs
│   │   │   └── OAuth/UserOAuthAccount.cs
│   │   ├── Enums/
│   │   │   ├── UserRole.cs
│   │   │   ├── WeightUnit.cs
│   │   │   └── OAuthProviderType.cs
│   │   ├── ValueObjects/
│   │   │   ├── Barcode.cs
│   │   │   └── Weight.cs
│   │   ├── Interfaces/
│   │   │   ├── IRepository.cs
│   │   │   ├── IUserRepository.cs
│   │   │   ├── ICustomerRepository.cs
│   │   │   ├── IProductRepository.cs
│   │   │   ├── IWeighingRecordRepository.cs
│   │   │   └── IUserOAuthAccountRepository.cs
│   │   └── Security/
│   │       └── PasswordHashService.cs
│   │
│   ├── Minimes.Application/             # 应用层
│   │   ├── DTOs/
│   │   │   ├── User/
│   │   │   ├── Customer/
│   │   │   └── Product/
│   │   ├── Validators/
│   │   │   ├── User/
│   │   │   ├── Customer/
│   │   │   └── Product/
│   │   ├── Interfaces/
│   │   │   ├── IUserService.cs
│   │   │   ├── IAuthenticationService.cs
│   │   │   ├── ICustomerService.cs
│   │   │   ├── IProductService.cs
│   │   │   └── IOAuthProvider.cs
│   │   ├── Services/
│   │   │   ├── UserService.cs
│   │   │   ├── AuthenticationService.cs
│   │   │   ├── CustomerService.cs
│   │   │   └── ProductService.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── Minimes.Infrastructure/          # 基础设施层
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   ├── CustomerConfiguration.cs
│   │   │   │   ├── ProductConfiguration.cs
│   │   │   │   ├── WeighingRecordConfiguration.cs
│   │   │   │   └── UserOAuthAccountConfiguration.cs
│   │   │   ├── Migrations/
│   │   │   └── SeedData.cs
│   │   ├── Repositories/
│   │   │   ├── Repository.cs
│   │   │   ├── UserRepository.cs
│   │   │   ├── CustomerRepository.cs
│   │   │   ├── ProductRepository.cs
│   │   │   ├── WeighingRecordRepository.cs
│   │   │   └── UserOAuthAccountRepository.cs
│   │   ├── Authentication/
│   │   │   ├── OAuthProviderFactory.cs
│   │   │   ├── WeChatOAuthProvider.cs
│   │   │   ├── GoogleOAuthProvider.cs
│   │   │   └── OAuthUserInfo.cs
│   │   └── DependencyInjection.cs
│   │
│   └── Minimes.Web/                     # 表示层 (Blazor Server)
│       ├── Extensions/
│       │   └── AuthenticationExtensions.cs
│       ├── Pages/
│       │   ├── _Host.cshtml
│       │   └── _Layout.cshtml
│       ├── Shared/
│       ├── wwwroot/
│       ├── appsettings.json
│       ├── Program.cs
│       └── Minimes.Web.csproj
│
└── docs/                                 # 文档（待建）
    ├── 用户手册.md
    ├── 管理员指南.md
    └── 硬件集成指南.md
```

---

## 🎉 总结

### V1.0 Blazor Server版本 - 已完成的核心功能 ✅
✅ 4层架构搭建（Clean Architecture）
✅ 5个核心数据实体（User、Customer、Product、WeighingRecord、OAuth）
✅ 本地账号认证（PBKDF2安全）
✅ 第三方OAuth框架（WeChat/Google）- V2.0可能移除
✅ 客户管理CRUD + 搜索
✅ 商品管理CRUD + 条形码扫码
✅ 称重记录服务（CRUD + 查询统计）
✅ ~~电子秤串口集成（多协议支持）~~ - V2.0需重写
✅ ~~扫码枪服务（键盘监听）~~ - V2.0需重写
✅ ~~SignalR实时推送（重量/扫码）~~ - V2.0移除
✅ 生产报表统计（按环节/批次）
✅ 质量追溯（损耗率计算）
✅ Excel导出（EPPlus）
✅ 用户认证界面（Login/Logout）
✅ 客户管理页面（Create/Edit）
✅ 生产报表页面（统计+导出）
✅ 平板触摸优化（WCAG 2.1 AAA）
✅ 完整的验证和错误处理
✅ ~~离线Cookie认证支持~~ - V2.0改为SecureStorage
✅ 角色授权策略
✅ 数据库迁移和索引优化

### V2.0 MAUI版本 - 架构优势 🚀
🔐 **安全**: PBKDF2密码、SecureStorage、本地数据加密
⚡ **高效**: 原生性能、本地数据库、无网络延迟
🧩 **可扩展**: 跨平台抽象、平台特定实现、接口驱动
📱 **完全离线**: 无需服务器、无需网络、独立运行
🎯 **清晰**: Clean Architecture保留、SOLID原则、单向依赖
🔌 **硬件集成**: 安卓USB OTG、蓝牙、Windows串口兼容
📊 **数据分析**: 生产报表、损耗率计算、质量追溯、Excel导出
📱 **触摸友好**: MAUI原生控件、平板优化、手势支持
🌍 **跨平台**: 支持Android、Windows、iOS（可选）

### 核心挑战 ⚠️
🔴 **USB OTG硬件集成**：安卓USB通信复杂度高
🟡 **电子秤协议解析**：USB数据格式可能不同
🟡 **认证机制重写**：SecureStorage跨平台处理
🟢 **UI迁移**：Blazor组件可复用，工作量小

---

**最后更新**: 2026-01-12 | **开发者**: 老王技术流 | **许可**: MIT

**架构状态**: V1.0 Blazor Server完成78% → V2.0 MAUI Blazor Hybrid重构中（预计17天）
