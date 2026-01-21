using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Minimes.Application.DTOs.WeighingRecord;
using Minimes.Application.Services;
using Minimes.Domain.Entities;
using Minimes.Domain.Enums;
using Minimes.Domain.Interfaces;
using Moq;
using Xunit;

namespace Minimes.Tests.Application.Services;

/// <summary>
/// WeighingRecordService单元测试 - 称重记录业务服务
/// </summary>
public class WeighingRecordServiceTests
{
    private readonly Mock<IWeighingRecordRepository> _mockRecordRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<WeighingRecordService>> _mockLogger;
    private readonly WeighingRecordService _service;

    public WeighingRecordServiceTests()
    {
        _mockRecordRepo = new Mock<IWeighingRecordRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<WeighingRecordService>>();

        _service = new WeighingRecordService(
            _mockRecordRepo.Object,
            _mockProductRepo.Object,
            _mockCustomerRepo.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateRecord()
    {
        // Arrange
        var request = new CreateWeighingRecordRequest
        {
            ProductId = 1,
            CustomerId = 2,
            Weight = 1500m,
            ProcessStage = ProcessStage.Receiving,
            UnitPrice = 10m,
            Remarks = "测试备注"
        };
        var createdBy = "admin";

        var product = new Product
        {
            Id = 1,
            Barcode = "6901234567890",
            Name = "测试商品",
            CreatedAt = DateTime.UtcNow
        };

        var customer = new Customer
        {
            Id = 2,
            Code = "C001",
            Name = "测试客户",
            CreatedAt = DateTime.UtcNow
        };

        var savedRecord = new WeighingRecord
        {
            Id = 100,
            ProductId = request.ProductId,
            CustomerId = request.CustomerId,
            Weight = request.Weight,
            ProcessStage = request.ProcessStage,
            UnitPrice = request.UnitPrice,
            TotalPrice = 15000m, // 1500 * 10
            Remarks = request.Remarks,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        _mockProductRepo.Setup(r => r.GetByIdAsync(request.ProductId))
            .ReturnsAsync(product);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(request.CustomerId!.Value))
            .ReturnsAsync(customer);

        _mockRecordRepo.Setup(r => r.AddAsync(It.IsAny<WeighingRecord>()))
            .ReturnsAsync((WeighingRecord rec) =>
            {
                rec.Id = savedRecord.Id;
                rec.CreatedAt = savedRecord.CreatedAt;
                return rec;
            });

        _mockRecordRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockRecordRepo.Setup(r => r.GetByIdAsync(savedRecord.Id))
            .ReturnsAsync(savedRecord);

        // Act
        var result = await _service.CreateAsync(request, createdBy);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(request.ProductId);
        result.CustomerId.Should().Be(request.CustomerId);
        result.Weight.Should().Be(request.Weight);
        result.ProcessStage.Should().Be(request.ProcessStage);
        result.UnitPrice.Should().Be(request.UnitPrice);
        result.TotalPrice.Should().Be(15000m);
        result.CreatedBy.Should().Be(createdBy);

        _mockRecordRepo.Verify(r => r.AddAsync(It.Is<WeighingRecord>(rec =>
            rec.ProductId == request.ProductId &&
            rec.Weight == request.Weight &&
            rec.TotalPrice == 15000m &&
            rec.CreatedBy == createdBy
        )), Times.Once);

        _mockRecordRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithoutCustomer_ShouldCreateRecord()
    {
        // Arrange
        var request = new CreateWeighingRecordRequest
        {
            ProductId = 1,
            CustomerId = null, // 没有客户
            Weight = 500m,
            ProcessStage = ProcessStage.Processing,
            UnitPrice = 20m
        };
        var createdBy = "operator";

        var product = new Product
        {
            Id = 1,
            Barcode = "6901234567890",
            Name = "测试商品",
            CreatedAt = DateTime.UtcNow
        };

        var savedRecord = new WeighingRecord
        {
            Id = 101,
            ProductId = request.ProductId,
            CustomerId = null,
            Weight = request.Weight,
            ProcessStage = request.ProcessStage,
            UnitPrice = request.UnitPrice,
            TotalPrice = 10000m,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        _mockProductRepo.Setup(r => r.GetByIdAsync(request.ProductId))
            .ReturnsAsync(product);

        _mockRecordRepo.Setup(r => r.AddAsync(It.IsAny<WeighingRecord>()))
            .ReturnsAsync((WeighingRecord rec) =>
            {
                rec.Id = savedRecord.Id;
                rec.CreatedAt = savedRecord.CreatedAt;
                return rec;
            });

        _mockRecordRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockRecordRepo.Setup(r => r.GetByIdAsync(savedRecord.Id))
            .ReturnsAsync(savedRecord);

        // Act
        var result = await _service.CreateAsync(request, createdBy);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().BeNull();
        result.TotalPrice.Should().Be(10000m);

        _mockCustomerRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistingProduct_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateWeighingRecordRequest
        {
            ProductId = 999,
            Weight = 1000m,
            ProcessStage = ProcessStage.Receiving,
            UnitPrice = 10m
        };
        var createdBy = "admin";

        _mockProductRepo.Setup(r => r.GetByIdAsync(request.ProductId))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync(request, createdBy));

        _mockRecordRepo.Verify(r => r.AddAsync(It.IsAny<WeighingRecord>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistingCustomer_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateWeighingRecordRequest
        {
            ProductId = 1,
            CustomerId = 999, // 不存在的客户
            Weight = 1000m,
            ProcessStage = ProcessStage.Receiving,
            UnitPrice = 10m
        };
        var createdBy = "admin";

        var product = new Product
        {
            Id = 1,
            Barcode = "6901234567890",
            Name = "测试商品",
            CreatedAt = DateTime.UtcNow
        };

        _mockProductRepo.Setup(r => r.GetByIdAsync(request.ProductId))
            .ReturnsAsync(product);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(request.CustomerId!.Value))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync(request, createdBy));

        _mockRecordRepo.Verify(r => r.AddAsync(It.IsAny<WeighingRecord>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldCalculateTotalPriceCorrectly()
    {
        // Arrange
        var request = new CreateWeighingRecordRequest
        {
            ProductId = 1,
            Weight = 2500m,
            ProcessStage = ProcessStage.Shipping,
            UnitPrice = 15.5m
        };
        var createdBy = "admin";
        var expectedTotalPrice = 2500m * 15.5m; // 38750

        var product = new Product { Id = 1, Barcode = "TEST", Name = "测试", CreatedAt = DateTime.UtcNow };

        var savedRecord = new WeighingRecord
        {
            Id = 102,
            ProductId = request.ProductId,
            Weight = request.Weight,
            ProcessStage = request.ProcessStage,
            UnitPrice = request.UnitPrice,
            TotalPrice = expectedTotalPrice,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        _mockProductRepo.Setup(r => r.GetByIdAsync(request.ProductId))
            .ReturnsAsync(product);

        _mockRecordRepo.Setup(r => r.AddAsync(It.IsAny<WeighingRecord>()))
            .ReturnsAsync((WeighingRecord rec) =>
            {
                rec.Id = savedRecord.Id;
                rec.CreatedAt = savedRecord.CreatedAt;
                return rec;
            });

        _mockRecordRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockRecordRepo.Setup(r => r.GetByIdAsync(savedRecord.Id)).ReturnsAsync(savedRecord);

        // Act
        var result = await _service.CreateAsync(request, createdBy);

        // Assert
        result.TotalPrice.Should().Be(expectedTotalPrice);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnRecord()
    {
        // Arrange
        var recordId = 1;
        var record = new WeighingRecord
        {
            Id = recordId,
            ProductId = 10,
            CustomerId = 20,
            Weight = 1000m,
            ProcessStage = ProcessStage.Receiving,
            UnitPrice = 10m,
            TotalPrice = 10000m,
            Remarks = "测试",
            CreatedBy = "admin",
            CreatedAt = DateTime.UtcNow
        };

        var product = new Product
        {
            Id = 10,
            Barcode = "6901234567890",
            Name = "测试商品",
            CreatedAt = DateTime.UtcNow
        };

        var customer = new Customer
        {
            Id = 20,
            Code = "C001",
            Name = "测试客户",
            CreatedAt = DateTime.UtcNow
        };

        _mockRecordRepo.Setup(r => r.GetByIdAsync(recordId))
            .ReturnsAsync(record);

        _mockProductRepo.Setup(r => r.GetByIdAsync(record.ProductId))
            .ReturnsAsync(product);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(record.CustomerId!.Value))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.GetByIdAsync(recordId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(recordId);
        result.ProductName.Should().Be(product.Name);
        result.ProductBarcode.Should().Be(product.Barcode);
        result.CustomerName.Should().Be(customer.Name);
        result.ProcessStageName.Should().Be("原料入库");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var recordId = 999;
        _mockRecordRepo.Setup(r => r.GetByIdAsync(recordId))
            .ReturnsAsync((WeighingRecord?)null);

        // Act
        var result = await _service.GetByIdAsync(recordId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithoutCustomer_ShouldReturnRecordWithNullCustomerName()
    {
        // Arrange
        var recordId = 1;
        var record = new WeighingRecord
        {
            Id = recordId,
            ProductId = 10,
            CustomerId = null, // 没有客户
            Weight = 1000m,
            ProcessStage = ProcessStage.Processing,
            UnitPrice = 10m,
            TotalPrice = 10000m,
            CreatedBy = "admin",
            CreatedAt = DateTime.UtcNow
        };

        var product = new Product
        {
            Id = 10,
            Barcode = "6901234567890",
            Name = "测试商品",
            CreatedAt = DateTime.UtcNow
        };

        _mockRecordRepo.Setup(r => r.GetByIdAsync(recordId))
            .ReturnsAsync(record);

        _mockProductRepo.Setup(r => r.GetByIdAsync(record.ProductId))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(recordId);

        // Assert
        result.Should().NotBeNull();
        result!.CustomerId.Should().BeNull();
        result.CustomerName.Should().BeNull();
        result.ProcessStageName.Should().Be("加工过程");

        _mockCustomerRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region QueryAsync Tests

    [Fact]
    public async Task QueryAsync_WithoutFilters_ShouldReturnAllRecordsPaged()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow.AddHours(-2), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 2, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        var product1 = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };
        var product2 = new Product { Id = 2, Barcode = "BAR002", Name = "商品2", CreatedAt = DateTime.UtcNow };

        var request = new WeighingRecordQueryRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product1);
        _mockProductRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product2);

        // Act
        var (resultRecords, totalCount) = await _service.QueryAsync(request);

        // Assert
        totalCount.Should().Be(3);
        resultRecords.Should().HaveCount(3);
        resultRecords[0].Id.Should().Be(3); // 按CreatedAt降序排列
        resultRecords[1].Id.Should().Be(2);
        resultRecords[2].Id.Should().Be(1);
    }

    [Fact]
    public async Task QueryAsync_WithCustomerIdFilter_ShouldReturnFilteredRecords()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, CustomerId = 10, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, CustomerId = 20, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 3, ProductId = 2, CustomerId = 10, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };
        var customer = new Customer { Id = 10, Code = "C001", Name = "客户1", CreatedAt = DateTime.UtcNow };

        var request = new WeighingRecordQueryRequest
        {
            CustomerId = 10,
            PageNumber = 1,
            PageSize = 10
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(records);
        _mockProductRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(customer);

        // Act
        var (resultRecords, totalCount) = await _service.QueryAsync(request);

        // Assert
        totalCount.Should().Be(2);
        resultRecords.Should().HaveCount(2);
        resultRecords.Should().OnlyContain(r => r.CustomerId == 10);
    }

    [Fact]
    public async Task QueryAsync_WithProductIdFilter_ShouldReturnFilteredRecords()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 2, ProductId = 2, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" }
        };

        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };

        var request = new WeighingRecordQueryRequest
        {
            ProductId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(records);
        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var (resultRecords, totalCount) = await _service.QueryAsync(request);

        // Assert
        totalCount.Should().Be(2);
        resultRecords.Should().HaveCount(2);
        resultRecords.Should().OnlyContain(r => r.ProductId == 1);
    }

    [Fact]
    public async Task QueryAsync_WithDateRangeFilter_ShouldReturnFilteredRecords()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = now.AddDays(-5), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = now.AddDays(-2), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = now, CreatedBy = "admin" }
        };

        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };

        var request = new WeighingRecordQueryRequest
        {
            StartDate = now.AddDays(-3),
            EndDate = now.AddDays(1),
            PageNumber = 1,
            PageSize = 10
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(records);
        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var (resultRecords, totalCount) = await _service.QueryAsync(request);

        // Assert
        totalCount.Should().Be(2);
        resultRecords.Should().HaveCount(2);
        resultRecords.Should().Contain(r => r.Id == 2);
        resultRecords.Should().Contain(r => r.Id == 3);
    }

    [Fact]
    public async Task QueryAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var records = Enumerable.Range(1, 25).Select(i => new WeighingRecord
        {
            Id = i,
            ProductId = 1,
            Weight = 1000m * i,
            ProcessStage = ProcessStage.Receiving,
            UnitPrice = 10m,
            TotalPrice = 10000m * i,
            CreatedAt = DateTime.UtcNow.AddHours(-i),
            CreatedBy = "admin"
        }).ToList();

        var product = new Product { Id = 1, Barcode = "BAR001", Name = "商品1", CreatedAt = DateTime.UtcNow };

        var request = new WeighingRecordQueryRequest
        {
            PageNumber = 2,
            PageSize = 10
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(records);
        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var (resultRecords, totalCount) = await _service.QueryAsync(request);

        // Assert
        totalCount.Should().Be(25);
        resultRecords.Should().HaveCount(10); // 第2页，每页10条
        resultRecords[0].Id.Should().Be(11); // 按CreatedAt降序，第2页从第11条开始
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var recordId = 1;
        var record = new WeighingRecord
        {
            Id = recordId,
            ProductId = 1,
            Weight = 1000m,
            ProcessStage = ProcessStage.Receiving,
            UnitPrice = 10m,
            TotalPrice = 10000m,
            CreatedBy = "admin",
            CreatedAt = DateTime.UtcNow
        };

        _mockRecordRepo.Setup(r => r.GetByIdAsync(recordId))
            .ReturnsAsync(record);

        _mockRecordRepo.Setup(r => r.DeleteAsync(It.IsAny<WeighingRecord>()))
            .Returns(Task.CompletedTask);

        _mockRecordRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteAsync(recordId);

        // Assert
        result.Should().BeTrue();
        _mockRecordRepo.Verify(r => r.DeleteAsync(record), Times.Once);
        _mockRecordRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var recordId = 999;
        _mockRecordRepo.Setup(r => r.GetByIdAsync(recordId))
            .ReturnsAsync((WeighingRecord?)null);

        // Act
        var result = await _service.DeleteAsync(recordId);

        // Assert
        result.Should().BeFalse();
        _mockRecordRepo.Verify(r => r.DeleteAsync(It.IsAny<WeighingRecord>()), Times.Never);
    }

    #endregion

    #region GetTodaySummaryAsync Tests

    [Fact]
    public async Task GetTodaySummaryAsync_ShouldReturnCorrectSummary()
    {
        // Arrange
        var today = DateTime.Today;
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, CustomerId = 10, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = today.AddHours(8), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 2, CustomerId = 10, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 15m, TotalPrice = 30000m, CreatedAt = today.AddHours(10), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 1, CustomerId = 20, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 20m, TotalPrice = 30000m, CreatedAt = today.AddHours(14), CreatedBy = "admin" },
            new() { Id = 4, ProductId = 3, Weight = 500m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 5000m, CreatedAt = today.AddDays(-1), CreatedBy = "admin" } // 昨天的记录
        };

        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(records);

