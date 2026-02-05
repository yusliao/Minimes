# MiniMES PWA 快速开始指南

> 📱 让你的Web项目变成手机APP，3步搞定！

---

## 🎯 第一步：生成APP图标（5分钟）

### 方法1：使用内置图标生成工具（推荐）

```bash
# 1. 启动项目
cd D:\MyDomain\src\AI\minimes\src\Minimes.Web
dotnet run

# 2. 浏览器访问图标生成工具
# http://localhost:5000/icons/generate-icons.html
```

**操作步骤**：
1. 准备一张正方形图片（推荐1024x1024，PNG/JPG格式）
2. 打开图标生成工具页面
3. 上传图片
4. 点击"下载所有图标"按钮
5. 将下载的8个PNG文件放到：`src\Minimes.Web\wwwroot\icons\` 目录

**需要的图标文件**：
```
wwwroot/icons/
├── icon-72x72.png
├── icon-96x96.png
├── icon-128x128.png
├── icon-144x144.png
├── icon-152x152.png
├── icon-192x192.png
├── icon-384x384.png
└── icon-512x512.png
```

### 方法2：使用在线工具

访问 https://realfavicongenerator.net/ 或 https://favicon.io/

---

## 🧪 第二步：测试PWA功能（2分钟）

### 电脑测试（Chrome浏览器）

```bash
# 启动项目
cd D:\MyDomain\src\AI\minimes\src\Minimes.Web
dotnet run
```

**测试步骤**：
1. Chrome浏览器访问：`http://localhost:5000`
2. 按 `F12` 打开开发者工具
3. 切换到 `Application` 标签
4. 检查左侧菜单：
   - **Manifest** → 确认配置正确（名称、图标、主题色）
   - **Service Workers** → 确认状态为"activated and is running"
5. 地址栏右侧出现"安装"图标（➕），点击测试安装

### 手机测试（推荐）

```bash
# 1. 查看电脑IP地址
ipconfig

# 2. 找到IPv4地址，例如：192.168.1.100
```

**测试步骤**：
1. 确保手机和电脑在同一WiFi网络
2. 手机浏览器访问：`http://你的IP:5000`（如：`http://192.168.1.100:5000`）
3. **Android手机（Chrome）**：
   - 浏览器自动弹出"添加到主屏幕"提示
   - 或点击菜单 → "添加到主屏幕"
4. **iPhone（Safari）**：
   - 点击底部"分享"按钮
   - 选择"添加到主屏幕"
5. 返回主屏幕，点击MiniMES图标，全屏打开！

---

## 🚀 第三步：部署到生产环境

### ⚠️ 重要：生产环境必须使用HTTPS

PWA的Service Worker功能**必须在HTTPS环境下运行**（localhost除外）。

### 快速部署方案

#### 方案A：使用Nginx反向代理（推荐）

```nginx
# /etc/nginx/sites-available/minimes
server {
    listen 443 ssl http2;
    server_name your-domain.com;

    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

```bash
# 启用配置
sudo ln -s /etc/nginx/sites-available/minimes /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

#### 方案B：使用Cloudflare（免费SSL）

1. 注册Cloudflare账号
2. 添加你的域名
3. 修改域名DNS服务器为Cloudflare提供的地址
4. 在Cloudflare控制台开启"SSL/TLS" → 选择"Full"模式
5. 自动获得免费HTTPS证书

#### 方案C：使用Let's Encrypt（免费证书）

```bash
# 安装Certbot
sudo apt install certbot python3-certbot-nginx

# 自动配置Nginx SSL
sudo certbot --nginx -d your-domain.com

# 自动续期
sudo certbot renew --dry-run
```

---

## 📋 部署检查清单

部署前请确认：

- [ ] 所有8个图标文件已生成并放置在 `wwwroot/icons/` 目录
- [ ] 本地测试通过（Chrome DevTools检查无错误）
- [ ] 手机测试通过（能正常"添加到主屏幕"）
- [ ] 生产环境已配置HTTPS证书
- [ ] 域名DNS已正确解析
- [ ] 防火墙已开放443端口
- [ ] 数据库已切换到MySQL（如需要）

---

## 🔧 常用命令

### 启动项目

```bash
# 开发环境（SQLite）
cd D:\MyDomain\src\AI\minimes\src\Minimes.Web
dotnet run

# 生产环境（MySQL）
set ASPNETCORE_ENVIRONMENT=Production
dotnet run
```

### 编译项目

```bash
cd D:\MyDomain\src\AI\minimes
dotnet build
```

### 发布项目

```bash
cd D:\MyDomain\src\AI\minimes
dotnet publish src/Minimes.Web/Minimes.Web.csproj -c Release -o ./publish
```

### 查看IP地址

```bash
# Windows
ipconfig

# Linux/macOS
ifconfig
# 或
ip addr show
```

---

## 🐛 快速故障排查

### 问题1：手机浏览器没有"添加到主屏幕"提示

**检查清单**：
- [ ] 是否使用HTTPS（生产环境必须）
- [ ] 图标文件是否存在（检查 `wwwroot/icons/` 目录）
- [ ] Chrome DevTools → Application → Manifest 是否有错误
- [ ] 浏览器版本是否支持PWA（Chrome 67+，Safari 11.3+）

**快速修复**：
```bash
# 检查图标文件是否存在
ls D:\MyDomain\src\AI\minimes\src\Minimes.Web\wwwroot\icons\

# 应该看到8个PNG文件
```

### 问题2：修改代码后，APP显示旧版本

**原因**：Service Worker缓存了旧版本

**快速修复**：
1. 打开 `wwwroot/service-worker.js`
2. 修改第3行的版本号：
   ```javascript
   const CACHE_VERSION = 'minimes-v1.0.1'; // 改成新版本号
   ```
3. 重新部署
4. 用户刷新页面会自动提示更新

### 问题3：Chrome DevTools显示Service Worker注册失败

**可能原因**：
- Service Worker文件路径错误
- 浏览器不支持Service Worker
- HTTPS配置问题（生产环境）

**快速修复**：
```bash
# 检查service-worker.js是否存在
ls D:\MyDomain\src\AI\minimes\src\Minimes.Web\wwwroot\service-worker.js

# 检查pwa.js是否存在
ls D:\MyDomain\src\AI\minimes\src\Minimes.Web\wwwroot\js\pwa.js
```

---

## 📚 相关文档

- **完整使用指南**：`PWA_GUIDE.md`（详细配置和高级功能）
- **项目文档**：`CLAUDE.md`（项目架构和开发规范）
- **部署文档**：`DEPLOYMENT.md`（完整部署方案）

---

## 💡 提示

1. **首次部署建议先在局域网测试**，确认功能正常后再部署到公网
2. **图标建议使用简洁的设计**，避免过于复杂的图案（在小尺寸下看不清）
3. **每次更新静态资源（CSS/JS）后记得修改Service Worker版本号**
4. **生产环境建议使用MySQL数据库**，性能更好

---

**最后更新**: 2026-02-05
**作者**: 老王（暴躁技术流）

艹，按照这个文档操作，保证你能把PWA搞定！有问题随时来找老王我！
