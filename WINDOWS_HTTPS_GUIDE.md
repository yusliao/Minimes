# Windows NSSM服务配置HTTPS指南

> 🔒 让你的MiniMES服务支持HTTPS，满足PWA要求

---

## 📋 方案对比

| 方案 | 难度 | 优点 | 缺点 | 推荐度 |
|-----|------|------|------|--------|
| **方案1：Kestrel内置HTTPS** | ⭐⭐ | 配置简单，无需额外软件 | 证书管理需手动 | ⭐⭐⭐⭐ |
| **方案2：IIS反向代理** | ⭐⭐⭐ | 证书管理方便，性能好 | 需要安装IIS | ⭐⭐⭐⭐⭐ |
| **方案3：自签名证书（测试）** | ⭐ | 快速测试PWA功能 | 浏览器警告，仅限测试 | ⭐⭐⭐ |

---

## 🚀 方案1：Kestrel内置HTTPS（推荐新手）

### 适用场景
- 简单部署，不想安装IIS
- 小型应用，访问量不大
- 快速上线

### 第一步：生成或获取SSL证书

#### 选项A：使用自签名证书（局域网测试）

```powershell
# 以管理员身份运行PowerShell

# 1. 生成自签名证书
$cert = New-SelfSignedCertificate `
    -DnsName "localhost", "192.168.1.100", "your-domain.com" `
    -CertStoreLocation "cert:\LocalMachine\My" `
    -FriendlyName "MiniMES Development Certificate" `
    -NotAfter (Get-Date).AddYears(5)

# 2. 导出证书（带私钥）
$certPassword = ConvertTo-SecureString -String "YourPassword123" -Force -AsPlainText
$certPath = "C:\Certificates\minimes.pfx"
New-Item -ItemType Directory -Force -Path "C:\Certificates"
Export-PfxCertificate -Cert $cert -FilePath $certPath -Password $certPassword

# 3. 将证书添加到受信任的根证书颁发机构（消除浏览器警告）
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root", "LocalMachine")
$store.Open("ReadWrite")
$store.Add($cert)
$store.Close()

Write-Host "证书已生成：$certPath"
Write-Host "证书密码：YourPassword123"
Write-Host "证书指纹：$($cert.Thumbprint)"
```

#### 选项B：使用Let's Encrypt免费证书（公网域名）

```powershell
# 安装win-acme（Let's Encrypt客户端）
# 下载地址：https://github.com/win-acme/win-acme/releases

# 1. 下载并解压win-acme
# 2. 以管理员身份运行 wacs.exe
# 3. 选择 N: Create certificate (default settings)
# 4. 输入域名
# 5. 选择验证方式（HTTP或DNS）
# 6. 证书会自动保存到：C:\ProgramData\win-acme\certificates\
```

### 第二步：配置Kestrel使用HTTPS

编辑 `appsettings.json`（或 `appsettings.Production.json`）：

```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://*:5000"
      },
      "Https": {
        "Url": "https://*:5001",
        "Certificate": {
          "Path": "C:\\Certificates\\minimes.pfx",
          "Password": "YourPassword123"
        }
      }
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=minimes.db"
  }
}
```

**重要配置说明**：
- `http://*:5000` - HTTP端口（可选，建议保留用于健康检查）
- `https://*:5001` - HTTPS端口
- `Path` - 证书文件路径（使用双反斜杠 `\\`）
- `Password` - 证书密码

### 第三步：配置防火墙

```powershell
# 以管理员身份运行PowerShell

# 开放HTTPS端口（5001）
New-NetFirewallRule -DisplayName "MiniMES HTTPS" `
    -Direction Inbound `
    -Protocol TCP `
    -LocalPort 5001 `
    -Action Allow

# 如果需要标准HTTPS端口（443），需要URL重写或端口转发
# 方法1：使用netsh端口转发（推荐）
netsh interface portproxy add v4tov4 `
    listenport=443 `
    listenaddress=0.0.0.0 `
    connectport=5001 `
    connectaddress=127.0.0.1

# 开放443端口
New-NetFirewallRule -DisplayName "HTTPS (443)" `
    -Direction Inbound `
    -Protocol TCP `
    -LocalPort 443 `
    -Action Allow

# 查看端口转发规则
netsh interface portproxy show all
```

### 第四步：重启NSSM服务

```powershell
# 重启服务
nssm restart MiniMES

# 查看服务状态
nssm status MiniMES

# 查看服务日志（如果配置了日志）
Get-Content "C:\MiniMES\Logs\service.log" -Tail 50
```

### 第五步：测试HTTPS访问

```powershell
# 本地测试
Start-Process "https://localhost:5001"

# 局域网测试（替换为你的IP）
Start-Process "https://192.168.1.100:5001"

# 如果配置了443端口转发
Start-Process "https://192.168.1.100"
```

---

## 🏆 方案2：IIS反向代理（推荐生产环境）

