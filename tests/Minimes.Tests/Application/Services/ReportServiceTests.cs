using FluentAssertions;
using Microsoft.Extensions.Logging;
using Minimes.Application.DTOs.Report;
using Minimes.Application.Services;
using Minimes.Domain.Entities;
using Minimes.Domain.Enums;
using Minimes.Domain.Interfaces;
using Moq;
using Xunit;

namespace Minimes.Tests.Application.Services;

/// <summary>
/// ReportService单元测试 - 生产报表和质量追溯
/// </summary>
public class ReportServiceTests
{
    private readonly Mock<IWeighingRecordRepository> _mockRecordRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ILogger<ReportService>> _mockLogger;
    private readonly ReportService _service;

    public ReportServiceTests()
    {
        _mockRecordRepo = new Mock<IWeighingRecordRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ReportService>>();

        _service = new ReportService(
            _mockRecordRepo.Object,
            _mockProductRepo.Object,
            _mockLogger.Object);
    }

    #region GetProductionReportAsync Tests

    [Fact]
    public async Task GetProductionReportAsync_WithoutFilters_ShouldReturnCompleteReport()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, CustomerId = 10, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 2, ProductId = 2, CustomerId = 10, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 15m, TotalPrice = 30000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, CustomerId = 20, Weight = 800m, ProcessStage = ProcessStage.Shipping, UnitPrice = 20m, TotalPrice = 16000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 4, ProductId = 3, Weight = 1500m, ProcessStage = ProcessStage.Receiving, UnitPrice = 12m, TotalPrice = 18000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        var request = new ProductionReportRequest();

