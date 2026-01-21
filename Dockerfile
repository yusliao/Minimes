# ============================================
# MiniMES Docker Multi-Stage Build
# Author: 老王 (暴躁技术流)
# 艹，别tm乱改这个文件，改坏了老子骂死你
# ============================================

# ============================================
# Stage 1: Build Stage (构建阶段)
# 用SDK镜像编译，这玩意儿大但是能编译
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 先复制csproj文件，利用Docker缓存层优化
# 这样依赖不变的情况下不用重新restore，聪明吧
COPY ["src/Minimes.Domain/Minimes.Domain.csproj", "src/Minimes.Domain/"]
COPY ["src/Minimes.Application/Minimes.Application.csproj", "src/Minimes.Application/"]
COPY ["src/Minimes.Infrastructure/Minimes.Infrastructure.csproj", "src/Minimes.Infrastructure/"]
COPY ["src/Minimes.Web/Minimes.Web.csproj", "src/Minimes.Web/"]
COPY ["Minimes.sln", "./"]

# 还原NuGet包（这一步缓存住了，下次就快了）
RUN dotnet restore "Minimes.sln"

# 复制所有源代码
COPY . .

# 编译发布（Release模式，不然老王骂死你）
WORKDIR "/src/src/Minimes.Web"
RUN dotnet publish "Minimes.Web.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ============================================
# Stage 2: Runtime Stage (运行阶段)
# 只用ASP.NET运行时镜像，小得很，200MB左右
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 安装一些必要的工具（调试用，生产可以去掉）
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    && rm -rf /var/lib/apt/lists/*

# 创建非root用户（安全第一，别用root跑服务）
RUN groupadd -r minimes && useradd -r -g minimes minimes

# 创建数据目录
RUN mkdir -p /app/data /app/logs && \
    chown -R minimes:minimes /app

# 从构建阶段复制发布文件
COPY --from=build /app/publish .

# 设置环境变量
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:5000 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    TZ=Asia/Shanghai

# 切换到非root用户
USER minimes

# 暴露端口
EXPOSE 5000

# 健康检查（每30秒检查一次，别太频繁浪费资源）
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# 启动命令
ENTRYPOINT ["dotnet", "Minimes.Web.dll"]