### 适用场景
- 生产环境部署
- 需要更好的性能和稳定性
- 需要方便的证书管理
- 多个应用共享443端口

### 优点
- IIS处理SSL/TLS，性能更好
- 证书管理更方便（可视化界面）
- 支持自动续期（Let's Encrypt）
- 可以配置负载均衡、缓存等高级功能

### 第一步：安装IIS和必要组件

```powershell
# 以管理员身份运行PowerShell

# 安装IIS
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment
Enable-WindowsOptionalFeature -Online -FeatureName IIS-NetFxExtensibility45
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security
Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementConsole

# 安装URL重写模块（必须）
# 下载地址：https://www.iis.net/downloads/microsoft/url-rewrite
# 或使用Chocolatey安装
choco install urlrewrite -y

# 安装Application Request Routing（ARR）模块（必须）
# 下载地址：https://www.iis.net/downloads/microsoft/application-request-routing
# 或使用Chocolatey安装
choco install iis-arr -y
```

### 第二步：配置IIS站点

#### 2.1 创建IIS站点

1. 打开IIS管理器（`inetmgr`）
2. 右键"网站" → "添加网站"
3. 配置：
   - **网站名称**：MiniMES
   - **物理路径**：`C:\inetpub\wwwroot\minimes`（创建一个空文件夹即可）
   - **绑定类型**：http
   - **IP地址**：全部未分配
   - **端口**：80
   - **主机名**：留空（或填写域名）

#### 2.2 配置反向代理

在IIS站点根目录（`C:\inetpub\wwwroot\minimes`）创建 `web.config`：

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <system.webServer>
        <rewrite>
            <rules>
                <rule name="ReverseProxyInboundRule" stopProcessing="true">
                    <match url="(.*)" />
                    <action type="Rewrite" url="http://localhost:5000/{R:1}" />
                    <serverVariables>
                        <set name="HTTP_X_FORWARDED_PROTO" value="https" />
                        <set name="HTTP_X_FORWARDED_HOST" value="{HTTP_HOST}" />
                    </serverVariables>
                </rule>
            </rules>
        </rewrite>

        <!-- WebSocket支持（Blazor Server必需） -->
        <webSocket enabled="true" />

        <!-- 禁用响应缓冲（Blazor Server必需） -->
        <httpProtocol>
            <customHeaders>
                <add name="X-Content-Type-Options" value="nosniff" />
            </customHeaders>
        </httpProtocol>
    </system.webServer>
</configuration>
```

### 第三步：配置SSL证书

#### 选项A：使用自签名证书（测试）

```powershell
# 生成自签名证书（同方案1）
$cert = New-SelfSignedCertificate `
    -DnsName "localhost", "192.168.1.100" `
    -CertStoreLocation "cert:\LocalMachine\My" `
    -FriendlyName "MiniMES IIS Certificate" `
    -NotAfter (Get-Date).AddYears(5)

# 将证书添加到受信任的根证书颁发机构
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root", "LocalMachine")
$store.Open("ReadWrite")
$store.Add($cert)
$store.Close()

Write-Host "证书指纹：$($cert.Thumbprint)"
```

在IIS管理器中：
1. 选择"MiniMES"站点
2. 右键 → "编辑绑定"
3. 点击"添加"
4. 配置：
   - **类型**：https
   - **IP地址**：全部未分配
   - **端口**：443
   - **SSL证书**：选择刚才生成的证书

#### 选项B：使用Let's Encrypt免费证书（生产）

```powershell
# 使用win-acme自动配置IIS证书
# 下载：https://github.com/win-acme/win-acme/releases

# 1. 解压win-acme到 C:\Tools\win-acme\
# 2. 以管理员身份运行 wacs.exe
# 3. 选择 M: Create certificate with advanced options
# 4. 选择 2: IIS bindings
# 5. 选择你的IIS站点（MiniMES）
# 6. 选择验证方式：1: [http-01] Save verification files on (network) path
# 7. 选择存储方式：2: Certificate Store
# 8. 完成后证书会自动绑定到IIS站点，并配置自动续期
```

### 第四步：配置Application Request Routing

1. 打开IIS管理器
2. 选择服务器节点（最顶层）
3. 双击"Application Request Routing Cache"
4. 右侧点击"Server Proxy Settings"
5. 勾选"Enable proxy"
6. 点击"Apply"

### 第五步：测试反向代理

```powershell
# 确保NSSM服务正在运行
nssm status MiniMES

# 测试HTTP（应该能访问）
Start-Process "http://localhost"

# 测试HTTPS
Start-Process "https://localhost"

# 局域网测试
Start-Process "https://192.168.1.100"
```

---

## 🧪 方案3：快速测试方案（自签名证书）

### 适用场景
- 快速测试PWA功能
- 局域网内部使用
- 不在意浏览器安全警告

### 一键生成并配置

```powershell
# 以管理员身份运行PowerShell

