// MiniMES PWA Service Worker
// 版本号：每次更新资源时需要修改此版本号
const CACHE_VERSION = 'minimes-v1.0.0';
const CACHE_NAME = `minimes-cache-${CACHE_VERSION}`;

// 需要缓存的静态资源
const STATIC_RESOURCES = [
    '/',
    '/manifest.json',
    '/css/bootstrap/bootstrap.min.css',
    '/css/site.css',
    '/css/tablet.css',
    '/css/devices.css',
    '/js/utils.js',
    '/js/charts.js',
    '/_framework/blazor.server.js',
    '/icons/icon-192x192.png',
    '/icons/icon-512x512.png'
];

// 安装事件：缓存静态资源
self.addEventListener('install', event => {
    console.log('[Service Worker] 安装中...');
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                console.log('[Service Worker] 缓存静态资源');
                return cache.addAll(STATIC_RESOURCES);
            })
            .then(() => self.skipWaiting()) // 立即激活新的Service Worker
    );
});

// 激活事件：清理旧缓存
self.addEventListener('activate', event => {
    console.log('[Service Worker] 激活中...');
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheName !== CACHE_NAME) {
                        console.log('[Service Worker] 删除旧缓存:', cacheName);
                        return caches.delete(cacheName);
                    }
                })
            );
        }).then(() => self.clients.claim()) // 立即控制所有页面
    );
});

// 请求拦截：网络优先策略（适合Blazor Server）
self.addEventListener('fetch', event => {
    // 跳过非GET请求
    if (event.request.method !== 'GET') {
        return;
    }

    // 跳过SignalR连接（Blazor Server必须在线）
    if (event.request.url.includes('/_blazor')) {
        return;
    }

    event.respondWith(
        // 网络优先策略：先尝试网络，失败后使用缓存
        fetch(event.request)
            .then(response => {
                // 如果网络请求成功，更新缓存
                if (response && response.status === 200) {
                    const responseClone = response.clone();
                    caches.open(CACHE_NAME).then(cache => {
                        cache.put(event.request, responseClone);
                    });
                }
                return response;
            })
            .catch(() => {
                // 网络失败，尝试从缓存获取
                return caches.match(event.request).then(cachedResponse => {
                    if (cachedResponse) {
                        return cachedResponse;
                    }
                    // 如果缓存也没有，返回离线页面
                    return caches.match('/');
                });
            })
    );
});

// 消息监听：支持手动更新缓存
self.addEventListener('message', event => {
    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaitng();
    }
});
