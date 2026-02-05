# 局域网服务器静态IP配置指南

> 🔒 让你的服务器IP地址固定不变，避免冲突

---

## 📋 方案对比

| 方案 | 难度 | 优点 | 缺点 | 推荐度 |
|-----|------|------|------|--------|
| **方案1：Windows静态IP** | ⭐ | 配置简单，不依赖路由器 | 需要手动配置DNS | ⭐⭐⭐⭐⭐ |
| **方案2：路由器DHCP保留** | ⭐⭐ | 自动获取DNS，管理方便 | 依赖路由器功能 | ⭐⭐⭐⭐ |

---

## 🎯 方案1：配置Windows静态IP（推荐）

### 适用场景
- 任何局域网环境
- 不需要路由器管理权限
- 快速配置

### 第一步：查看当前网络信息

```powershell
# 以管理员身份运行PowerShell

# 查看网络适配器信息
ipconfig /all

# 或使用更详细的命令
Get-NetIPConfiguration
```

**记录以下信息**（重要！）：
```
当前IP地址：192.168.1.100（示例）
子网掩码：255.255.255.0
默认网关：192.168.1.1
DNS服务器：192.168.1.1（或8.8.8.8）
```

### 第二步：选择合适的静态IP地址

**IP地址选择原则**：

1. **必须在同一网段**
   - 如果网关是 `192.168.1.1`，静态IP应该是 `192.168.1.X`
   - 如果网关是 `192.168.0.1`，静态IP应该是 `192.168.0.X`

2. **避开DHCP分配范围**（重要！）
   - 大多数路由器DHCP范围：`192.168.1.100 ~ 192.168.1.200`
   - 建议使用范围外的IP：`192.168.1.10 ~ 192.168.1.50`

3. **推荐的静态IP地址**：
   ```
   192.168.1.10  （推荐，容易记忆）
   192.168.1.20
   192.168.1.30
   192.168.1.50
   ```

4. **避免使用的IP**：
   ```
   192.168.1.1   （路由器网关）
   192.168.1.255 （广播地址）
   192.168.1.100-200 （DHCP范围，可能冲突）
   ```

### 第三步：配置静态IP（图形界面）

#### 方法A：通过设置界面（推荐新手）

1. **打开网络设置**
   - 按 `Win + I` 打开设置
   - 点击"网络和Internet"
   - 点击"以太网"（有线）或"Wi-Fi"（无线）

2. **编辑IP设置**
   - 找到当前连接的网络
   - 点击"编辑"（IP分配旁边）
   - 选择"手动"

3. **配置IPv4**
   - 打开"IPv4"开关
   - 填写以下信息：
     ```
     IP地址：192.168.1.10
     子网前缀长度：24（对应子网掩码255.255.255.0）
     网关：192.168.1.1
     首选DNS：192.168.1.1（或8.8.8.8）
     备用DNS：8.8.4.4（可选）
     ```

4. **保存设置**
   - 点击"保存"
   - 等待网络重新连接

#### 方法B：通过控制面板（经典方式）

1. **打开网络连接**
   - 按 `Win + R`，输入 `ncpa.cpl`，回车
   - 或右键任务栏网络图标 → "网络和Internet设置" → "高级网络设置" → "更多网络适配器选项"

2. **配置网络适配器**
   - 右键当前使用的网络适配器（以太网或Wi-Fi）
   - 选择"属性"

3. **配置IPv4**
   - 双击"Internet协议版本4 (TCP/IPv4)"
   - 选择"使用下面的IP地址"
   - 填写：
     ```
     IP地址：192.168.1.10
     子网掩码：255.255.255.0
     默认网关：192.168.1.1
     ```
   - 选择"使用下面的DNS服务器地址"
   - 填写：
     ```
     首选DNS服务器：192.168.1.1
     备用DNS服务器：8.8.8.8
     ```

4. **确认保存**
   - 点击"确定"
   - 点击"关闭"

### 第四步：配置静态IP（PowerShell命令）

