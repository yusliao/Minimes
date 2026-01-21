# 📦 MiniMES 记账系统 - 部署文档

> **版本**: v1.0.1
> **更新日期**: 2026-01-18
> **适用平台**: Windows / Linux / macOS

---

## 📋 目录

1. [系统要求](#系统要求)
2. [部署前准备](#部署前准备)
3. [Windows部署](#windows部署)
4. [Linux部署](#linux部署)
5. [macOS部署](#macos部署)
6. [配置说明](#配置说明)
7. [数据库初始化](#数据库初始化)
8. [硬件设备配置](#硬件设备配置)
9. [启动和验证](#启动和验证)
10. [故障排查](#故障排查)
11. [常见问题](#常见问题)

---

## 系统要求

### 最低配置

| 组件 | 要求 |
|------|------|
| **操作系统** | Windows 10/11、Ubuntu 20.04+ 或 macOS 11+ |
| **CPU** | 双核 2.0 GHz |
| **内存** | 2 GB RAM |
| **硬盘** | 500 MB 可用空间 |
| **框架** | .NET 8.0 Runtime |
| **数据库** | SQLite（内置） |

### 推荐配置

| 组件 | 推荐 |
|------|------|
| **操作系统** | Windows Server 2019+ 或 Ubuntu 22.04 LTS |
| **CPU** | 四核 2.5 GHz |
| **内存** | 4 GB RAM |
| **硬盘** | 2 GB 可用空间（SSD） |
| **框架** | .NET 8.0 Runtime |

### 硬件外设（可选）

- **电子秤**: 支持串口通信（RS-232）的电子秤
- **扫码枪**: USB接口或无线扫码枪（模拟键盘输入）

---

## 部署前准备

### 1. 安装 .NET 8.0 Runtime

#### Windows:
```powershell
# 下载 .NET 8.0 Runtime (ASP.NET Core)
# 访问: https://dotnet.microsoft.com/download/dotnet/8.0
# 选择: ASP.NET Core Runtime 8.0.x - Windows Hosting Bundle

# 安装后验证
dotnet --list-runtimes
```

#### Linux (Ubuntu):
```bash
# 添加 Microsoft 包仓库
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# 安装 ASP.NET Core Runtime
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-8.0

# 验证安装
dotnet --list-runtimes
```

### 2. 下载应用程序

从GitHub Release下载最新版本的发布包：

```bash
# Windows
minimes-win-x64-v1.0.0.zip

# Linux
minimes-linux-x64-v1.0.0.tar.gz
```

---

## Windows部署

### 步骤1：解压应用程序

```powershell
# 解压到目标目录（例如：C:\MiniMES）
Expand-Archive -Path minimes-win-x64-v1.0.0.zip -DestinationPath C:\MiniMES
cd C:\MiniMES
```

### 步骤2：配置应用设置

编辑 `appsettings.json` 文件：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=minimes.db"
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5000"
      }
    }
  },
  "Hardware": {
    "Scale": {
      "PortName": "COM3",
      "BaudRate": 9600,
      "Protocol": "Generic"
    }
  }
}
```

### 步骤3：运行应用程序

#### 方式A：直接运行（测试用）

```powershell
.\Minimes.Web.exe
```

#### 方式B：注册为Windows服务（生产环境推荐）

```powershell
# 使用 NSSM (Non-Sucking Service Manager)
# 下载 NSSM: https://nssm.cc/download

# 安装服务
nssm install MiniMES "C:\MiniMES\Minimes.Web.exe"

# 配置服务
nssm set MiniMES AppDirectory "C:\MiniMES"
nssm set MiniMES DisplayName "MiniMES 记账系统"
nssm set MiniMES Description "MiniMES 生产记账系统 - 扫码称重管理"
nssm set MiniMES Start SERVICE_AUTO_START

# 启动服务
nssm start MiniMES

# 查看服务状态
nssm status MiniMES
```

#### 方式C：使用 sc 命令（Windows原生）

```powershell
# 创建服务
sc create MiniMES binPath= "C:\MiniMES\Minimes.Web.exe" start= auto DisplayName= "MiniMES记账系统"

# 启动服务
sc start MiniMES

# 查询服务状态
sc query MiniMES
```

### 步骤4：配置防火墙

```powershell
# 允许端口5000通过防火墙
New-NetFirewallRule -DisplayName "MiniMES HTTP" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

---

## Linux部署

### 步骤1：解压应用程序

```bash
# 创建应用目录
sudo mkdir -p /opt/minimes
cd /opt/minimes

# 解压
sudo tar -xzf /path/to/minimes-linux-x64-v1.0.0.tar.gz

# 设置权限
sudo chmod +x Minimes.Web
```

### 步骤2：配置应用设置

编辑 `appsettings.json`：

```bash
sudo nano appsettings.json
```

修改内容（同Windows配置）。

### 步骤3：创建 systemd 服务

创建服务文件：

```bash
sudo nano /etc/systemd/system/minimes.service
```

服务文件内容：

```ini
[Unit]
Description=MiniMES 记账系统
After=network.target

[Service]
Type=notify
WorkingDirectory=/opt/minimes
ExecStart=/opt/minimes/Minimes.Web
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=minimes
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

### 步骤4：启动服务

```bash
# 重新加载 systemd 配置
sudo systemctl daemon-reload

# 启用开机自启
sudo systemctl enable minimes

# 启动服务
sudo systemctl start minimes

# 查看服务状态
sudo systemctl status minimes

# 查看日志
sudo journalctl -u minimes -f
```

### 步骤5：配置反向代理（可选）

#### 使用 Nginx:

```bash
# 安装 Nginx
sudo apt-get install nginx

# 创建配置文件
sudo nano /etc/nginx/sites-available/minimes
```

Nginx配置：

```nginx
server {
    listen 80;
    server_name minimes.yourdomain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

启用站点：

```bash
# 创建软链接
sudo ln -s /etc/nginx/sites-available/minimes /etc/nginx/sites-enabled/

# 测试配置
sudo nginx -t

# 重启 Nginx
sudo systemctl restart nginx
```

---

## macOS部署

### 步骤1：发布应用程序

#### 1. 确定目标架构

在 macOS 服务器上运行以下命令确认芯片类型：

```bash
# 查看系统架构
uname -m

# 输出 arm64 → 使用 osx-arm64（M1/M2/M3 芯片）
# 输出 x86_64 → 使用 osx-x64（Intel 芯片）
```

#### 2. 发布命令（在开发机上执行）

```bash
# 进入项目根目录
cd D:\MyDomain\src\AI\minimes

# 发布 ARM64 版本（M1/M2/M3 芯片）
dotnet publish src/Minimes.Web/Minimes.Web.csproj \
  -c Release \
  -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish/osx-arm64

# 或者发布 x64 版本（Intel 芯片）
dotnet publish src/Minimes.Web/Minimes.Web.csproj \
  -c Release \
  -r osx-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish/osx-x64
```

**参数说明：**

| 参数 | 作用 |
|-----|------|
| `-c Release` | 生产优化版本 |
| `-r osx-arm64` | 目标运行时（根据服务器芯片选择） |
| `--self-contained true` | 包含 .NET Runtime，服务器无需安装 .NET |
| `-p:PublishSingleFile=true` | 打包成单文件，方便部署 |
| `-p:IncludeNativeLibrariesForSelfExtract=true` | 包含原生库（SQLite 需要） |

### 步骤2：上传到服务器

#### 1. 打包发布文件

```bash
# 在开发机上，进入发布目录
cd publish/osx-arm64

# 打包成 tar.gz
tar -czf minimes-macos.tar.gz *
```

#### 2. 上传到服务器

```bash
# 使用 scp 上传（替换成实际的服务器地址）
scp minimes-macos.tar.gz user@your-mac-server:/tmp/
```

#### 3. 在服务器上解压

```bash
# SSH 登录到 macOS 服务器
ssh user@your-mac-server

# 创建部署目录
sudo mkdir -p /opt/minimes
sudo chown $USER /opt/minimes

# 解压文件
cd /opt/minimes
tar -xzf /tmp/minimes-macos.tar.gz

# 添加执行权限
chmod +x Minimes.Web

# 创建必要目录
mkdir -p data logs
```

### 步骤3：配置应用设置

编辑 `appsettings.json` 文件：

```bash
nano /opt/minimes/appsettings.json
```

**关键配置项：**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/opt/minimes/data/minimes.db"
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5000"
      }
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Hardware": {
    "Scale": {
      "PortName": "",
      "BaudRate": 9600,
      "Protocol": "Generic"
    }
  }
}
```

**配置说明：**
- `ConnectionStrings`：数据库路径使用绝对路径
- `Kestrel.Endpoints`：监听所有网卡的 5000 端口
- `Hardware.Scale.PortName`：留空表示不使用硬件设备

### 步骤4：配置 launchd 系统服务

#### 1. 创建 launchd 配置文件

```bash
sudo nano /Library/LaunchDaemons/com.minimes.web.plist
```

**配置文件内容：**

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>com.minimes.web</string>

    <key>ProgramArguments</key>
    <array>
        <string>/opt/minimes/Minimes.Web</string>
        <string>--urls</string>
        <string>http://0.0.0.0:5000</string>
    </array>

    <key>WorkingDirectory</key>
    <string>/opt/minimes</string>

    <key>RunAtLoad</key>
    <true/>

    <key>KeepAlive</key>
    <true/>

    <key>StandardOutPath</key>
    <string>/opt/minimes/logs/stdout.log</string>

    <key>StandardErrorPath</key>
    <string>/opt/minimes/logs/stderr.log</string>

    <key>EnvironmentVariables</key>
    <dict>
        <key>ASPNETCORE_ENVIRONMENT</key>
        <string>Production</string>
    </dict>
</dict>
</plist>
```

**配置项说明：**

| 配置项 | 作用 |
|-------|------|
| `Label` | 服务唯一标识 |
| `ProgramArguments` | 启动命令和参数 |
| `WorkingDirectory` | 工作目录 |
| `RunAtLoad` | 开机自启 |
| `KeepAlive` | 崩溃后自动重启 |
| `StandardOutPath` | 标准输出日志 |
| `StandardErrorPath` | 错误日志 |

#### 2. 验证配置文件

```bash
# 检查 plist 文件语法
plutil -lint /Library/LaunchDaemons/com.minimes.web.plist
```

### 步骤5：启动和管理服务

#### 1. 加载并启动服务

```bash
# 加载服务配置
sudo launchctl load /Library/LaunchDaemons/com.minimes.web.plist

# 启动服务
sudo launchctl start com.minimes.web
```

#### 2. 常用管理命令

```bash
# 查看服务状态
sudo launchctl list | grep minimes

# 停止服务
sudo launchctl stop com.minimes.web

# 重启服务
sudo launchctl stop com.minimes.web
sudo launchctl start com.minimes.web

# 卸载服务
sudo launchctl unload /Library/LaunchDaemons/com.minimes.web.plist
```

#### 3. 查看日志

```bash
# 查看标准输出日志
tail -f /opt/minimes/logs/stdout.log

# 查看错误日志
tail -f /opt/minimes/logs/stderr.log

# 查看最近100行日志
tail -n 100 /opt/minimes/logs/stdout.log
```

### 步骤6：验证部署

#### 1. 检查服务运行状态

```bash
# 检查服务是否运行
sudo launchctl list | grep minimes

# 检查进程
ps aux | grep Minimes.Web

# 检查端口监听
lsof -i :5000
```

#### 2. 测试 HTTP 访问

```bash
# 本地测试
curl http://localhost:5000

# 浏览器访问
# http://localhost:5000
# http://服务器IP:5000
```

---

## 配置说明

### appsettings.json 完整配置

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=minimes.db"
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5000"
      }
    }
  },
  "Hardware": {
    "Scale": {
      "PortName": "COM3",
      "BaudRate": 9600,
      "DataBits": 8,
      "StopBits": "One",
      "Parity": "None",
      "Protocol": "Generic",
      "ReadTimeout": 2000
    }
  },
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

### 配置项说明

| 配置项 | 说明 | 默认值 |
|--------|------|--------|
| `ConnectionStrings:DefaultConnection` | 数据库连接字符串 | `Data Source=minimes.db` |
| `Kestrel:Endpoints:Http:Url` | HTTP监听地址 | `http://0.0.0.0:5000` |
| `Hardware:Scale:PortName` | 电子秤串口号（Windows: COM3, Linux: /dev/ttyUSB0） | `COM3` |
| `Hardware:Scale:BaudRate` | 波特率 | `9600` |
| `Hardware:Scale:Protocol` | 协议（Generic/Toledo/Mettler） | `Generic` |

---

## 数据库初始化

应用程序首次启动时会自动执行以下操作：

1. ✅ **创建数据库文件** (`minimes.db`)
2. ✅ **执行数据库迁移** (创建所有表和索引)
3. ✅ **初始化种子数据** (默认管理员账户)

### 默认账户

| 角色 | 用户名 | 密码 | 说明 |
|------|--------|------|------|
| 管理员 | `admin` | `Admin123456` | 拥有所有权限 |
| 操作员 | `operator` | `Operator123456` | 只能操作称重记录 |
| 演示账户 | `demo` | `demo123` | 管理员权限，自动启用硬件模拟模式 |

**⚠️ 重要：首次登录后请立即修改默认密码！**

### 演示模式说明

使用 `demo` 账户登录时，系统会自动启用**硬件模拟模式**：

- **电子秤模拟**: 自动推送波动的重量值，模拟真实秤的抖动和稳定过程
- **扫码枪模拟**: 从数据库已有商品中随机选择条码推送

演示模式适用于：
- 无硬件环境下的功能演示
- 新用户熟悉系统操作流程
- 销售演示和培训

---

## 硬件设备配置

### 电子秤配置

#### 1. 确认串口号

**Windows:**
```powershell
# 打开设备管理器
devmgmt.msc

# 查看 "端口(COM和LPT)" → 找到电子秤设备（例如：COM3）
```

**Linux:**
```bash
# 列出所有串口设备
ls /dev/tty*

# 常见串口设备
# /dev/ttyUSB0  - USB转串口
# /dev/ttyS0    - 板载串口
# /dev/ttyACM0  - ACM设备

# 赋予权限（根据实际设备调整）
sudo chmod 666 /dev/ttyUSB0

# 或添加当前用户到dialout组
sudo usermod -a -G dialout $USER
# 注销后重新登录生效
```

#### 2. 测试串口通信

**Windows (使用PuTTY):**
```
下载PuTTY → 选择Serial → 设置COM口和波特率 → 打开连接
观察电子秤是否有数据输出
```

**Linux (使用minicom):**
```bash
# 安装minicom
sudo apt-get install minicom

# 配置并连接
sudo minicom -s
# 选择 "Serial port setup"
# 设置: /dev/ttyUSB0, 9600 8N1, No flow control

# 连接后观察数据输出
```

#### 3. 修改配置文件

根据测试结果修改 `appsettings.json`：

```json
{
  "Hardware": {
    "Scale": {
      "PortName": "COM3",        // Windows: COMx, Linux: /dev/ttyUSBx
      "BaudRate": 9600,           // 根据电子秤手册设置
      "Protocol": "Generic"       // Generic/Toledo/Mettler
    }
  }
}
```

### 扫码枪配置

扫码枪通常模拟键盘输入，无需特殊配置。只需确保：

1. ✅ USB接口连接正常
2. ✅ 扫码后自动回车（配置扫码枪添加后缀）
3. ✅ 浏览器聚焦在输入框

---

## 启动和验证

### 1. 访问应用程序

打开浏览器，访问：

```
http://localhost:5000
http://服务器IP:5000
```

### 2. 登录系统

使用默认账户登录：
- 用户名：`admin`
- 密码：`Admin123456`

### 3. 功能验证清单

- [ ] 用户登录成功
- [ ] 导航菜单正常显示
- [ ] 客户管理CRUD功能正常
- [ ] 商品管理CRUD功能正常
- [ ] 硬件测试页面正常（/hardware-test）
- [ ] 电子秤连接成功（如有硬件）
- [ ] 扫码枪识别正常（如有硬件）
- [ ] 生产报表数据显示正常
- [ ] 质量追溯功能正常（/reports/tracing）
- [ ] Excel导出功能正常
- [ ] 演示模式正常（使用demo账户登录）

---

## 故障排查

### 问题1：应用程序无法启动

**症状**: 双击exe无反应，或服务启动失败

**排查步骤**:

```powershell
# 1. 检查.NET Runtime版本
dotnet --list-runtimes
# 确保有 Microsoft.AspNetCore.App 8.0.x

# 2. 查看错误日志
# Windows: C:\MiniMES\logs\
# Linux: /opt/minimes/logs/ 或 journalctl -u minimes

# 3. 检查端口占用
netstat -ano | findstr :5000    # Windows
sudo lsof -i :5000              # Linux

# 4. 手动启动查看错误
cd C:\MiniMES
.\Minimes.Web.exe
```

**解决方案**:
- 安装正确版本的.NET Runtime
- 修改端口配置（避免冲突）
- 检查文件权限

### 问题2：电子秤无法连接

**症状**: 硬件测试页面显示连接失败

**排查步骤**:

```bash
# 1. 检查串口设备是否存在
# Windows: 设备管理器查看COM口
# Linux:
ls -l /dev/ttyUSB*

# 2. 检查串口权限（Linux）
sudo chmod 666 /dev/ttyUSB0

# 3. 检查串口是否被占用
# Windows:
# 打开任务管理器，结束可能占用串口的进程

# Linux:
sudo lsof /dev/ttyUSB0

# 4. 测试串口通信
# 使用 PuTTY (Windows) 或 minicom (Linux) 手动连接
```

**解决方案**:
- 确认硬件连接正常
- 核对串口号和波特率
- 修改 `appsettings.json` 配置
- 重启应用程序

### 问题3：数据库文件损坏

**症状**: 应用启动后报数据库错误

**解决方案**:

```powershell
# 1. 备份现有数据库
copy minimes.db minimes.db.backup

# 2. 删除数据库文件
rm minimes.db

# 3. 重启应用（自动重建数据库）
# 注意：所有数据将丢失，仅保留默认账户
```

### 问题4：浏览器无法访问

**排查步骤**:

```bash
# 1. 检查应用是否运行
# Windows: 任务管理器查看 Minimes.Web.exe
# Linux: systemctl status minimes

# 2. 检查端口监听
netstat -ano | findstr :5000    # Windows
sudo netstat -tulpn | grep 5000 # Linux

# 3. 检查防火墙
# Windows: 控制面板 → 防火墙 → 允许的应用
# Linux: sudo ufw status

# 4. Ping测试
ping 服务器IP
```

**解决方案**:
- 启动应用服务
- 开放防火墙端口
- 检查网络连接

### 问题5：macOS 特有问题

#### 5.1 launchd 服务无法启动

**症状**: `launchctl list` 看不到服务

**排查步骤**:

```bash
# 1. 检查 plist 文件语法
plutil -lint /Library/LaunchDaemons/com.minimes.web.plist

# 2. 查看系统日志
sudo log show --predicate 'process == "launchd"' --last 5m

# 3. 手动运行测试
cd /opt/minimes
./Minimes.Web --urls "http://localhost:5000"

# 4. 检查文件权限
ls -la /opt/minimes/Minimes.Web
```

**解决方案**:
- 修正 plist 文件语法错误
- 确保可执行文件有执行权限（`chmod +x`）
- 检查工作目录是否存在

#### 5.2 权限问题（macOS 安全机制）

**症状**: 提示"无法验证开发者"或"文件已损坏"

**解决方案**:

```bash
# 移除隔离属性
xattr -d com.apple.quarantine /opt/minimes/Minimes.Web

# 或者移除所有扩展属性
xattr -cr /opt/minimes/
```

#### 5.3 数据库权限问题

**症状**: 日志中出现 SQLite 权限错误

**解决方案**:

```bash
# 检查数据库目录权限
ls -ld /opt/minimes/data

# 修改权限
chmod 755 /opt/minimes/data
chmod 644 /opt/minimes/data/minimes.db
```

---

## 常见问题

### Q1: 支持哪些操作系统？

**A**:
- Windows 10/11、Windows Server 2016+
- Linux (Ubuntu 20.04+, CentOS 8+, Debian 10+)
- macOS 11+ (已支持，使用 launchd 系统服务)

### Q2: 数据库文件在哪里？

**A**:
- 默认位置：应用程序根目录下的 `minimes.db`
- 可在 `appsettings.json` 中修改路径

### Q3: 如何备份数据？

**A**:
```bash
# 停止应用服务
# Windows:
nssm stop MiniMES

# Linux:
sudo systemctl stop minimes

# 复制数据库文件
copy minimes.db backup/minimes-20260108.db

# 重启应用
```

### Q4: 如何更新到新版本？

**A**:
```bash
# 1. 备份数据库
# 2. 停止服务
# 3. 替换应用程序文件（保留appsettings.json和minimes.db）
# 4. 重启服务
```

### Q5: 支持多台电脑同时使用吗？

**A**:
- 是的，部署在服务器后，局域网内所有电脑都可以通过浏览器访问
- 访问地址：`http://服务器IP:5000`

### Q6: 忘记管理员密码怎么办？

**A**:
```bash
# 删除数据库文件，重建后恢复默认账户
# 注意：所有数据将丢失
rm minimes.db
# 重启应用
```

### Q7: 如何启用HTTPS？

**A**:
```json
// 修改 appsettings.json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://0.0.0.0:5001",
        "Certificate": {
          "Path": "certificate.pfx",
          "Password": "your-password"
        }
      }
    }
  }
}
```

### Q8: 电子秤数据格式不对怎么办？

**A**:
- 尝试切换协议（Generic/Toledo/Mettler）
- 联系厂商确认通信协议
- 查看硬件测试页面的原始数据

---

## 📞 技术支持

- **GitHub**: https://github.com/yourusername/minimes
- **Issues**: https://github.com/yourusername/minimes/issues
- **Email**: support@minimes.com

---

**最后更新**: 2026-01-18 | **版本**: v1.0.1 | **许可**: MIT
