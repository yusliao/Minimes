#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
模拟WiFi电子秤HTTP服务器
用于测试WiFiScaleService，模拟真实电子秤的行为
"""

from http.server import HTTPServer, BaseHTTPRequestHandler
import json
import random
import time
from datetime import datetime

class MockScaleHandler(BaseHTTPRequestHandler):
    # 模拟当前重量（会随机变化）
    current_weight = 0.0
    tare_weight = 0.0

    def do_GET(self):
        """处理GET请求"""
        if self.path == '/api/weight':
            self.send_weight_data()
        elif self.path == '/':
            self.send_response(200)
            self.send_header('Content-type', 'text/html; charset=utf-8')
            self.end_headers()
            html = """
            <html>
            <head><title>模拟WiFi电子秤</title></head>
            <body>
                <h1>🔧 模拟WiFi电子秤服务器</h1>
                <p>当前运行中...</p>
                <ul>
                    <li>获取重量: <code>GET /api/weight</code></li>
                    <li>去皮操作: <code>POST /api/tare</code></li>
                </ul>
            </body>
            </html>
            """
            self.wfile.write(html.encode('utf-8'))
        else:
            self.send_error(404)

    def do_POST(self):
        """处理POST请求"""
        if self.path == '/api/tare':
            # 去皮操作：记录当前重量为皮重
            MockScaleHandler.tare_weight = MockScaleHandler.current_weight

            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.end_headers()

            response = {
                "success": True,
                "message": "去皮成功",
                "tare": MockScaleHandler.tare_weight
            }
            self.wfile.write(json.dumps(response).encode('utf-8'))

            print(f"[{datetime.now().strftime('%H:%M:%S')}] 去皮操作：皮重 = {MockScaleHandler.tare_weight:.2f} kg")
        else:
            self.send_error(404)

    def send_weight_data(self):
        """发送重量数据（模拟真实电子秤）"""
        # 模拟重量变化：在0-20kg之间随机变化
        if MockScaleHandler.current_weight == 0:
            # 初始重量
            MockScaleHandler.current_weight = random.uniform(5.0, 15.0)
        else:
            # 重量随机波动（±0.2kg）
            change = random.uniform(-0.2, 0.2)
            MockScaleHandler.current_weight += change
            MockScaleHandler.current_weight = max(0, min(20, MockScaleHandler.current_weight))

        # 计算净重（去皮后）
        net_weight = MockScaleHandler.current_weight - MockScaleHandler.tare_weight

        # 判断是否稳定（90%概率稳定）
        is_stable = random.random() > 0.1

        # 发送JSON响应（支持多种格式）
        self.send_response(200)
        self.send_header('Content-type', 'application/json')
        self.send_header('Access-Control-Allow-Origin', '*')  # 允许跨域
        self.end_headers()

        # 模拟A&D格式（标准格式）
        response = {
            "weight": round(net_weight, 2),
            "unit": "kg",
            "stable": is_stable,
            "tare": round(MockScaleHandler.tare_weight, 2),
            "timestamp": datetime.now().isoformat()
        }

        self.wfile.write(json.dumps(response).encode('utf-8'))

        # 打印日志
        print(f"[{datetime.now().strftime('%H:%M:%S')}] 重量: {net_weight:.2f} kg | 稳定: {'✓' if is_stable else '✗'} | 皮重: {MockScaleHandler.tare_weight:.2f} kg")

    def log_message(self, format, *args):
        """禁用默认的请求日志（避免刷屏）"""
        pass

def run_server(host='0.0.0.0', port=8080):
    """启动模拟电子秤服务器"""
    server_address = (host, port)
    httpd = HTTPServer(server_address, MockScaleHandler)

    print("=" * 60)
    print("🔧 模拟WiFi电子秤服务器启动成功")
    print("=" * 60)
    print(f"服务地址: http://{host}:{port}")
    print(f"重量API: http://{host}:{port}/api/weight")
    print(f"去皮API: http://{host}:{port}/api/tare (POST)")
    print("=" * 60)
    print(f"量程: 0-20 kg (自动随机变化)")
    print(f"精度: 0.01 kg")
    print(f"稳定率: 90%")
    print("=" * 60)
    print("配置MiniMES系统:")
    print("  Hardware:WiFiScale:IpAddress = \"localhost\" (或你的IP)")
    print("  Hardware:WiFiScale:Port = 8080")
    print("=" * 60)
    print("按 Ctrl+C 停止服务器")
    print("=" * 60)
    print()

    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        print("\n\n服务器已停止")
        httpd.server_close()

if __name__ == '__main__':
    run_server()