```powershell
# 以管理员身份运行PowerShell

# 1. 查看网络适配器名称
Get-NetAdapter

# 2. 记录适配器名称（例如："以太网"或"Ethernet"）
$AdapterName = "以太网"

# 3. 移除现有IP配置
Remove-NetIPAddress -InterfaceAlias $AdapterName -Confirm:$false
Remove-NetRoute -InterfaceAlias $AdapterName -Confirm:$false

# 4. 配置静态IP
New-NetIPAddress `
    -InterfaceAlias $AdapterName `
    -IPAddress "192.168.1.10" `
    -PrefixLength 24 `
    -DefaultGateway "192.168.1.1"

# 5. 配置DNS服务器
Set-DnsClientServerAddress `
    -InterfaceAlias $AdapterName `
    -ServerAddresses ("192.168.1.1", "8.8.8.8")

# 6. 验证配置
Get-NetIPConfiguration -InterfaceAlias $AdapterName
```

### 第五步：测试网络连接

```powershell
# 测试网关连接
ping 192.168.1.1

# 测试DNS解析
ping www.baidu.com

# 测试本机IP
ipconfig

# 查看详细信息
ipconfig /all
```

**预期结果**：
```
以太网适配器 以太网:
   IPv4 地址 . . . . . . . . . . . . : 192.168.1.10
   子网掩码  . . . . . . . . . . . . : 255.255.255.0
   默认网关. . . . . . . . . . . . . : 192.168.1.1
```

---

## 🏆 方案2：路由器DHCP保留（备选）

### 适用场景
- 有路由器管理权限
- 希望集中管理IP分配
- 需要自动获取DNS

### 优点
- 服务器仍然使用DHCP，但总是获得相同的IP
- 自动获取DNS服务器
- 路由器重启后IP不变
- 便于集中管理多台设备

### 第一步：查看服务器MAC地址

```powershell
# 查看MAC地址
ipconfig /all

# 或使用
Get-NetAdapter | Select-Object Name, MacAddress
```

**记录MAC地址**（示例）：
```
物理地址：00-15-5D-01-02-03
```

### 第二步：登录路由器管理界面

**常见路由器登录地址**：
```
http://192.168.1.1    （最常见）
http://192.168.0.1
http://192.168.31.1   （小米路由器）
http://192.168.2.1    （华为路由器）
```

**默认用户名密码**（常见）：
```
admin / admin
admin / password
root / admin
（或查看路由器背面标签）
```

### 第三步：配置DHCP保留（不同品牌路由器）

#### TP-Link路由器

1. 登录路由器管理界面
2. 点击"DHCP服务器" → "静态地址分配"
3. 点击"添加新条目"
4. 填写：
   ```
   MAC地址：00-15-5D-01-02-03
   IP地址：192.168.1.10
   状态：生效
   ```
5. 点击"保存"

#### 小米路由器

1. 登录路由器管理界面（miwifi.com）
2. 点击"常用设置" → "局域网设置"
3. 找到"DHCP静态IP分配"
4. 找到服务器设备，点击"+"号
5. 设置固定IP：192.168.1.10
6. 点击"确定"

#### 华为路由器

1. 登录路由器管理界面
2. 点击"更多功能" → "网络设置" → "局域网"
3. 找到"DHCP服务器" → "静态IP地址分配"
4. 点击"添加"
5. 填写MAC地址和IP地址
6. 点击"保存"

#### 通用步骤（其他品牌）

1. 找到"DHCP设置"或"局域网设置"
2. 找到"静态IP分配"、"IP地址保留"或"DHCP保留"
3. 添加新条目：
   - MAC地址：服务器的MAC地址
   - IP地址：想要分配的固定IP
4. 保存并重启路由器

### 第四步：重启服务器网络

```powershell
# 方法1：禁用并启用网络适配器
Disable-NetAdapter -Name "以太网" -Confirm:$false
Start-Sleep -Seconds 3
Enable-NetAdapter -Name "以太网" -Confirm:$false

# 方法2：释放并重新获取IP
ipconfig /release
ipconfig /renew

# 方法3：重启电脑（最彻底）
Restart-Computer
```

