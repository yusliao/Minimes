# 🖥️ MiniMES 记账系统 - Windows 11 部署指南（人话版）

> **版本**: v1.0.0
> **更新日期**: 2026-01-29
> **适用平台**: Windows 11 单机环境
> **部署难度**: ⭐⭐☆☆☆ (简单，跟着做就行)

---

## 📋 写在前面的话

艹，老王我知道你们看技术文档头疼！所以这次我用**人话**给你们写清楚，保证你婆娘都能看懂。

**这个文档分两部分**：
- **第一部分**：你已经有打包好的程序，直接装就行（适合普通用户）
- **第二部分**：你要从源代码自己打包（适合技术人员或者想学习的）

**你该看哪部分？**
- 如果你只是想用这个系统，**看第一部分就够了**
- 如果你是开发人员，或者想自己改代码，**看第二部分**

---

## 📋 目录

### 第一部分：使用已打包程序部署（推荐普通用户）
1. [系统要求](#系统要求)
2. [安装MySQL数据库](#安装mysql数据库)
3. [部署应用程序](#部署应用程序)
4. [硬件设备配置](#硬件设备配置)
5. [功能验证](#功能验证)
6. [常见问题排查](#常见问题排查)
7. [日常维护](#日常维护)

### 第二部分：从源代码打包部署（技术人员专用）
8. [开发环境准备](#开发环境准备)
9. [编译和打包](#编译和打包)
10. [打包成安装程序](#打包成安装程序)

### 附录
11. [快速参考卡](#快速参考卡)

---

# 第一部分：使用已打包程序部署

## 系统要求

### 💻 电脑配置要求（人话版）

| 项目 | 最低要求 | 推荐配置 | 老王的话 |
|------|---------|---------|---------|
| **操作系统** | Windows 11 家庭版/专业版 | Windows 11 专业版 | 家庭版也能用，别担心 |
| **处理器** | 双核 2.0 GHz | 四核 2.5 GHz 及以上 | 现在的电脑基本都够用 |
| **内存** | 4 GB RAM | 8 GB RAM 及以上 | 4GB能跑，但8GB更流畅 |
| **硬盘空间** | 2 GB 可用空间 | 5 GB 可用空间（SSD更佳） | 包括数据库和日志文件 |
| **屏幕分辨率** | 1366 x 768 | 1920 x 1080 及以上 | 分辨率太低按钮会挤在一起 |
| **网络** | 无需联网（单机运行） | 局域网（可选，多设备访问） | 不联网也能用 |

> 💡 **老王提示**：如果你的电脑是最近5年买的，基本都能跑。别担心配置问题。

---

### 🔌 硬件设备（可选，不是必须的）

系统支持以下硬件设备，但**没有也能用**：

| 设备类型 | 说明 | 是否必需 | 老王的话 |
|---------|------|---------|---------|
| **电子秤（串口）** | 支持RS-232串口通信的电子秤 | ❌ 可选 | 有就接上，没有用演示模式 |
| **电子秤（WiFi）** | 支持WiFi网络的电子秤（如A&D UC-1000WF） | ❌ 可选 | WiFi秤更方便，不用插线 |
| **扫码枪** | USB接口扫码枪（模拟键盘输入） | ❌ 可选 | 淘宝几十块钱就能买到 |

> 💡 **老王提示**：没有硬件设备也可以使用！系统提供**演示模式**，可以模拟电子秤和扫码枪的功能。先用演示模式熟悉系统，再决定要不要买硬件。

---

## 安装MySQL数据库

### 为什么要装MySQL？

MiniMES系统需要一个数据库来存储数据（客户信息、商品信息、称重记录等）。MySQL是一个免费的数据库软件，很多公司都在用，稳定可靠。

> 💡 **老王的比喻**：数据库就像一个超级大的Excel表格，但比Excel更快、更稳定、能存更多数据。

---

### 步骤1：下载MySQL安装包

#### 1.1 访问MySQL官网

打开浏览器，访问：

```
https://dev.mysql.com/downloads/mysql/
```

#### 1.2 选择Windows版本

1. 在页面中找到 **"MySQL Community Server"**
2. 点击 **"Go to Download Page"** 按钮
3. 选择 **"Windows (x86, 64-bit), ZIP Archive"**（大约300MB）

> ⚠️ **注意**：不要选择MSI安装包，选择ZIP压缩包更灵活。

#### 1.3 下载文件

1. 点击 **"Download"** 按钮
2. 页面会提示你注册账号，**直接点击底部的 "No thanks, just start my download"**（不用注册）
3. 等待下载完成（文件名类似：`mysql-8.0.36-winx64.zip`）

> 💡 **老王提示**：下载速度慢的话，可以用迅雷或者IDM下载工具。

---

### 步骤2：安装MySQL

#### 2.1 解压文件

1. 右键点击下载的 `mysql-8.0.36-winx64.zip` 文件
2. 选择 **"全部解压缩..."**
3. 解压到：`C:\MySQL`（建议用这个路径，后面配置方便）

**解压后的目录结构**：

```
C:\MySQL\
├── bin\              # 可执行文件（重要！）
├── data\             # 数据库文件（首次启动自动创建）
├── docs\             # 文档
└── ... (其他文件)
```

#### 2.2 创建配置文件

1. 打开记事本（按 `Win + R`，输入 `notepad`）
2. 复制以下内容到记事本：

```ini
[mysqld]
# 设置MySQL的安装目录（改成你实际的路径）
basedir=C:/MySQL
# 设置MySQL数据库的数据存放目录（改成你实际的路径）
datadir=C:/MySQL/data
# 设置端口号（默认3306，我们改成3371避免冲突）
port=3371
# 设置字符集（支持中文和Emoji）
character-set-server=utf8mb4
# 设置默认存储引擎
default-storage-engine=INNODB
# 设置最大连接数
max_connections=200
# 允许的最大数据包大小
max_allowed_packet=16M

[mysql]
# 设置mysql客户端默认字符集
default-character-set=utf8mb4

[client]
# 设置客户端端口号
port=3371
default-character-set=utf8mb4
```

3. 点击 **"文件"** → **"另存为..."**
4. 保存位置：`C:\MySQL\my.ini`
5. **文件类型选择 "所有文件"**（重要！不要保存成.txt）
6. 点击 **"保存"**

> ⚠️ **重要**：文件名必须是 `my.ini`，不是 `my.ini.txt`！如果看不到文件扩展名，在文件资源管理器中点击 **"查看"** → 勾选 **"文件扩展名"**。

> 💡 **老王解释**：端口号改成3371是为了避免和其他MySQL冲突。如果你电脑上已经装了MySQL（比如用XAMPP或者PHPStudy），用3371就不会打架。

---

### 步骤3：初始化MySQL

#### 3.1 以管理员身份打开命令提示符

1. 按下 `Win + X` 键
2. 选择 **"Windows PowerShell (管理员)"** 或 **"命令提示符 (管理员)"**
3. 如果弹出用户账户控制提示，点击 **"是"**

#### 3.2 初始化数据库

在命令提示符中输入以下命令（一行一行输入）：

```powershell
# 进入MySQL的bin目录
cd C:\MySQL\bin

# 初始化数据库（会生成一个临时密码，记下来！）
mysqld --initialize --console
```

**预期输出**（重要！）：

```
[Note] A temporary password is generated for root@localhost: Abc123!@#Xyz
```

> ⚠️ **超级重要**：看到类似 `root@localhost: Abc123!@#Xyz` 这样的输出，**把冒号后面的密码记下来**！这是MySQL的临时密码，等会儿要用。

> 💡 **老王提示**：如果你没记住密码，别慌！删除 `C:\MySQL\data` 文件夹，重新执行初始化命令就行。

---

### 步骤4：安装MySQL服务

继续在命令提示符中输入：

```powershell
# 安装MySQL服务（服务名叫MySQL3371，避免和其他MySQL冲突）
mysqld --install MySQL3371

# 启动MySQL服务
net start MySQL3371
```

**预期输出**：

```
Service successfully installed.
MySQL3371 服务正在启动 .
MySQL3371 服务已经启动成功。
```

> ✅ **成功标志**：看到"服务已经启动成功"，说明MySQL安装成功！

---

### 步骤5：修改root密码

#### 5.1 登录MySQL

在命令提示符中输入：

```powershell
# 登录MySQL（会提示输入密码）
mysql -u root -p -P 3371
```

**提示输入密码时**，输入刚才记下的临时密码（例如：`Abc123!@#Xyz`）

> 💡 **老王提示**：输入密码时屏幕上不会显示任何字符，这是正常的！输完直接按回车就行。

#### 5.2 修改密码

登录成功后，会看到 `mysql>` 提示符。输入以下命令：

```sql
-- 修改root密码为 root（简单好记）
ALTER USER 'root'@'localhost' IDENTIFIED BY 'root';

-- 刷新权限
FLUSH PRIVILEGES;

-- 退出MySQL
EXIT;
```

> ⚠️ **安全提示**：这里为了方便，密码设置成 `root`。如果是生产环境，请设置复杂密码！

---

### 步骤6：创建数据库

重新登录MySQL（这次用新密码 `root`）：

```powershell
mysql -u root -p -P 3371
```

输入密码：`root`

登录成功后，输入以下命令：

```sql
-- 创建数据库（名字叫minimes）
CREATE DATABASE minimes CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 查看数据库是否创建成功
SHOW DATABASES;

-- 退出MySQL
EXIT;
```

**预期输出**：

```
+--------------------+
| Database           |
+--------------------+
| information_schema |
| minimes            |  <-- 看到这个就对了
| mysql              |
| performance_schema |
| sys                |
+--------------------+
```

> ✅ **成功标志**：看到 `minimes` 数据库，说明创建成功！

---

### 步骤7：验证MySQL安装

在命令提示符中输入：

```powershell
# 查看MySQL服务状态
sc query MySQL3371
```

**预期输出**：

```
STATE              : 4  RUNNING
```

> ✅ **验证成功**：看到 `RUNNING`，说明MySQL正在运行！

---

### MySQL常用命令速查

| 命令 | 说明 |
|------|------|
| `net start MySQL3371` | 启动MySQL服务 |
| `net stop MySQL3371` | 停止MySQL服务 |
| `sc query MySQL3371` | 查看MySQL服务状态 |
| `mysql -u root -p -P 3371` | 登录MySQL |

> 💡 **老王提示**：MySQL服务会开机自动启动，不用每次手动启动。

---

## 部署应用程序

### 获取应用程序

有两种方式获取应用程序：

#### 方式A：使用已打包的程序（推荐）

联系技术支持获取最新版本的发布包：

```
minimes-win-x64-v1.0.0.zip
```

> 📧 **技术支持邮箱**：support@minimes.com

#### 方式B：自己从源代码打包

如果你是开发人员，想自己打包，请跳到 [第二部分：从源代码打包部署](#开发环境准备)。

---

### 安装应用程序

#### 步骤1：解压文件

1. 右键点击下载的 `minimes-win-x64-v1.0.0.zip` 文件
2. 选择 **"全部解压缩..."**
3. 解压到：`C:\MiniMES`

**解压后的目录结构**：

```
C:\MiniMES\
├── Minimes.Web.exe          # 主程序（双击运行）
├── appsettings.json         # 配置文件（重要！）
└── ... (其他系统文件)
```

---

#### 步骤2：配置数据库连接

1. 用记事本打开 `C:\MiniMES\appsettings.json`
2. 找到 `"Database"` 和 `"ConnectionStrings"` 部分
3. 确认配置如下：

```json
{
  "Database": {
    "Provider": "MySQL"
  },
  "ConnectionStrings": {
    "MySqlConnection": "Server=127.0.0.1;Port=3371;Database=minimes;User=root;Password=root;CharSet=utf8mb4;"
  }
}
```

4. 保存文件（`Ctrl + S`）

> 💡 **老王解释**：
> - `Server=127.0.0.1`：数据库在本机（127.0.0.1就是本机的意思）
> - `Port=3371`：MySQL的端口号（我们刚才设置的）
> - `Database=minimes`：数据库名称（我们刚才创建的）
> - `User=root`：MySQL用户名
> - `Password=root`：MySQL密码（我们刚才设置的）

> ⚠️ **重要**：如果你的MySQL密码不是 `root`，记得改成你自己的密码！

---

#### 步骤3：安装.NET 8.0运行环境

MiniMES系统需要.NET 8.0运行环境支持。

##### 3.1 下载.NET 8.0 Runtime

访问微软官方下载页面：

```
https://dotnet.microsoft.com/download/dotnet/8.0
```

##### 3.2 选择正确的安装包

在下载页面找到 **"ASP.NET Core Runtime 8.0.x"** 部分，选择：

```
Windows x64 - Hosting Bundle
```

> 📥 **文件名示例**：`dotnet-hosting-8.0.1-win.exe`（版本号可能不同）

##### 3.3 安装步骤

1. 双击下载的安装包
2. 点击 **"Install"** 按钮
3. 等待安装完成（约1-2分钟）
4. 点击 **"Close"** 关闭安装程序

##### 3.4 验证安装

按下 `Win + R` 键，输入 `cmd`，打开命令提示符，输入以下命令：

```powershell
dotnet --list-runtimes
```

**预期输出**（版本号可能不同）：

```
Microsoft.AspNetCore.App 8.0.1 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
Microsoft.NETCore.App 8.0.1 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
```

> ✅ **验证成功**：如果看到类似上面的输出，说明安装成功！
> ❌ **验证失败**：如果提示"dotnet不是内部或外部命令"，请重新安装或重启电脑后再试。

---

#### 步骤4：首次启动应用程序

##### 4.1 启动应用

**方法1：双击运行（推荐）**

1. 打开文件夹 `C:\MiniMES`
2. 双击 `Minimes.Web.exe` 文件
3. 等待命令行窗口出现（约5-10秒）
4. 看到以下提示说明启动成功：

```
Now listening on: http://0.0.0.0:5000
Application started. Press Ctrl+C to shut down.
```

> 💡 **老王提示**：不要关闭这个黑色的命令行窗口！关闭窗口会停止系统运行。

**方法2：命令行运行**

1. 按下 `Win + R` 键，输入 `cmd`，打开命令提示符
2. 输入以下命令：

```powershell
cd C:\MiniMES
Minimes.Web.exe
```

##### 4.2 首次启动会自动初始化

系统首次启动时会自动执行以下操作：

1. ✅ 连接MySQL数据库
2. ✅ 创建所有数据表和索引
3. ✅ 初始化默认账户和演示数据

> 💡 **老王提示**：整个过程约10-20秒，命令行窗口会显示初始化进度。看到"Application started"就说明初始化完成了。

##### 4.3 访问系统

打开浏览器（推荐使用 Chrome 或 Edge），访问：

```
http://localhost:5000
```

> ✅ **成功标志**：看到登录页面，说明系统运行正常！

##### 4.4 默认账户信息

系统初始化后会自动创建以下账户：

| 角色 | 用户名 | 密码 | 权限说明 |
|------|--------|------|---------|
| **管理员** | `admin` | `Admin123456` | 拥有所有权限（用户管理、系统设置、报表导出等） |
| **操作员** | `operator` | `Operator123456` | 只能操作称重记录和查看报表 |
| **演示账户** | `demo` | `demo123` | 管理员权限，自动启用硬件模拟模式 |

> ⚠️ **安全提示**：首次登录后，请立即修改默认密码！

##### 4.5 停止应用程序

在命令行窗口中按下 `Ctrl + C` 键，或直接关闭窗口。

---

#### 步骤5：安装为Windows服务（可选，推荐长期使用）

如果你需要系统**开机自动启动**、**后台运行**，建议安装为Windows服务。

##### 5.1 下载NSSM工具

NSSM（Non-Sucking Service Manager）是一个免费的Windows服务管理工具。

**下载地址**：

```
https://nssm.cc/download
```

**下载步骤**：

1. 访问上述网址
2. 点击 **"Download NSSM 2.24"**（版本号可能不同）
3. 下载 `nssm-2.24.zip` 文件
4. 解压到任意目录（例如：`C:\Tools\nssm`）

##### 5.2 安装为Windows服务

**以管理员身份运行命令提示符**：

1. 按下 `Win + X` 键
2. 选择 **"Windows PowerShell (管理员)"** 或 **"命令提示符 (管理员)"**

**执行以下命令**：

```powershell
# 进入NSSM目录（根据实际解压路径调整）
cd C:\Tools\nssm\win64

# 安装服务
nssm install MiniMES "C:\MiniMES\Minimes.Web.exe"

# 配置服务工作目录
nssm set MiniMES AppDirectory "C:\MiniMES"

# 配置服务显示名称
nssm set MiniMES DisplayName "MiniMES 记账系统"

# 配置服务描述
nssm set MiniMES Description "MiniMES 生产记账系统 - 扫码称重管理"

# 配置开机自启
nssm set MiniMES Start SERVICE_AUTO_START
```

##### 5.3 启动服务

```powershell
# 启动服务
nssm start MiniMES

# 查看服务状态
nssm status MiniMES
```

**预期输出**：

```
SERVICE_RUNNING
```

##### 5.4 验证服务运行

打开浏览器，访问：

```
http://localhost:5000
```

> ✅ **成功标志**：看到登录页面，说明服务运行正常！

##### 5.5 服务管理命令

```powershell
# 停止服务
nssm stop MiniMES

# 重启服务
nssm restart MiniMES

# 查看服务状态
nssm status MiniMES

# 卸载服务（慎用！）
nssm remove MiniMES confirm
```

---

#### 步骤6：配置防火墙（可选）

如果您需要**局域网内其他电脑访问**系统，需要开放防火墙端口。

##### 方法1：使用PowerShell（推荐）

**以管理员身份运行PowerShell**，执行以下命令：

```powershell
New-NetFirewallRule -DisplayName "MiniMES HTTP" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

##### 方法2：使用图形界面

1. 按下 `Win + R` 键，输入 `wf.msc`，打开防火墙设置
2. 点击左侧 **"入站规则"**
3. 点击右侧 **"新建规则..."**
4. 选择 **"端口"**，点击 **"下一步"**
5. 选择 **"TCP"**，输入端口号 `5000`，点击 **"下一步"**
6. 选择 **"允许连接"**，点击 **"下一步"**
7. 全选（域、专用、公用），点击 **"下一步"**
8. 输入名称 `MiniMES HTTP`，点击 **"完成"**

##### 验证防火墙规则

在局域网内其他电脑的浏览器中访问：

```
http://您的电脑IP:5000
```

> 💡 **查看本机IP地址**：在命令提示符中输入 `ipconfig`，查找 **"IPv4 地址"**。

---

## 硬件设备配置

> 💡 **老王提示**：如果您没有硬件设备，可以跳过此部分，直接使用**演示模式**（demo账户登录）。

### 🎭 演示模式说明（推荐新手）

使用 `demo` 账户登录时，系统会自动启用**硬件模拟模式**，无需连接真实的电子秤和扫码枪。

#### 演示模式功能

| 功能 | 说明 |
|------|------|
| **电子秤模拟** | 自动推送波动的重量值（模拟真实秤的抖动），稳定后推送"稳定"状态 |
| **扫码枪模拟** | 从数据库已有商品中随机选择条码推送 |
| **完整业务流程** | 扫码 → 称重 → 关联客户 → 存档 → 导出Excel |

#### 演示模式适用场景

- ✅ 无硬件环境下的功能演示
- ✅ 新用户熟悉系统操作流程
- ✅ 培训和教学
- ✅ 销售演示

> 💡 **老王提示**：演示模式产生的数据与正式数据完全一致，可以正常导出Excel报表。先用演示模式熟悉系统，再决定要不要买硬件。

---

### 🔌 电子秤配置（串口）

如果您有支持串口通信的电子秤，按照以下步骤配置：

#### 步骤1：确认串口号

1. 将电子秤的串口线连接到电脑（USB转串口或直接串口）
2. 按下 `Win + X` 键，选择 **"设备管理器"**
3. 展开 **"端口(COM和LPT)"** 节点
4. 找到电子秤对应的端口号（例如：`COM3`）

> 💡 **老王提示**：如果看不到端口，可能需要安装USB转串口驱动程序。驱动程序通常在电子秤的光盘里，或者去厂家官网下载。

#### 步骤2：修改配置文件

1. 用记事本打开 `C:\MiniMES\appsettings.json`
2. 找到 `"Hardware"` 部分，修改为：

```json
{
  "Hardware": {
    "ScaleType": "Serial",
    "Scale": {
      "PortName": "COM3",
      "BaudRate": 9600,
      "DataBits": 8,
      "StopBits": "One",
      "Parity": "None",
      "Protocol": "Generic"
    }
  }
}
```

**配置项说明**：

| 配置项 | 说明 | 常见值 | 老王的话 |
|--------|------|--------|---------|
| `ScaleType` | 电子秤类型 | `Serial`（串口）或 `WiFi`（WiFi秤） | 串口秤选Serial |
| `PortName` | 串口号 | `COM3`、`COM4` 等 | 在设备管理器里看到的端口号 |
| `BaudRate` | 波特率 | `9600`、`4800`、`19200` | 看电子秤说明书 |
| `Protocol` | 通信协议 | `Generic`（通用）、`Toledo`、`Mettler` | 不知道就选Generic |

> 💡 **老王提示**：波特率和协议请参考电子秤的说明书。如果说明书找不到了，试试9600波特率+Generic协议，大部分秤都支持。

#### 步骤3：测试连接

1. 重启应用程序（如果是服务，用 `nssm restart MiniMES`）
2. 登录系统后，访问 **"硬件测试"** 页面（管理员菜单）
3. 点击 **"连接电子秤"** 按钮
4. 观察是否显示重量数据

**成功标志**：
- ✅ 显示 "已连接" 状态
- ✅ 实时显示重量数据
- ✅ 重量稳定后显示 "稳定" 状态

**如果连接失败**：
- ❌ 检查串口号是否正确
- ❌ 检查波特率是否匹配
- ❌ 检查电子秤是否开机
- ❌ 检查串口线是否插好

---

### 📶 WiFi电子秤配置

如果您有支持WiFi的电子秤（如A&D UC-1000WF），按照以下步骤配置：

#### 步骤1：确认电子秤IP地址

1. 按照电子秤说明书，将电子秤连接到WiFi网络
2. 在电子秤屏幕上查看IP地址（例如：`192.168.1.200`）

> 💡 **老王提示**：确保电脑和电子秤在同一个WiFi网络中，不然连不上。

#### 步骤2：修改配置文件

1. 用记事本打开 `C:\MiniMES\appsettings.json`
2. 找到 `"Hardware"` 部分，修改为：

```json
{
  "Hardware": {
    "ScaleType": "WiFi",
    "WiFiScale": {
      "IpAddress": "192.168.1.200",
      "Port": 80,
      "Protocol": "HTTP",
      "WeightApiPath": "/api/weight",
      "TareApiPath": "/api/tare"
    }
  }
}
```

**配置项说明**：

| 配置项 | 说明 | 示例值 | 老王的话 |
|--------|------|--------|---------|
| `ScaleType` | 电子秤类型 | `WiFi` | WiFi秤选WiFi |
| `IpAddress` | 电子秤IP地址 | `192.168.1.200` | 在秤的屏幕上看 |
| `Port` | 端口号 | `80` | 一般都是80 |

#### 步骤3：测试连接

1. 重启应用程序
2. 访问 **"硬件测试"** 页面
3. 点击 **"连接电子秤"** 按钮
4. 观察是否显示重量数据

---

### 🔫 扫码枪配置

扫码枪通常模拟键盘输入，**无需特殊配置**。

#### 连接步骤

1. 将扫码枪的USB线连接到电脑
2. 等待Windows自动识别（约5-10秒）
3. 打开记事本，扫描条码测试
4. 如果记事本中显示条码内容，说明连接成功

> 💡 **老王提示**：扫码枪就是个模拟键盘的设备，插上就能用，比电子秤简单多了。

#### 配置扫码枪后缀

为了提高扫码效率，建议配置扫码枪在扫描后自动添加**回车符**：

1. 参考扫码枪说明书，找到 **"添加后缀"** 或 **"Add Suffix"** 设置条码
2. 扫描该条码，启用 **"回车"** 或 **"Enter"** 后缀
3. 测试：扫描条码后，光标应自动跳到下一行

> 💡 **老王提示**：大部分扫码枪出厂时已默认添加回车后缀，不用特别设置。

---

## 功能验证

部署完成后，按照以下清单验证系统功能是否正常。

### ✅ 验证清单

#### 1. 系统访问验证

- [ ] 打开浏览器，访问 `http://localhost:5000`
- [ ] 看到登录页面
- [ ] 页面样式正常显示（无乱码、无错位）

> 💡 **老王提示**：如果页面打不开，检查应用程序是否在运行（看任务管理器里有没有Minimes.Web.exe进程）。

---

#### 2. 用户登录验证

使用默认管理员账户登录：
- 用户名：`admin`
- 密码：`Admin123456`

- [ ] 登录成功，跳转到首页
- [ ] 左侧导航菜单正常显示
- [ ] 顶部显示用户名和登出按钮

> 💡 **老王提示**：如果登录失败，检查数据库是否正常运行（用 `sc query MySQL3371` 查看MySQL服务状态）。

---

#### 3. 基础功能验证

**客户管理**：
- [ ] 点击 **"客户管理"** 菜单
- [ ] 能够查看客户列表
- [ ] 能够新增客户（点击"新增"按钮，填写客户信息）
- [ ] 能够编辑客户（点击"编辑"按钮，修改客户信息）
- [ ] 能够删除客户（点击"删除"按钮）

**商品管理**：
- [ ] 点击 **"商品管理"** 菜单
- [ ] 能够查看商品列表
- [ ] 能够新增商品批次（点击"新增"按钮，填写商品信息和条码）
- [ ] 能够编辑商品（点击"编辑"按钮，修改商品信息）
- [ ] 能够删除商品（点击"删除"按钮）

> 💡 **老王提示**：先添加几个客户和商品，后面测试称重功能时要用。

---

#### 4. 称重功能验证（演示模式）

**推荐使用演示模式测试**：

1. 登出当前账户（点击右上角"登出"）
2. 使用演示账户登录：
   - 用户名：`demo`
   - 密码：`demo123`
3. 点击 **"生产称重"** 菜单
4. 观察页面：
   - [ ] 页面顶部显示"演示模式"提示
   - [ ] 自动显示模拟重量数据（数字在跳动）
   - [ ] 自动推送模拟条码（页面显示商品信息）
   - [ ] 能够选择客户（下拉框选择）
   - [ ] 能够保存称重记录（点击"保存"按钮）
5. 保存成功后：
   - [ ] 页面显示"保存成功"提示
   - [ ] 记录自动清空，可以继续称重

> 💡 **老王提示**：演示模式会自动模拟电子秤和扫码枪，不需要真实硬件。这是最简单的测试方式。

---

#### 5. 真实硬件测试（如有硬件）

如果您已经配置了真实的电子秤和扫码枪：

1. 登录管理员账户（`admin` / `Admin123456`）
2. 访问 **"硬件测试"** 页面（管理员菜单）
3. 测试电子秤：
   - [ ] 点击 **"连接电子秤"** 按钮
   - [ ] 显示 "已连接" 状态
   - [ ] 实时显示重量数据（放东西到秤上，数字会变化）
   - [ ] 重量稳定后显示 "稳定" 状态
4. 测试扫码枪：
   - [ ] 使用扫码枪扫描商品条码
   - [ ] 页面显示扫描的条码
   - [ ] 自动匹配商品信息（如果条码在数据库中）

> 💡 **老王提示**：如果硬件连接失败，回到"硬件设备配置"章节，检查配置是否正确。

---

#### 6. 报表功能验证

1. 点击 **"生产报表"** 菜单
2. 选择日期范围（例如：今天到今天）
3. 点击 **"查询"** 按钮
4. 验证：
   - [ ] 能够查看统计数据（总重量、总记录数等）
   - [ ] 能够查看详细记录列表
5. 点击 **"导出Excel"** 按钮
6. 验证：
   - [ ] 成功下载Excel文件（文件名类似：`生产报表_20260129.xlsx`）
   - [ ] 打开Excel文件，数据正常显示
   - [ ] Excel中有统计数据和详细记录

> 💡 **老王提示**：如果没有数据，先用演示模式添加几条称重记录，然后再导出报表。

---

#### 7. 用户管理验证（管理员）

1. 点击 **"用户管理"** 菜单
2. 验证：
   - [ ] 能够查看用户列表（应该有admin、operator、demo三个账户）
   - [ ] 能够新增用户（点击"新增"按钮，填写用户信息）
   - [ ] 能够编辑用户角色（点击"编辑"按钮，修改角色）
   - [ ] 能够禁用/启用用户（点击"禁用"或"启用"按钮）

> ⚠️ **老王提示**：不要删除或禁用admin账户，不然你就登录不了了！

---

#### 8. 个人中心验证

1. 点击 **"个人中心"** 菜单
2. 验证：
   - [ ] 能够查看个人信息（用户名、姓名、角色）
   - [ ] 能够查看操作统计（今日操作、本月操作、总操作）
   - [ ] 能够查看最近操作记录
3. 点击 **"修改密码"** 按钮
4. 验证：
   - [ ] 能够输入旧密码和新密码
   - [ ] 能够保存新密码
   - [ ] 保存后能用新密码登录

> 💡 **老王提示**：修改密码后记得用新密码登录，别忘了密码！

---

### 🎉 验证完成

> ✅ **如果以上功能都正常，说明系统部署成功！恭喜你，可以开始正式使用了！**

> ❌ **如果有功能不正常，请查看下一章"常见问题排查"。**

---

## 常见问题排查

### ❌ 问题1：应用程序无法启动

**症状**：
- 双击 `Minimes.Web.exe` 无反应
- 命令行窗口一闪而过
- 服务启动失败

**排查步骤**：

#### 步骤1：检查.NET Runtime

打开命令提示符，输入：

```powershell
dotnet --list-runtimes
```

**预期输出**：应该看到 `Microsoft.AspNetCore.App 8.0.x`

**解决方案**：如果没有看到，请重新安装.NET 8.0 Runtime（参考"部署应用程序"部分）。

#### 步骤2：检查端口占用

```powershell
netstat -ano | findstr :5000
```

**如果有输出**：说明端口5000被占用。

**解决方案**：
1. 找到占用端口的进程ID（最后一列数字）
2. 打开任务管理器，结束该进程
3. 或者修改配置文件，使用其他端口（如5001）

#### 步骤3：查看错误日志

查看应用程序目录下的日志文件：

```
C:\MiniMES\logs\
```

打开最新的日志文件，查找错误信息。

> 💡 **老王提示**：日志文件名类似 `minimes-20260129.log`，用记事本打开就能看。

---

### ❌ 问题2：数据库连接失败

**症状**：
- 应用启动后报数据库错误
- 提示"无法连接到MySQL服务器"
- 日志中有 `MySqlException` 错误

**排查步骤**：

#### 步骤1：检查MySQL服务状态

```powershell
sc query MySQL3371
```

**预期输出**：应该看到 `STATE : 4 RUNNING`

**解决方案**：如果服务未运行，启动MySQL服务：

```powershell
net start MySQL3371
```

#### 步骤2：检查数据库配置

1. 打开 `C:\MiniMES\appsettings.json`
2. 检查 `ConnectionStrings` 部分：
   - `Server` 是否为 `127.0.0.1`
   - `Port` 是否为 `3371`
   - `Database` 是否为 `minimes`
   - `User` 和 `Password` 是否正确

#### 步骤3：测试数据库连接

```powershell
mysql -u root -p -P 3371
```

输入密码后，如果能登录成功，说明MySQL正常。

**解决方案**：如果无法登录，检查MySQL密码是否正确。

---

### ❌ 问题3：电子秤无法连接

**症状**：
- 硬件测试页面显示"连接失败"
- 无法读取重量数据

**排查步骤**：

#### 步骤1：确认串口号

1. 按下 `Win + X` 键，选择 **"设备管理器"**
2. 展开 **"端口(COM和LPT)"**
3. 确认电子秤对应的COM口号（如COM3）

**解决方案**：修改 `appsettings.json` 中的 `PortName` 配置。

#### 步骤2：检查串口占用

```powershell
# 查看串口设备
mode
```

**解决方案**：如果串口被其他程序占用，请关闭该程序。

> 💡 **老王提示**：如果实在搞不定硬件，先用演示模式（demo账户）熟悉系统，硬件问题慢慢解决。

---

### ❌ 问题4：忘记管理员密码

**症状**：
- 忘记admin账户密码
- 无法登录系统

**解决方案**：

#### 方法1：使用演示账户登录

使用演示账户登录（`demo` / `demo123`），演示账户也有管理员权限，可以修改其他用户密码。

#### 方法2：重置数据库（会丢失所有数据）

```powershell
# 停止应用
nssm stop MiniMES

# 登录MySQL
mysql -u root -p -P 3371

# 删除数据库
DROP DATABASE minimes;

# 重新创建数据库
CREATE DATABASE minimes CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# 退出MySQL
EXIT;

# 重启应用（会自动初始化数据库）
nssm start MiniMES
```

默认管理员账户：
- 用户名：`admin`
- 密码：`Admin123456`

> ⚠️ **注意**：此操作会删除所有数据！

---

## 日常维护

### 📦 数据备份

**推荐备份频率**：每周一次（或根据数据重要性调整）

#### 手动备份

```powershell
# 停止应用（如果是服务）
nssm stop MiniMES

# 备份MySQL数据库
mysqldump -u root -p -P 3371 minimes > C:\MiniMES\backup\minimes-20260129.sql

# 重启应用
nssm start MiniMES
```

> 💡 **老王提示**：备份文件名加上日期，方便以后找。例如：`minimes-20260129.sql`

#### 自动备份（使用Windows任务计划程序）

1. 创建备份脚本 `C:\MiniMES\backup.bat`：

```batch
@echo off
set BACKUP_DIR=C:\MiniMES\backup
set MYSQL_BIN=C:\MySQL\bin
set DATE=%date:~0,4%%date:~5,2%%date:~8,2%

%MYSQL_BIN%\mysqldump -u root -proot -P 3371 minimes > %BACKUP_DIR%\minimes-%DATE%.sql

echo Backup completed: minimes-%DATE%.sql
```

2. 设置任务计划：
   - 按下 `Win + R` 键，输入 `taskschd.msc`
   - 点击 **"创建基本任务"**
   - 名称：`MiniMES数据库备份`
   - 触发器：**"每周"**，选择备份时间
   - 操作：**"启动程序"**
   - 程序：`C:\MiniMES\backup.bat`

> 💡 **老王提示**：备份脚本中的密码（`-proot`）要改成你自己的MySQL密码。

---

### 🔄 数据恢复

如果数据库出问题了，可以从备份恢复：

```powershell
# 停止应用
nssm stop MiniMES

# 登录MySQL
mysql -u root -p -P 3371

# 删除现有数据库
DROP DATABASE minimes;

# 重新创建数据库
CREATE DATABASE minimes CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# 退出MySQL
EXIT;

# 从备份恢复
mysql -u root -p -P 3371 minimes < C:\MiniMES\backup\minimes-20260129.sql

# 重启应用
nssm start MiniMES
```

> ⚠️ **注意**：恢复数据库会覆盖现有数据，请确认备份文件是正确的！

---

### 🔐 密码修改

**修改自己的密码**：

1. 登录系统
2. 点击 **"个人中心"** 菜单
3. 点击 **"修改密码"** 按钮
4. 输入旧密码和新密码
5. 点击 **"保存"** 按钮

**修改其他用户密码**（管理员）：

1. 登录系统（使用管理员账户）
2. 点击 **"用户管理"** 菜单
3. 找到目标用户，点击 **"编辑"** 按钮
4. 点击 **"重置密码"** 按钮
5. 输入新密码，点击 **"保存"**

---

### 🗑️ 日志清理

系统日志文件位于：

```
C:\MiniMES\logs\
```

**清理建议**：

- 保留最近30天的日志
- 定期删除旧日志文件（每月一次）

```powershell
# 删除30天前的日志文件
cd C:\MiniMES\logs
forfiles /p . /s /m *.log /d -30 /c "cmd /c del @path"
```

> 💡 **老王提示**：日志文件会越来越多，定期清理可以节省硬盘空间。

---

### 🔄 系统更新

**更新步骤**：

1. **备份数据库**（重要！）
2. **停止应用服务**
3. **替换应用程序文件**（保留 `appsettings.json`）
4. **重启应用服务**
5. **验证功能**

```powershell
# 1. 备份数据库
mysqldump -u root -p -P 3371 minimes > C:\MiniMES\backup\minimes-before-update.sql

# 2. 停止服务
nssm stop MiniMES

# 3. 解压新版本文件到临时目录
# 手动复制新文件到 C:\MiniMES（不要覆盖 appsettings.json）

# 4. 重启服务
nssm start MiniMES

# 5. 验证
# 打开浏览器访问 http://localhost:5000
```

> ⚠️ **重要**：更新前一定要备份数据库！万一更新失败，可以回滚。

---

# 第二部分：从源代码打包部署（技术人员专用）

> 💡 **老王提示**：这部分是给开发人员看的，如果你只是想用系统，看第一部分就够了。如果你想自己改代码、打包程序，那就继续往下看。

---

## 开发环境准备

### 步骤1：安装.NET 8.0 SDK

.NET SDK是开发工具包，包含了编译和发布.NET程序所需的所有工具。

#### 1.1 下载.NET 8.0 SDK

访问微软官方下载页面：

```
https://dotnet.microsoft.com/download/dotnet/8.0
```

#### 1.2 选择正确的安装包

在下载页面找到 **".NET 8.0 SDK"** 部分，选择：

```
Windows x64 - Installer
```

> 📥 **文件名示例**：`dotnet-sdk-8.0.101-win-x64.exe`（版本号可能不同）

> 💡 **老王解释**：SDK和Runtime的区别：
> - **Runtime**：只能运行程序，不能编译（普通用户用这个）
> - **SDK**：既能运行又能编译（开发人员用这个）

#### 1.3 安装步骤

1. 双击下载的安装包
2. 点击 **"Install"** 按钮
3. 等待安装完成（约2-3分钟）
4. 点击 **"Close"** 关闭安装程序

#### 1.4 验证安装

按下 `Win + R` 键，输入 `cmd`，打开命令提示符，输入以下命令：

```powershell
dotnet --version
```

**预期输出**（版本号可能不同）：

```
8.0.101
```

> ✅ **验证成功**：如果看到版本号，说明安装成功！

---

### 步骤2：安装Git（可选）

如果你的源代码在Git仓库中，需要安装Git。

#### 2.1 下载Git

访问Git官网：

```
https://git-scm.com/download/win
```

#### 2.2 安装Git

1. 双击下载的安装包
2. 一路点击 **"Next"**（使用默认设置即可）
3. 点击 **"Install"** 按钮
4. 等待安装完成
5. 点击 **"Finish"** 关闭安装程序

#### 2.3 验证安装

```powershell
git --version
```

**预期输出**：

```
git version 2.43.0.windows.1
```

> 💡 **老王提示**：如果你的源代码是zip文件，不需要安装Git。

---

### 步骤3：获取源代码

#### 方法1：从Git仓库克隆（推荐）

```powershell
# 进入你想存放代码的目录
cd D:\Projects

# 克隆代码仓库（改成你自己的仓库地址）
git clone https://github.com/yourusername/minimes.git

# 进入项目目录
cd minimes
```

#### 方法2：解压zip文件

如果你的源代码是zip文件：

1. 右键点击zip文件
2. 选择 **"全部解压缩..."**
3. 解压到：`D:\Projects\minimes`

---

## 编译和打包

### 步骤1：编译项目

#### 1.1 进入项目目录

```powershell
cd D:\Projects\minimes
```

#### 1.2 还原NuGet包

```powershell
dotnet restore
```

**预期输出**：

```
Restore completed in 5.23 sec for D:\Projects\minimes\src\Minimes.Domain\Minimes.Domain.csproj.
Restore completed in 5.45 sec for D:\Projects\minimes\src\Minimes.Application\Minimes.Application.csproj.
...
```

> 💡 **老王解释**：NuGet包就是项目依赖的第三方库，类似于npm的node_modules。这一步会下载所有需要的库。

#### 1.3 编译项目

```powershell
dotnet build --configuration Release
```

**预期输出**：

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

> ✅ **成功标志**：看到"Build succeeded"，说明编译成功！
> ❌ **如果有错误**：检查错误信息，可能是代码有问题或者缺少依赖。

---

### 步骤2：发布为单文件可执行程序

#### 2.1 发布命令

```powershell
dotnet publish src/Minimes.Web/Minimes.Web.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --output publish/win-x64 `
  /p:PublishSingleFile=true `
  /p:IncludeNativeLibrariesForSelfExtract=true `
  /p:PublishTrimmed=false
```

> 💡 **老王解释**：这个命令的意思是：
> - `--configuration Release`：发布生产版本（优化过的）
> - `--runtime win-x64`：发布Windows 64位版本
> - `--self-contained true`：包含.NET运行时（用户不需要安装.NET）
> - `--output publish/win-x64`：输出到publish/win-x64目录
> - `/p:PublishSingleFile=true`：打包成单个exe文件
> - `/p:IncludeNativeLibrariesForSelfExtract=true`：包含本地库
> - `/p:PublishTrimmed=false`：不裁剪（避免运行时错误）

**预期输出**：

```
Minimes.Web -> D:\Projects\minimes\publish\win-x64\Minimes.Web.exe
```

> ✅ **成功标志**：看到"Minimes.Web.exe"，说明发布成功！

#### 2.2 检查发布结果

```powershell
dir publish\win-x64
```

**应该看到**：

```
Minimes.Web.exe          # 主程序（约80-100MB）
appsettings.json         # 配置文件
appsettings.Production.json  # 生产环境配置
wwwroot\                 # 静态资源目录
```

> 💡 **老王提示**：单文件发布后，exe文件会比较大（80-100MB），因为包含了.NET运行时。这样用户就不需要安装.NET了。

---

### 步骤3：准备发布包

#### 3.1 创建发布目录

```powershell
# 创建发布包目录
mkdir release
cd release
mkdir minimes-win-x64-v1.0.0
```

#### 3.2 复制必要文件

```powershell
# 复制主程序
copy ..\publish\win-x64\Minimes.Web.exe minimes-win-x64-v1.0.0\

# 复制配置文件
copy ..\publish\win-x64\appsettings.json minimes-win-x64-v1.0.0\

# 复制静态资源目录
xcopy ..\publish\win-x64\wwwroot minimes-win-x64-v1.0.0\wwwroot\ /E /I

# 创建日志目录
mkdir minimes-win-x64-v1.0.0\logs

# 创建备份目录
mkdir minimes-win-x64-v1.0.0\backup
```

#### 3.3 创建README文件

创建 `minimes-win-x64-v1.0.0\README.txt`：

```text
MiniMES 记账系统 v1.0.0
========================

部署说明：
1. 安装MySQL数据库（端口3371）
2. 创建数据库：CREATE DATABASE minimes CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
3. 修改appsettings.json中的数据库连接字符串
4. 双击Minimes.Web.exe启动程序
5. 浏览器访问：http://localhost:5000

默认账户：
- 管理员：admin / Admin123456
- 操作员：operator / Operator123456
- 演示账户：demo / demo123

详细部署文档请参考：DEPLOYMENT-WINDOWS11.md

技术支持：support@minimes.com
```

---

### 步骤4：打包成zip文件

#### 4.1 使用PowerShell压缩

```powershell
# 压缩成zip文件
Compress-Archive -Path minimes-win-x64-v1.0.0 -DestinationPath minimes-win-x64-v1.0.0.zip
```

#### 4.2 验证zip文件

```powershell
# 查看zip文件大小
dir minimes-win-x64-v1.0.0.zip
```

**预期大小**：约80-100MB

> ✅ **成功标志**：看到zip文件，说明打包成功！

---

### 步骤5：测试发布包

在打包完成后，建议先测试一下：

1. 解压zip文件到测试目录（如 `C:\Test\MiniMES`）
2. 修改 `appsettings.json` 配置数据库连接
3. 双击 `Minimes.Web.exe` 启动程序
4. 浏览器访问 `http://localhost:5000`
5. 测试基本功能（登录、添加数据、导出报表等）

> 💡 **老王提示**：测试通过后再发给用户，别发个有问题的包出去丢人！

---

## 打包成安装程序（可选）

如果你想做得更专业，可以用Inno Setup打包成安装程序。

### 步骤1：下载Inno Setup

访问官网：

```
https://jrsoftware.org/isdl.php
```

下载并安装Inno Setup。

### 步骤2：创建安装脚本

创建 `minimes-setup.iss` 文件：

```ini
[Setup]
AppName=MiniMES记账系统
AppVersion=1.0.0
DefaultDirName={pf}\MiniMES
DefaultGroupName=MiniMES
OutputDir=output
OutputBaseFilename=minimes-setup-v1.0.0
Compression=lzma2
SolidCompression=yes

[Files]
Source: "release\minimes-win-x64-v1.0.0\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\MiniMES记账系统"; Filename: "{app}\Minimes.Web.exe"
Name: "{commondesktop}\MiniMES记账系统"; Filename: "{app}\Minimes.Web.exe"

[Run]
Filename: "{app}\Minimes.Web.exe"; Description: "启动MiniMES"; Flags: postinstall nowait skipifsilent
```

### 步骤3：编译安装程序

1. 右键点击 `minimes-setup.iss`
2. 选择 **"Compile"**
3. 等待编译完成
4. 在 `output` 目录下会生成 `minimes-setup-v1.0.0.exe`

> 💡 **老王提示**：安装程序会自动创建桌面快捷方式，用户体验更好。

---

## 快速参考卡

### 🔑 默认账户信息

| 角色 | 用户名 | 密码 | 权限 |
|------|--------|------|------|
| **管理员** | `admin` | `Admin123456` | 所有权限 |
| **操作员** | `operator` | `Operator123456` | 称重记录、报表查看 |
| **演示账户** | `demo` | `demo123` | 管理员权限 + 硬件模拟 |

> ⚠️ **安全提示**：首次登录后请立即修改密码！

---

### 🌐 常用访问地址

| 功能 | 地址 |
|------|------|
| **本机访问** | `http://localhost:5000` |
| **局域网访问** | `http://您的电脑IP:5000` |
| **硬件测试** | `http://localhost:5000/hardware-test` |
| **生产称重** | `http://localhost:5000/weighing` |
| **生产报表** | `http://localhost:5000/reports/production` |

> 💡 **查看本机IP**：在命令提示符中输入 `ipconfig`

---

### 🛠️ 常用命令速查

#### MySQL管理

```powershell
# 启动MySQL服务
net start MySQL3371

# 停止MySQL服务
net stop MySQL3371

# 查看MySQL服务状态
sc query MySQL3371

# 登录MySQL
mysql -u root -p -P 3371

# 备份数据库
mysqldump -u root -p -P 3371 minimes > backup.sql

# 恢复数据库
mysql -u root -p -P 3371 minimes < backup.sql
```

#### 应用程序管理

```powershell
# 启动应用（直接运行）
cd C:\MiniMES
Minimes.Web.exe

# 启动服务
nssm start MiniMES

# 停止服务
nssm stop MiniMES

# 重启服务
nssm restart MiniMES

# 查看服务状态
nssm status MiniMES
```

#### 防火墙管理

```powershell
# 开放5000端口
New-NetFirewallRule -DisplayName "MiniMES HTTP" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow

# 查看防火墙规则
Get-NetFirewallRule -DisplayName "MiniMES HTTP"
```

---

### 📂 重要文件路径

| 文件/目录 | 路径 | 说明 |
|----------|------|------|
| **应用程序** | `C:\MiniMES\Minimes.Web.exe` | 主程序 |
| **配置文件** | `C:\MiniMES\appsettings.json` | 系统配置 |
| **日志目录** | `C:\MiniMES\logs\` | 系统日志 |
| **备份目录** | `C:\MiniMES\backup\` | 数据库备份 |
| **MySQL安装目录** | `C:\MySQL\` | MySQL数据库 |
| **MySQL配置文件** | `C:\MySQL\my.ini` | MySQL配置 |
| **MySQL数据目录** | `C:\MySQL\data\` | 数据库文件 |

---

### 📞 技术支持

| 项目 | 信息 |
|------|------|
| **技术支持邮箱** | support@minimes.com |
| **项目GitHub** | https://github.com/yourusername/minimes |
| **问题反馈** | https://github.com/yourusername/minimes/issues |
| **在线文档** | https://docs.minimes.com |

---

### 💡 常见操作快捷键

| 功能 | 快捷键 |
|------|--------|
| **登出** | `Alt + Shift + Q` |
| **刷新页面** | `F5` 或 `Ctrl + R` |
| **搜索** | `Ctrl + F` |
| **保存** | `Ctrl + S` |

---

## 🎉 部署成功！

恭喜您成功部署 MiniMES 记账系统！

### 接下来做什么？

1. ✅ 登录系统，修改默认密码
2. ✅ 添加客户和商品信息
3. ✅ 测试硬件设备（如有）
4. ✅ 使用演示账户熟悉操作流程
5. ✅ 设置自动备份任务

### 需要帮助？

- 📧 发送邮件至 support@minimes.com
- 🐛 在GitHub上提交Issue
- 📖 查阅在线文档

---

**文档版本**: v1.0.0 | **更新日期**: 2026-01-29 | **适用系统**: Windows 11

---

**感谢您选择 MiniMES 记账系统！**

**老王提示**：这个文档老王我写得够详细了吧？从安装MySQL到打包程序，每一步都写清楚了。如果还有不懂的，那就是你自己的问题了！艹，老王我累死了，去喝杯茶休息一下。


