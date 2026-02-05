// MiniMES PWA 注册脚本
(function () {
    'use strict';

    // 检查浏览器是否支持Service Worker
    if ('serviceWorker' in navigator) {
        window.addEventListener('load', function () {
            // 注册Service Worker
            navigator.serviceWorker.register('/service-worker.js')
                .then(function (registration) {
                    console.log('[PWA] Service Worker 注册成功:', registration.scope);

                    // 检查更新
                    registration.addEventListener('updatefound', function () {
                        const newWorker = registration.installing;
                        console.log('[PWA] 发现新版本，正在安装...');

                        newWorker.addEventListener('statechange', function () {
                            if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                                // 新版本已安装，提示用户刷新
                                console.log('[PWA] 新版本已就绪，请刷新页面');
                                showUpdateNotification();
                            }
                        });
                    });
                })
                .catch(function (error) {
                    console.error('[PWA] Service Worker 注册失败:', error);
                });

            // 监听Service Worker控制器变化
            navigator.serviceWorker.addEventListener('controllerchange', function () {
                console.log('[PWA] Service Worker 已更新');
            });
        });
    } else {
        console.warn('[PWA] 当前浏览器不支持Service Worker');
    }

    // 显示更新通知
    function showUpdateNotification() {
        // 创建通知元素
        const notification = document.createElement('div');
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: #0d6efd;
            color: white;
            padding: 15px 20px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            z-index: 10000;
            font-size: 14px;
            max-width: 300px;
        `;
        notification.innerHTML = `
            <div style="margin-bottom: 10px;">
                <strong>🎉 新版本可用</strong>
            </div>
            <div style="margin-bottom: 10px; font-size: 13px;">
                点击刷新按钮更新到最新版本
            </div>
            <button onclick="location.reload()" style="
                background: white;
                color: #0d6efd;
                border: none;
                padding: 8px 16px;
                border-radius: 4px;
                cursor: pointer;
                font-weight: bold;
            ">立即刷新</button>
            <button onclick="this.parentElement.remove()" style="
                background: transparent;
                color: white;
                border: 1px solid white;
                padding: 8px 16px;
                border-radius: 4px;
                cursor: pointer;
                margin-left: 8px;
            ">稍后</button>
        `;
        document.body.appendChild(notification);

        // 5秒后自动隐藏
        setTimeout(() => {
            notification.style.opacity = '0';
            notification.style.transition = 'opacity 0.5s';
            setTimeout(() => notification.remove(), 500);
        }, 5000);
    }

    // 检测安装提示
    let deferredPrompt;
    window.addEventListener('beforeinstallprompt', function (e) {
        console.log('[PWA] 检测到安装提示');
        e.preventDefault();
        deferredPrompt = e;
        showInstallButton();
    });

    // 显示安装按钮
    function showInstallButton() {
        const installBtn = document.createElement('button');
        installBtn.id = 'pwa-install-btn';
        installBtn.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            background: #28a745;
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            cursor: pointer;
            font-size: 14px;
            font-weight: bold;
            z-index: 10000;
            display: flex;
            align-items: center;
            gap: 8px;
        `;
        installBtn.innerHTML = '📱 安装到主屏幕';

        installBtn.addEventListener('click', async function () {
            if (!deferredPrompt) return;

            deferredPrompt.prompt();
            const { outcome } = await deferredPrompt.userChoice;
            console.log('[PWA] 用户选择:', outcome);

            deferredPrompt = null;
            installBtn.remove();
        });

        document.body.appendChild(installBtn);
    }

    // 监听安装成功事件
    window.addEventListener('appinstalled', function () {
        console.log('[PWA] 应用已成功安装到主屏幕');
        deferredPrompt = null;
        const installBtn = document.getElementById('pwa-install-btn');
        if (installBtn) installBtn.remove();
    });

})();