### 第五步：验证IP地址

```powershell
# 查看IP地址
ipconfig

# 应该显示保留的IP地址：192.168.1.10
```

---

## 🔍 IP冲突检测和解决

### 检测IP冲突

```powershell
# 方法1：使用arp命令检测
arp -a

# 方法2：使用ping命令测试
ping 192.168.1.10

# 如果有回复，说明该IP已被占用
```

### 解决IP冲突

#### 情况1：配置静态IP前检测

```powershell
# 测试目标IP是否被占用
Test-Connection -ComputerName 192.168.1.10 -Count 2 -Quiet

# 如果返回True，说明IP被占用，换一个IP
# 如果返回False，说明IP可用
```

#### 情况2：配置后出现冲突

**症状**：
- 网络连接不稳定
- 无法访问网络
- Windows提示"IP地址冲突"

**解决方法**：

```powershell
# 1. 查看冲突设备的MAC地址
arp -a | findstr "192.168.1.10"

# 2. 更换静态IP地址
# 重新配置为其他未使用的IP（如192.168.1.11）

# 3. 清除ARP缓存
arp -d

# 4. 刷新网络配置
ipconfig /flushdns
ipconfig /release
ipconfig /renew
```

---

## 📋 完整配置检查清单

配置完成后，请确认：

- [ ] 服务器IP地址固定（192.168.1.10）
- [ ] 能ping通网关（192.168.1.1）
- [ ] 能ping通外网（ping www.baidu.com）
- [ ] 能通过IP访问Web服务（http://192.168.1.10:5000）
- [ ] 手机能通过IP访问（http://192.168.1.10:5000）
- [ ] 重启服务器后IP不变
- [ ] 没有IP冲突警告

---

## 🔧 常用命令速查

```powershell
# 查看当前IP配置
ipconfig
ipconfig /all

# 查看网络适配器
Get-NetAdapter
Get-NetIPConfiguration

# 测试网络连接
ping 192.168.1.1          # 测试网关
ping www.baidu.com        # 测试外网
Test-Connection -ComputerName 192.168.1.10  # 测试IP是否被占用

# 刷新网络配置
ipconfig /release         # 释放IP
ipconfig /renew           # 重新获取IP
ipconfig /flushdns        # 清除DNS缓存

# 查看ARP表（检测IP冲突）
arp -a
arp -d                    # 清除ARP缓存

# 重启网络适配器
Disable-NetAdapter -Name "以太网" -Confirm:$false
Enable-NetAdapter -Name "以太网" -Confirm:$false
```

---

## 🐛 常见问题

### 问题1：配置静态IP后无法上网

**可能原因**：
- 网关地址错误
- DNS服务器错误
- 子网掩码错误

**解决方法**：
```powershell
# 1. 检查网关是否正确
ping 192.168.1.1

# 2. 检查DNS是否正确
nslookup www.baidu.com

# 3. 如果DNS有问题，使用公共DNS
Set-DnsClientServerAddress -InterfaceAlias "以太网" -ServerAddresses ("8.8.8.8", "8.8.4.4")
```

### 问题2：重启后IP地址变回DHCP

**原因**：配置未保存或被覆盖

**解决方法**：
```powershell
# 重新配置静态IP（使用PowerShell命令）
# 确保以管理员身份运行
```

### 问题3：手机无法通过IP访问服务器

**检查清单**：
- [ ] 服务器和手机在同一WiFi网络
- [ ] 服务器防火墙已开放端口（5000或5001）
- [ ] 服务器Web服务正在运行
- [ ] IP地址输入正确

**测试方法**：
```powershell
# 在服务器上测试
Start-Process "http://192.168.1.10:5000"

# 检查防火墙规则
Get-NetFirewallRule | Where-Object {$_.DisplayName -like "*MiniMES*"}

# 检查服务状态
nssm status MiniMES
```

---

**最后更新**: 2026-02-05
