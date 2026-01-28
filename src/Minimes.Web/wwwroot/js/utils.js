// 文件下载工具函数
window.downloadFile = function (fileName, base64String) {
    // 将Base64字符串转换为Blob
    const byteCharacters = atob(base64String);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);

    // 根据文件扩展名选择MIME类型
    let mimeType = 'application/octet-stream';
    if (fileName.endsWith('.xlsx')) {
        mimeType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
    } else if (fileName.endsWith('.db')) {
        mimeType = 'application/x-sqlite3';
    }

    const blob = new Blob([byteArray], { type: mimeType });

    // 创建临时下载链接
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;

    // 触发下载
    document.body.appendChild(link);
    link.click();

    // 清理
    document.body.removeChild(link);
    window.URL.revokeObjectURL(link.href);
};

// 二维码打印工具函数
window.printQRCode = function () {
    const printArea = document.getElementById('printArea');
    if (!printArea) {
        console.error('打印区域未找到');
        return;
    }

    // 保存原始内容
    const originalContents = document.body.innerHTML;
    const printContents = printArea.innerHTML;

    // 替换为打印内容
    document.body.innerHTML = printContents;

    // 执行打印
    window.print();

    // 恢复原始内容
    document.body.innerHTML = originalContents;

    // 重新加载页面以恢复事件绑定
    window.location.reload();
};