        // Act
        var result = await _service.GetTodaySummaryAsync();

        // Assert
        result.TotalRecords.Should().Be(3); // 只统计今天的3条记录
        result.TotalWeight.Should().Be(4500m); // 1000 + 2000 + 1500
        result.TotalAmount.Should().Be(70000m); // 10000 + 30000 + 30000
        result.UniqueCustomers.Should().Be(2); // 客户10和20
        result.UniqueProducts.Should().Be(2); // 商品1和2
    }

    [Fact]
    public async Task GetTodaySummaryAsync_WithNoRecords_ShouldReturnZeroSummary()
    {
        // Arrange
        _mockRecordRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<WeighingRecord>());

        // Act
        var result = await _service.GetTodaySummaryAsync();

        // Assert
        result.TotalRecords.Should().Be(0);
        result.TotalWeight.Should().Be(0);
        result.TotalAmount.Should().Be(0);
        result.UniqueCustomers.Should().Be(0);
        result.UniqueProducts.Should().Be(0);
    }

    #endregion

    #region GetCustomerStatisticsAsync Tests

    [Fact]
    public async Task GetCustomerStatisticsAsync_WithoutDateFilter_ShouldReturnAllStatistics()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, CustomerId = 10, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow.AddDays(-5), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, CustomerId = 10, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = DateTime.UtcNow.AddDays(-3), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 2, CustomerId = 20, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = DateTime.UtcNow.AddDays(-1), CreatedBy = "admin" },
            new() { Id = 4, ProductId = 3, CustomerId = null, Weight = 500m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 5000m, CreatedAt = DateTime.UtcNow, CreatedBy = "admin" } // 无客户记录
        };

        var customer10 = new Customer { Id = 10, Code = "C001", Name = "客户A", CreatedAt = DateTime.UtcNow };
        var customer20 = new Customer { Id = 20, Code = "C002", Name = "客户B", CreatedAt = DateTime.UtcNow };

        _mockRecordRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(records);
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(customer10);
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(customer20);

        // Act
        var result = await _service.GetCustomerStatisticsAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].CustomerId.Should().Be(10);
        result[0].CustomerName.Should().Be("客户A");
        result[0].RecordCount.Should().Be(2);
        result[0].TotalWeight.Should().Be(3000m);
        result[0].TotalAmount.Should().Be(30000m);

        result[1].CustomerId.Should().Be(20);
        result[1].TotalAmount.Should().Be(22500m);
    }

    [Fact]
    public async Task GetCustomerStatisticsAsync_WithDateFilter_ShouldReturnFilteredStatistics()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, CustomerId = 10, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = now.AddDays(-10), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, CustomerId = 10, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = now.AddDays(-2), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 2, CustomerId = 20, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = now.AddDays(-1), CreatedBy = "admin" }
        };

        var customer10 = new Customer { Id = 10, Code = "C001", Name = "客户A", CreatedAt = DateTime.UtcNow };
        var customer20 = new Customer { Id = 20, Code = "C002", Name = "客户B", CreatedAt = DateTime.UtcNow };

        _mockRecordRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(records);
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(customer10);
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(customer20);

        // Act
        var result = await _service.GetCustomerStatisticsAsync(
            startDate: now.AddDays(-5),
            endDate: now);

        // Assert
        result.Should().HaveCount(2);
        result[0].TotalAmount.Should().Be(22500m); // 客户20排第一（金额更高）
        result[1].RecordCount.Should().Be(1); // 客户10只有1条记录在时间范围内
    }

    #endregion

    #region GetProductStatisticsAsync Tests

    [Fact]
    public async Task GetProductStatisticsAsync_WithoutDateFilter_ShouldReturnAllStatistics()
    {
        // Arrange
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = DateTime.UtcNow.AddDays(-5), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = DateTime.UtcNow.AddDays(-3), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 2, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = DateTime.UtcNow.AddDays(-1), CreatedBy = "admin" }
        };

        var product1 = new Product { Id = 1, Barcode = "BAR001", Name = "商品A", CreatedAt = DateTime.UtcNow };
        var product2 = new Product { Id = 2, Barcode = "BAR002", Name = "商品B", CreatedAt = DateTime.UtcNow };

        _mockRecordRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(records);
        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product1);
        _mockProductRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product2);

        // Act
        var result = await _service.GetProductStatisticsAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].ProductId.Should().Be(1);
        result[0].ProductName.Should().Be("商品A");
        result[0].Barcode.Should().Be("BAR001");
        result[0].RecordCount.Should().Be(2);
        result[0].TotalWeight.Should().Be(3000m);
        result[0].TotalAmount.Should().Be(30000m);

        result[1].ProductId.Should().Be(2);
        result[1].TotalAmount.Should().Be(22500m);
    }

    [Fact]
    public async Task GetProductStatisticsAsync_WithDateFilter_ShouldReturnFilteredStatistics()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var records = new List<WeighingRecord>
        {
            new() { Id = 1, ProductId = 1, Weight = 1000m, ProcessStage = ProcessStage.Receiving, UnitPrice = 10m, TotalPrice = 10000m, CreatedAt = now.AddDays(-10), CreatedBy = "admin" },
            new() { Id = 2, ProductId = 1, Weight = 2000m, ProcessStage = ProcessStage.Processing, UnitPrice = 10m, TotalPrice = 20000m, CreatedAt = now.AddDays(-2), CreatedBy = "admin" },
            new() { Id = 3, ProductId = 2, Weight = 1500m, ProcessStage = ProcessStage.Shipping, UnitPrice = 15m, TotalPrice = 22500m, CreatedAt = now.AddDays(-1), CreatedBy = "admin" }
        };

        var product1 = new Product { Id = 1, Barcode = "BAR001", Name = "商品A", CreatedAt = DateTime.UtcNow };
        var product2 = new Product { Id = 2, Barcode = "BAR002", Name = "商品B", CreatedAt = DateTime.UtcNow };

        _mockRecordRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(records);
        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product1);
        _mockProductRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product2);

        // Act
        var result = await _service.GetProductStatisticsAsync(
            startDate: now.AddDays(-5),
            endDate: now);

        // Assert
        result.Should().HaveCount(2);
        result[0].TotalAmount.Should().Be(22500m); // 商品2排第一（金额更高）
        result[1].RecordCount.Should().Be(1); // 商品1只有1条记录在时间范围内
    }

    #endregion
}
