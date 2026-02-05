# MiniMES PWA 使用指南

## 📱 什么是PWA？

PWA（Progressive Web App，渐进式Web应用）让你的Web项目可以像原生APP一样使用：
- ✅ 添加到手机主屏幕，点击图标直接打开
- ✅ 全屏显示，没有浏览器地址栏
- ✅ 离线缓存，加载速度更快
- ✅ 无需应用商店，直接通过浏览器安装

---

## 🚀 快速开始

### 第一步：生成APP图标

1. 准备一张正方形图片（推荐1024x1024或512x512）
2. 启动项目后，访问：`http://localhost:5000/icons/generate-icons.html`
3. 上传图片，点击"下载所有图标"
4. 将下载的8个PNG文件放到 `wwwroot/icons/` 目录

**图标尺寸清单**：
- icon-72x72.png
- icon-96x96.png
- icon-128x128.png
- icon-144x144.png
- icon-152x152.png
- icon-192x192.png
- icon-384x384.png
- icon-512x512.png

### 第二步：启动项目

```bash
cd src/Minimes.Web
dotnet run
```

### 第三步：测试PWA功能

#### 在电脑上测试（Chrome浏览器）

1. 打开Chrome浏览器，访问 `http://localhost:5000`
2. 按F12打开开发者工具
3. 切换到"Application"标签
4. 左侧菜单找到"Manifest"，检查配置是否正确
5. 左侧菜单找到"Service Workers"，确认已注册
6. 地址栏右侧会出现"安装"图标（➕），点击即可安装

#### 在手机上测试（推荐）

**Android手机（Chrome浏览器）**：
1. 确保电脑和手机在同一局域网
2. 查看电脑IP地址（如：192.168.1.100）
3. 手机浏览器访问 `http://192.168.1.100:5000`
4. 浏览器会自动弹出"添加到主屏幕"提示
5. 或者点击浏览器菜单 → "添加到主屏幕"
6. 返回主屏幕，点击MiniMES图标，全屏打开！

**iPhone（Safari浏览器）**：
1. 手机Safari访问项目地址
2. 点击底部"分享"按钮
3. 选择"添加到主屏幕"
4. 输入名称，点击"添加"

---

## 🌐 生产环境部署

### 重要：PWA必须使用HTTPS

PWA的Service Worker功能**必须在HTTPS环境下运行**（localhost除外）。

#### 方案1：使用Nginx反向代理（推荐）

```nginx
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

#### 方案2：使用Kestrel内置HTTPS

修改 `appsettings.Production.json`：

```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://*:443",
        "Certificate": {
          "Path": "/path/to/cert.pfx",
          "Password": "your-password"
        }
      }
    }
  }
}
```

#### 方案3：使用Cloudflare（免费SSL）

1. 将域名DNS指向Cloudflare
2. 开启"SSL/TLS"加密模式（Full或Flexible）
3. 自动获得免费HTTPS证书

---

## 🔧 自定义配置

### 修改APP名称和主题色

编辑 `wwwroot/manifest.json`：

```json
{
  "name": "你的APP名称",
  "short_name": "短名称",
  "theme_color": "#你的主题色",
  "background_color": "#背景色"
}
```

### 修改缓存策略

编辑 `wwwroot/service-worker.js`：

```javascript
// 修改版本号（每次更新资源时必须修改）
const CACHE_VERSION = 'minimes-v1.0.1';

// 添加需要缓存的资源
const STATIC_RESOURCES = [
    '/',
    '/manifest.json',
    // 添加更多资源...
];
```

**重要**：每次修改静态资源（CSS/JS/图片）后，必须修改`CACHE_VERSION`，否则用户看到的是旧版本！

---

## 📊 PWA功能检查清单

部署前请确认：

- [ ] 所有图标文件已生成并放置在 `wwwroot/icons/` 目录
- [ ] manifest.json 配置正确（名称、主题色、图标路径）
- [ ] service-worker.js 已配置缓存资源列表
- [ ] _Layout.cshtml 已添加PWA相关meta标签
- [ ] 项目使用HTTPS协议（生产环境）
- [ ] Chrome DevTools → Application → Manifest 显示正常
- [ ] Chrome DevTools → Application → Service Workers 已注册
- [ ] 手机浏览器能正常"添加到主屏幕"

---

## 🐛 常见问题

### 1. 为什么手机浏览器没有"添加到主屏幕"提示？

**可能原因**：
- 未使用HTTPS（生产环境必须）
- manifest.json配置错误
- 图标文件缺失或路径错误
- 浏览器不支持PWA（需Chrome 67+或Safari 11.3+）

**解决方法**：
- 打开Chrome DevTools → Application → Manifest，查看错误提示
- 确认所有图标文件存在
- 使用HTTPS访问

### 2. 修改了代码，但APP显示的还是旧版本？

**原因**：Service Worker缓存了旧版本

**解决方法**：
1. 修改 `service-worker.js` 中的 `CACHE_VERSION`（如：v1.0.0 → v1.0.1）
2. 用户刷新页面时会自动提示更新
3. 或者在Chrome DevTools → Application → Service Workers → 点击"Unregister"

### 3. 如何禁用PWA功能？

删除或注释以下文件引用：
- `_Layout.cshtml` 中的 `<script src="js/pwa.js"></script>`
- `_Layout.cshtml` 中的 `<link rel="manifest" href="manifest.json" />`

---

## 📈 性能优化建议

1. **压缩图标文件**：使用TinyPNG等工具压缩PNG图标
2. **启用Gzip压缩**：在Nginx或IIS中启用Gzip
3. **CDN加速**：将静态资源部署到CDN
4. **预缓存关键资源**：在Service Worker中预缓存首页必需的资源

---

## 📚 相关资源

- [PWA官方文档](https://web.dev/progressive-web-apps/)
- [Manifest配置参考](https://developer.mozilla.org/en-US/docs/Web/Manifest)
- [Service Worker API](https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API)
- [PWA检测工具](https://www.pwabuilder.com/)

---

**最后更新**: 2026-02-05
