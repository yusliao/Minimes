/**
 * Chart.js 封装工具类
 * 用于在Blazor页面中快速初始化各类图表
 */

// 全局图表实例存储（防止重复创建）
const chartInstances = {};

/**
 * 初始化折线图
 * @param {string} canvasId - canvas元素ID
 * @param {string[]} labels - X轴标签
 * @param {Array} datasets - 数据集数组
 * @param {object} options - Chart.js选项（可选）
 */
function initLineChart(canvasId, labels, datasets, options = {}) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.error(`Canvas element with id '${canvasId}' not found`);
        return;
    }

    // 销毁旧图表
    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
    }

    const defaultOptions = {
        responsive: true,
        maintainAspectRatio: true,
        plugins: {
            legend: {
                display: true,
                position: 'top'
            }
        },
        scales: {
            y: {
                beginAtZero: true
            }
        },
        ...options
    };

    chartInstances[canvasId] = new Chart(canvas, {
        type: 'line',
        data: {
            labels: labels,
            datasets: datasets
        },
        options: defaultOptions
    });
}

/**
 * 初始化饼图
 * @param {string} canvasId - canvas元素ID
 * @param {string[]} labels - 标签
 * @param {number[]} data - 数据值
 * @param {string[]} backgroundColor - 背景颜色数组
 * @param {object} options - Chart.js选项（可选）
 */
function initPieChart(canvasId, labels, data, backgroundColor, options = {}) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.error(`Canvas element with id '${canvasId}' not found`);
        return;
    }

    // 销毁旧图表
    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
    }

    const defaultOptions = {
        responsive: true,
        maintainAspectRatio: true,
        plugins: {
            legend: {
                display: true,
                position: 'right'
            }
        },
        ...options
    };

    chartInstances[canvasId] = new Chart(canvas, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: backgroundColor,
                borderColor: '#fff',
                borderWidth: 2
            }]
        },
        options: defaultOptions
    });
}

/**
 * 初始化柱状图
 * @param {string} canvasId - canvas元素ID
 * @param {string[]} labels - X轴标签
 * @param {Array} datasets - 数据集数组
 * @param {object} options - Chart.js选项（可选）
 */
function initBarChart(canvasId, labels, datasets, options = {}) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.error(`Canvas element with id '${canvasId}' not found`);
        return;
    }

    // 销毁旧图表
    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
    }

    const defaultOptions = {
        responsive: true,
        maintainAspectRatio: true,
        plugins: {
            legend: {
                display: true,
                position: 'top'
            }
        },
        scales: {
            y: {
                beginAtZero: true
            }
        },
        ...options
    };

    chartInstances[canvasId] = new Chart(canvas, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: datasets
        },
        options: defaultOptions
    });
}

/**
 * 销毁指定的图表实例
 * @param {string} canvasId - canvas元素ID
 */
function destroyChart(canvasId) {
    if (chartInstances[canvasId]) {
        chartInstances[canvasId].destroy();
        delete chartInstances[canvasId];
    }
}

/**
 * 销毁所有图表实例
 */
function destroyAllCharts() {
    Object.keys(chartInstances).forEach(key => {
        chartInstances[key].destroy();
    });
    chartInstances = {};
}

// 预定义的颜色配置
const chartColors = {
    // 用于不同环节的颜色
    stages: ['#FF6384', '#36A2EB', '#FFCE56'],
    // 用于肉类类型的颜色集
    types: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40', '#FF6384', '#C9CBCF'],
    // 用于趋势线的颜色
    trend: ['#36A2EB', '#FF6384', '#4BC0C0'],
    // 获取颜色方法
    getColor: function(index) {
        return this.types[index % this.types.length];
    }
};
