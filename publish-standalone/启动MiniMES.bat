@echo off
chcp 65001 >nul
title MiniMES 记账系统

echo ==========================================
echo    MiniMES 记账系统 - 启动中...
echo ==========================================
echo.
echo [信息] 正在启动服务...
echo [信息] 启动后请访问: http://localhost:5000
echo.
echo ------------------------------------------
echo  默认账户:
echo    管理员: admin / Admin123456
echo    演示号: demo / demo123 (自动模拟硬件)
echo ------------------------------------------
echo.
echo 按 Ctrl+C 停止服务
echo.

start http://localhost:5000
Minimes.Web.exe
