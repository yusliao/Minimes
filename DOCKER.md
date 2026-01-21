# 🐳 MiniMES Docker 部署指南

> **作者**: 老王 (暴躁技术流)
> **版本**: v1.0.0
> **更新日期**: 2026-01-20
> **艹，Docker部署就是方便，一键搞定！**

---

## 📋 目录

1. [快速开始](#快速开始)
2. [部署方式选择](#部署方式选择)
3. [生产环境部署（MySQL）](#生产环境部署mysql)
4. [开发/演示环境部署（SQLite）](#开发演示环境部署sqlite)
5. [自定义配置](#自定义配置)
6. [运维管理](#运维管理)
7. [故障排查](#故障排查)
8. [硬件设备支持](#硬件设备支持)
9. [常见问题](#常见问题)

---

## 快速开始

### 系统要求

| 组件 | 最低要求 | 推荐配置 |
|------|---------|---------|
| Docker | 20.10+ | 24.0+ |
| Docker Compose | 2.0+ | 2.20+ |
| 内存 | 1 GB | 2 GB |
| 硬盘 | 2 GB | 5 GB |

### 30秒部署（演示环境）

```bash
# 克隆项目
git clone https://github.com/yourusername/minimes.git
cd minimes

# 一键启动（SQLite模式，开箱即用）
docker compose -f docker-compose.dev.yml up -d

# 访问系统
# http://localhost:5000
# 演示账户: demo / demo123
```

**艹，就这么简单！**

---

## 部署方式选择

| 场景 | 推荐方式 | 说明 |
|------|---------|------|
| **快速演示** | `docker-compose.dev.yml` | SQLite，无需数据库，开箱即用 |
| **开发测试** | `docker-compose.dev.yml` | SQLite，方便调试 |
| **生产环境** | `docker-compose.yml` | MySQL，高并发、高可用 |
| **生产+Nginx** | `docker-compose.yml --profile with-nginx` | 带反向代理和HTTPS |

---

## 生产环境部署（MySQL）

### 步骤1：准备环境变量

```bash
# 复制环境变量模板
cp .env.example .env

# 编辑环境变量（一定要改密码！）
nano .env
```

`.env` 文件内容：

```bash
# MySQL配置（艹！生产环境一定要改这些密码！）
MYSQL_ROOT_PASSWORD=YourStrongRootPassword123!
MYSQL_PASSWORD=YourStrongAppPassword456!
```

### 步骤2：启动服务

```bash
# 构建并启动
docker compose up -d

# 查看启动日志
docker compose logs -f

# 等待MySQL初始化完成（大约30秒）
# 看到 "Application started" 就成功了
```

### 步骤3：验证部署

```bash
# 检查服务状态
docker compose ps

# 应该看到：
# NAME          STATUS         PORTS
# minimes-app   Up (healthy)   0.0.0.0:5000->5000/tcp
# minimes-db    Up (healthy)   0.0.0.0:3306->3306/tcp

# 测试访问
curl http://localhost:5000
```

### 步骤4：访问系统

- **地址**: http://服务器IP:5000
- **管理员**: admin / Admin123456
- **操作员**: operator / Operator123456
- **演示账户**: demo / demo123（自动启用硬件模拟）

**⚠️ 首次登录后请立即修改默认密码！**

### 启用Nginx反向代理（可选）

```bash
# 使用 with-nginx profile 启动
docker compose --profile with-nginx up -d

# 访问地址变为：
# http://服务器IP (端口80)
```

---

## 开发/演示环境部署（SQLite）

### 一键启动

```bash
# 构建并启动
docker compose -f docker-compose.dev.yml up -d

# 查看日志
docker compose -f docker-compose.dev.yml logs -f
```

### 停止服务

```bash
docker compose -f docker-compose.dev.yml down
```

### 清理数据重新开始

```bash
# 停止并删除数据卷
docker compose -f docker-compose.dev.yml down -v

# 重新启动
docker compose -f docker-compose.dev.yml up -d
```

---

## 自定义配置

### 修改端口

编辑 `docker-compose.yml`：

```yaml
services:
  minimes:
    ports:
      - "8080:5000"  # 改成8080端口
```

### 修改数据库配置

通过环境变量覆盖：

```yaml
services:
  minimes:
    environment:
      - Database__Provider=MySQL
      - ConnectionStrings__MySqlConnection=Server=外部数据库地址;Port=3306;Database=minimes;User=用户名;Password=密码;CharSet=utf8mb4;
```

### 使用外部MySQL数据库

如果你已经有MySQL数据库，可以直接连接：

```bash
# docker-compose.override.yml
services:
  minimes:
    environment:
      - ConnectionStrings__MySqlConnection=Server=192.168.1.100;Port=3306;Database=minimes;User=minimes;Password=YourPassword;CharSet=utf8mb4;
    depends_on: []  # 移除对内置数据库的依赖

# 不启动内置数据库
# 注释掉 minimes-db 服务
```

### 配置时区

```yaml
environment:
  - TZ=Asia/Shanghai  # 中国时区
  # - TZ=America/New_York  # 美东时区
```

### 配置日志级别

```yaml
environment:
  - Logging__LogLevel__Default=Warning  # 生产环境用Warning
  # - Logging__LogLevel__Default=Debug  # 调试用Debug
```

---

## 运维管理

### 常用命令

```bash
# 查看服务状态
docker compose ps

# 查看日志（实时）
docker compose logs -f

# 只看应用日志
docker compose logs -f minimes

# 重启服务
docker compose restart

# 重启单个服务
docker compose restart minimes

# 停止服务
docker compose stop

# 启动服务
docker compose start

# 停止并删除容器（保留数据）
docker compose down

# 停止并删除所有数据（危险！）
docker compose down -v
```

### 更新应用

```bash
# 拉取最新代码
git pull

# 重新构建并启动
docker compose up -d --build

# 或者分步执行
docker compose build --no-cache
docker compose up -d
```

### 备份数据

#### 备份MySQL数据

```bash
# 创建备份目录
mkdir -p backups

# 导出数据库
docker compose exec minimes-db mysqldump -u root -p minimes > backups/minimes_$(date +%Y%m%d_%H%M%S).sql

# 输入密码后等待导出完成
```

#### 备份SQLite数据

```bash
# 复制数据库文件
docker cp minimes-dev:/app/data/minimes.db backups/minimes_$(date +%Y%m%d_%H%M%S).db
```

### 恢复数据

#### 恢复MySQL数据

```bash
# 导入备份
docker compose exec -T minimes-db mysql -u root -p minimes < backups/minimes_20260120_120000.sql
```

#### 恢复SQLite数据

```bash
# 停止服务
docker compose -f docker-compose.dev.yml stop

# 复制数据库文件
docker cp backups/minimes_20260120_120000.db minimes-dev:/app/data/minimes.db

# 启动服务
docker compose -f docker-compose.dev.yml start
```

### 查看资源使用

```bash
# 查看容器资源使用
docker stats

# 查看容器详情
docker inspect minimes-app
```

---

## 故障排查

### 问题1：容器启动失败

```bash
# 查看详细日志
docker compose logs minimes

# 常见原因：
# 1. 端口被占用 → 修改端口配置
# 2. 数据库连接失败 → 检查数据库是否启动
# 3. 权限问题 → 检查数据卷权限
```

### 问题2：数据库连接失败

```bash
# 检查数据库容器状态
docker compose ps minimes-db

# 查看数据库日志
docker compose logs minimes-db

# 手动测试连接
docker compose exec minimes-db mysql -u minimes -p -e "SELECT 1"
```

### 问题3：健康检查失败

```bash
# 检查健康检查状态
docker inspect --format='{{json .State.Health}}' minimes-app

# 手动测试健康检查端点
docker compose exec minimes curl -f http://localhost:5000/health
```

**注意**：如果应用没有 `/health` 端点，需要添加（见下方说明）。

### 问题4：SignalR/WebSocket连接失败

```bash
# 如果使用Nginx，确保配置了WebSocket支持
# 检查nginx配置中的 proxy_set_header Upgrade 和 Connection
```

### 问题5：磁盘空间不足

```bash
# 清理无用镜像
docker image prune -a

# 清理无用容器
docker container prune

# 清理所有无用资源
docker system prune -a
```

---

## 硬件设备支持

### 连接电子秤（串口设备）

Docker容器默认无法访问主机的串口设备，需要特殊配置：

#### 方法1：设备映射

编辑 `docker-compose.yml`：

```yaml
services:
  minimes:
    # 添加设备映射
    devices:
      - /dev/ttyUSB0:/dev/ttyUSB0  # Linux USB串口
      # - /dev/ttyS0:/dev/ttyS0    # Linux 原生串口
    # 添加权限
    privileged: true  # 或者使用更精细的cap_add
```

#### 方法2：使用host网络模式

```yaml
services:
  minimes:
    network_mode: host
    # 注意：使用host模式后ports配置无效
```

#### 配置串口参数

通过环境变量配置：

```yaml
environment:
  - Hardware__Scale__PortName=/dev/ttyUSB0
  - Hardware__Scale__BaudRate=9600
  - Hardware__Scale__Protocol=Generic
```

### 扫码枪支持

扫码枪通常模拟键盘输入，在容器中需要通过浏览器访问，无需特殊配置。

---

## 添加健康检查端点

如果应用还没有 `/health` 端点，需要在 `Program.cs` 中添加：

```csharp
// 在 app.MapBlazorHub() 之前添加
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
```

或者使用ASP.NET Core内置健康检查：

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// ...

app.MapHealthChecks("/health");
```

---

## 常见问题

### Q1: Docker镜像有多大？

**A**:
- 构建镜像（SDK）: ~700MB（仅构建时使用）
- 运行镜像（Runtime）: ~200MB
- 最终应用: ~250MB（包含应用代码）

### Q2: 如何查看容器内的文件？

```bash
# 进入容器shell
docker compose exec minimes bash

# 查看文件
ls -la /app
```

### Q3: 如何修改应用配置而不重建镜像？

使用环境变量覆盖：

```yaml
environment:
  - Logging__LogLevel__Default=Debug
  - Hardware__Scale__PortName=/dev/ttyUSB0
```

### Q4: 支持Docker Swarm/Kubernetes吗？

**A**: 支持。Dockerfile和镜像都是标准的，可以部署到任何容器编排平台。

### Q5: 如何限制容器资源？

```yaml
services:
  minimes:
    deploy:
      resources:
        limits:
          cpus: '1'
          memory: 512M
        reservations:
          cpus: '0.5'
          memory: 256M
```

### Q6: 如何启用HTTPS？

1. 准备SSL证书（`cert.pem` 和 `key.pem`）
2. 放入 `docker/nginx/ssl/` 目录
3. 编辑 `docker/nginx/nginx.conf` 启用HTTPS配置
4. 启动带Nginx的配置：`docker compose --profile with-nginx up -d`

### Q7: 多实例部署（负载均衡）？

```yaml
services:
  minimes:
    deploy:
      replicas: 3  # 启动3个实例
    # 注意：需要配置外部数据库和会话共享
```

---

## 文件结构

```
minimes/
├── Dockerfile              # Docker镜像构建文件
├── docker-compose.yml      # 生产环境配置（MySQL）
├── docker-compose.dev.yml  # 开发环境配置（SQLite）
├── .dockerignore           # Docker构建忽略文件
├── .env.example            # 环境变量模板
└── docker/
    ├── nginx/
    │   ├── nginx.conf      # Nginx主配置
    │   ├── conf.d/         # Nginx站点配置
    │   └── ssl/            # SSL证书目录
    └── mysql/
        ├── conf.d/
        │   └── custom.cnf  # MySQL自定义配置
        └── init/           # MySQL初始化脚本
```

---

## 快速参考

| 操作 | 命令 |
|------|------|
| 启动（生产） | `docker compose up -d` |
| 启动（开发） | `docker compose -f docker-compose.dev.yml up -d` |
| 停止 | `docker compose down` |
| 重建 | `docker compose up -d --build` |
| 查看日志 | `docker compose logs -f` |
| 进入容器 | `docker compose exec minimes bash` |
| 备份数据库 | `docker compose exec minimes-db mysqldump ...` |

---

**最后更新**: 2026-01-20 | **作者**: 老王 | **许可**: MIT

艹，Docker部署就是香！有问题找老王！
