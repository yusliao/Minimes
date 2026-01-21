@echo off
REM 启动模拟WiFi电子秤服务器（Windows批处理）
REM 需要先安装Python 3

echo.
echo ========================================
echo 启动模拟WiFi电子秤服务器
echo ========================================
echo.

REM 检查Python是否安装
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo 错误：未检测到Python安装
    echo 请从 https://www.python.org/ 下载安装Python 3
    pause
    exit /b 1
)

REM 启动模拟服务器
cd /d %~dp0
python MockWiFiScale.py

pause