# 1. 生成自签名证书
$cert = New-SelfSignedCertificate `
    -DnsName "localhost", "127.0.0.1", "192.168.1.100" `
    -CertStoreLocation "cert:\LocalMachine\My" `
    -FriendlyName "MiniMES Test Certificate" `
    -NotAfter (Get-Date).AddYears(1)

# 2. 导出证书
$certPassword = ConvertTo-SecureString -String "Test123456" -Force -AsPlainText
$certPath = "C:\Certificates\minimes-test.pfx"
New-Item -ItemType Directory -Force -Path "C:\Certificates"
Export-PfxCertificate -Cert $cert -FilePath $certPath -Password $certPassword

# 3. 添加到受信任的根证书（消除警告）
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root", "LocalMachine")
$store.Open("ReadWrite")
$store.Add($cert)
$store.Close()

Write-Host "✅ 证书已生成并安装"
Write-Host "证书路径：$certPath"
Write-Host "证书密码：Test123456"
Write-Host ""
Write-Host "下一步：修改 appsettings.json 配置证书路径和密码"
```

---

## 📋 完整配置示例

### appsettings.Production.json（Kestrel HTTPS）

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://*:5000"
      },
      "Https": {
        "Url": "https://*:443",
        "Certificate": {
          "Path": "C:\\Certificates\\minimes.pfx",
          "Password": "YourPassword123"
        }
      }
    }
  },
  "Database": {
    "Provider": "MySQL"
  },
  "ConnectionStrings": {
    "MySqlConnection": "Server=localhost;Port=3306;Database=minimes;User=root;Password=yourpassword;CharSet=utf8mb4;"
  },
  "AllowedHosts": "*"
}
```

### NSSM服务配置

```powershell
# 安装NSSM服务（如果还没安装）
nssm install MiniMES "C:\Program Files\dotnet\dotnet.exe"
nssm set MiniMES AppDirectory "D:\MyDomain\src\AI\minimes\src\Minimes.Web"
nssm set MiniMES AppParameters "Minimes.Web.dll"
nssm set MiniMES AppEnvironmentExtra "ASPNETCORE_ENVIRONMENT=Production"

# 配置日志
nssm set MiniMES AppStdout "C:\MiniMES\Logs\service.log"
nssm set MiniMES AppStderr "C:\MiniMES\Logs\error.log"

# 配置自动重启
nssm set MiniMES AppExit Default Restart
nssm set MiniMES AppRestartDelay 5000

# 启动服务
nssm start MiniMES
```

---

## 🔍 故障排查

### 问题1：证书错误 "The certificate chain was issued by an authority that is not trusted"

**原因**：自签名证书未添加到受信任的根证书颁发机构

**解决方法**：
```powershell
# 方法1：PowerShell添加
$cert = Get-ChildItem -Path "Cert:\LocalMachine\My" | Where-Object {$_.Subject -like "*MiniMES*"}
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root", "LocalMachine")
$store.Open("ReadWrite")
$store.Add($cert)
$store.Close()

# 方法2：手动添加
# 1. Win+R 运行 certmgr.msc
# 2. 展开"个人" → "证书"
# 3. 找到MiniMES证书，右键 → "复制"
# 4. 展开"受信任的根证书颁发机构" → "证书"
# 5. 右键 → "粘贴"
```

### 问题2：端口被占用

**检查端口占用**：
```powershell
# 检查443端口
netstat -ano | findstr :443

# 检查5001端口
netstat -ano | findstr :5001

# 查看进程
tasklist | findstr "进程ID"

# 结束进程
taskkill /PID 进程ID /F
```

### 问题3：NSSM服务无法启动

**查看错误日志**：
```powershell
# 查看服务日志
Get-Content "C:\MiniMES\Logs\error.log" -Tail 50

# 查看Windows事件日志
Get-EventLog -LogName Application -Source "MiniMES" -Newest 10
```

**常见原因**：
- 证书文件路径错误
- 证书密码错误
- 端口被占用
- .NET运行时未安装

---

## 📚 相关命令速查

```powershell
# NSSM服务管理
nssm status MiniMES          # 查看状态
nssm start MiniMES           # 启动服务
nssm stop MiniMES            # 停止服务
nssm restart MiniMES         # 重启服务
nssm remove MiniMES confirm  # 删除服务

# 证书管理
certmgr.msc                  # 打开证书管理器
Get-ChildItem Cert:\LocalMachine\My  # 列出所有证书

# 防火墙管理
Get-NetFirewallRule | Where-Object {$_.DisplayName -like "*MiniMES*"}  # 查看规则
Remove-NetFirewallRule -DisplayName "MiniMES HTTPS"  # 删除规则

# 端口转发管理
netsh interface portproxy show all  # 查看所有转发规则
netsh interface portproxy delete v4tov4 listenport=443 listenaddress=0.0.0.0  # 删除规则
```

---

**最后更新**: 2026-02-05