        // Act
        var result = await _service.GetProductionReportAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.TotalRecords.Should().Be(4);
        result.ReceivingWeight.Should().Be(2500m); // 1000 + 1500
        result.ProcessingWeight.Should().Be(2000m);
        result.ShippingWeight.Should().Be(800m);
        result.TotalAmount.Should().Be(74000m); // 10000 + 30000 + 16000 + 18000
        result.UniqueCustomers.Should().Be(2); // 客户10和20
        result.UniqueProducts.Should().Be(3); // 商品1、2、3
    }

    [Fact]
    public async Task GetProductionReportAsync_WithDateFilter_ShouldReturnFilteredReport()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = now.AddDays(-10), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = now.AddDays(-2), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = now, CreatedBy = "admin" }
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        var request = new ProductionReportRequest
        {
            StartDate = now.AddDays(-5),
            EndDate = now.AddDays(1)
        };

        // Act
        var result = await _service.GetProductionReportAsync(request);

        // Assert
        result.TotalRecords.Should().Be(2); // 只统计过去5天内的记录
        result.ReceivingWeight.Should().Be(0m);
        result.ProcessingWeight.Should().Be(2000m);
        result.ShippingWeight.Should().Be(1500m);
    }

    [Fact]
    public async Task GetProductionReportAsync_WithProductIdFilter_ShouldReturnFilteredReport()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 2, ProductId = 2, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        var request = new ProductionReportRequest
        {
            ProductId = 1
        };

        // Act
        var result = await _service.GetProductionReportAsync(request);

        // Assert
        result.TotalRecords.Should().Be(2);
        result.ReceivingWeight.Should().Be(1000m);
        result.ProcessingWeight.Should().Be(0m);
        result.ShippingWeight.Should().Be(1500m);
        result.UniqueProducts.Should().Be(1);
    }

    [Fact]
    public async Task GetProductionReportAsync_WithCustomerIdFilter_ShouldReturnFilteredReport()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, CustomerId = 10, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, CustomerId = 20, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, CustomerId = 10, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        var request = new ProductionReportRequest
        {
            CustomerId = 10
        };

        // Act
        var result = await _service.GetProductionReportAsync(request);

        // Assert
        result.TotalRecords.Should().Be(2);
        result.ReceivingWeight.Should().Be(1000m);
        result.ShippingWeight.Should().Be(1500m);
        result.UniqueCustomers.Should().Be(1);
    }

    [Fact]
    public async Task GetProductionReportAsync_WithProcessStageFilter_ShouldReturnFilteredReport()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, Weight = 1500m, ProcessStage = ProcessStage.Receiving, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        var request = new ProductionReportRequest
        {
            ProcessStage = ProcessStage.Receiving
        };

        // Act
        var result = await _service.GetProductionReportAsync(request);

        // Assert
        result.TotalRecords.Should().Be(2);
        result.ReceivingWeight.Should().Be(2500m);
        result.ProcessingWeight.Should().Be(0m);
        result.ShippingWeight.Should().Be(0m);
    }

    [Fact]
    public async Task GetProductionReportAsync_WithNoRecords_ShouldReturnZeroReport()
    {
        // Arrange
        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<WeighingRecord>());

        var request = new ProductionReportRequest();

        // Act
        var result = await _service.GetProductionReportAsync(request);

        // Assert
        result.TotalRecords.Should().Be(0);
        result.ReceivingWeight.Should().Be(0m);
        result.ProcessingWeight.Should().Be(0m);
        result.ShippingWeight.Should().Be(0m);
        result.TotalAmount.Should().Be(0m);
        result.UniqueCustomers.Should().Be(0);
        result.UniqueProducts.Should().Be(0);
    }

    #endregion

    #region GetProductLossRateAsync Tests

    [Fact]
    public async Task GetProductLossRateAsync_WithoutDateFilter_ShouldCalculateLossRateCorrectly()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            // 商品1: 入库1000g，出库800g，损耗200g，损耗率20%
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 800m, ProcessStage = ProcessStage.Shipping, UnitPrice = 10m, TotalPrice = 8000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            // 商品2: 入库2000g，出库1500g，损耗500g，损耗率25%
            new() { Id = 3, ProductId = 2, Weight = 2000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 15m, TotalPrice = 30000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 4, ProductId = 2, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        var product1 = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };
        var product2 = new Product { Id = 2, Barcode = "BAR002", Name = "商品2", CreatedAt = DateTime.UtcNow };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product1);
        _mockProductRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product2);

        // Act
        var result = await _service.GetProductLossRateAsync();

        // Assert
        result.Should().HaveCount(2);

        // 按损耗率降序排列，商品2应该排在第一位（25% > 20%）
        result[0].ProductId.Should().Be(2);
        result[0].ProductName.Should().Be("商品2");
        result[0].Barcode.Should().Be("BAR002");
        result[0].ReceivingWeight.Should().Be(2000m);
        result[0].ShippingWeight.Should().Be(1500m);
        result[0].LossWeight.Should().Be(500m);
        result[0].LossRate.Should().Be(25m);
        result[0].ReceivingRecords.Should().Be(1);
        result[0].ShippingRecords.Should().Be(1);

        result[1].ProductId.Should().Be(1);
        result[1].LossRate.Should().Be(20m);
    }

    [Fact]
    public async Task GetProductLossRateAsync_WithProcessingWeight_ShouldIncludeProcessingData()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 950m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 9500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, Weight = 900m, ProcessStage = ProcessStage.Shipping, UnitPrice = 10m, TotalPrice = 9000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _service.GetProductLossRateAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].ProcessingWeight.Should().Be(950m);
        result[0].ProcessingRecords.Should().Be(1);
        result[0].LossRate.Should().Be(10m); // (1000 - 900) / 1000 * 100 = 10%
    }

    [Fact]
    public async Task GetProductLossRateAsync_WithZeroReceivingWeight_ShouldReturnZeroLossRate()
    {
        // Arrange - 只有出库，没有入库
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 800m, ProcessStage = ProcessStage.Shipping, UnitPrice = 10m, TotalPrice = 8000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _service.GetProductLossRateAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].ReceivingWeight.Should().Be(0m);
        result[0].LossWeight.Should().Be(-800m); // 0 - 800
        result[0].LossRate.Should().Be(0m); // 避免除以零
    }

    [Fact]
    public async Task GetProductLossRateAsync_WithDateFilter_ShouldReturnFilteredData()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = now.AddDays(-10), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 500m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 5000m, CreatedAt = now.AddDays(-2), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, Weight = 400m, ProcessStage = ProcessStage.Shipping, UnitPrice = 10m, TotalPrice = 4000m, CreatedAt = now, CreatedBy = "admin" }
        };

        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act - 只统计最近5天
        var result = await _service.GetProductLossRateAsync(
            startDate: now.AddDays(-5),
            endDate: now.AddDays(1));

        // Assert
        result.Should().HaveCount(1);
        result[0].ReceivingWeight.Should().Be(500m); // 只统计最近的入库
        result[0].ShippingWeight.Should().Be(400m);
        result[0].LossRate.Should().Be(20m); // (500 - 400) / 500 * 100
    }

    #endregion

    #region GetProductLossRateByIdAsync Tests

    [Fact]
    public async Task GetProductLossRateByIdAsync_WithValidProductId_ShouldReturnLossRate()
    {
        // Arrange
        var productId = 1;
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 950m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 9500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, Weight = 850m, ProcessStage = ProcessStage.Shipping, UnitPrice = 10m, TotalPrice = 8500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 4, ProductId = 2, Weight = 2000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 15m, TotalPrice = 30000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" } // 其他商品，不应统计
        };

        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        // Act
        var result = await _service.GetProductLossRateByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.ProductId.Should().Be(1);
        result.ProductName.Should().Be("商品1");
        result.Barcode.Should().Be("BAR001");
        result.ReceivingWeight.Should().Be(1000m);
        result.ProcessingWeight.Should().Be(950m);
        result.ShippingWeight.Should().Be(850m);
        result.LossWeight.Should().Be(150m);
        result.LossRate.Should().Be(15m); // (1000 - 850) / 1000 * 100
        result.ReceivingRecords.Should().Be(1);
        result.ProcessingRecords.Should().Be(1);
        result.ShippingRecords.Should().Be(1);
    }

    [Fact]
    public async Task GetProductLossRateByIdAsync_WithNonExistingProduct_ShouldReturnNull()
    {
        // Arrange
        var productId = 999;
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetProductLossRateByIdAsync(productId);

        // Assert
        result.Should().BeNull();
        _mockRecordRepo.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetProductLossRateByIdAsync_WithDateFilter_ShouldReturnFilteredData()
    {
        // Arrange
        var productId = 1;
        var now = DateTime.UtcNow;
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = now.AddDays(-10), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 500m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 5000m, CreatedAt = now.AddDays(-2), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, Weight = 450m, ProcessStage = ProcessStage.Shipping, UnitPrice = 10m, TotalPrice = 4500m, CreatedAt = now, CreatedBy = "admin" }
        };

        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        // Act
        var result = await _service.GetProductLossRateByIdAsync(
            productId,
            startDate: now.AddDays(-5),
            endDate: now.AddDays(1));

        // Assert
        result.Should().NotBeNull();
        result!.ReceivingWeight.Should().Be(500m); // 只统计最近5天的入库
        result.ShippingWeight.Should().Be(450m);
        result.LossRate.Should().Be(10m); // (500 - 450) / 500 * 100
    }

    [Fact]
    public async Task GetProductLossRateByIdAsync_WithNoRecords_ShouldReturnZeroLossRate()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };

        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<WeighingRecord>());

        // Act
        var result = await _service.GetProductLossRateByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.ReceivingWeight.Should().Be(0m);
        result.ProcessingWeight.Should().Be(0m);
        result.ShippingWeight.Should().Be(0m);
        result.LossWeight.Should().Be(0m);
        result.LossRate.Should().Be(0m);
    }

    #endregion
}
