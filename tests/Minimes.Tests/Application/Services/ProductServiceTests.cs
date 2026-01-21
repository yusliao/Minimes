using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Minimes.Application.DTOs.Product;
using Minimes.Application.Services;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;
using Moq;
using Xunit;

namespace Minimes.Tests.Application.Services;

/// <summary>
/// ProductService单元测试
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<IValidator<CreateProductRequest>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateProductRequest>> _mockUpdateValidator;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockCreateValidator = new Mock<IValidator<CreateProductRequest>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateProductRequest>>();

        // 默认验证通过
        _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateProductRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateProductRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new ProductService(_mockRepository.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Barcode = "6901234567890",
            Name = "测试商品",
            Specification = "500g",
            Unit = "kg",
            ReferencePrice = 19.99m
        };

        _mockRepository.Setup(r => r.GetByBarcodeAsync(request.Barcode))
            .ReturnsAsync((Product?)null);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Barcode.Should().Be(request.Barcode);
        result.Name.Should().Be(request.Name);
        result.Specification.Should().Be(request.Specification);
        result.Unit.Should().Be(request.Unit);
        result.ReferencePrice.Should().Be(request.ReferencePrice);
        result.IsActive.Should().BeTrue();

        _mockRepository.Verify(r => r.AddAsync(It.Is<Product>(p =>
            p.Barcode == request.Barcode &&
            p.Name == request.Name &&
            p.IsActive == true
        )), Times.Once);

        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateBarcode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Barcode = "6901234567890",
            Name = "测试商品"
        };

        var existingProduct = new Product
        {
            Id = 1,
            Barcode = "6901234567890",
            Name = "现有商品",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByBarcodeAsync(request.Barcode))
            .ReturnsAsync(existingProduct);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldThrowValidationException()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Barcode = "",
            Name = ""
        };

        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Barcode", "条形码不能为空"),
            new ValidationFailure("Name", "商品名称不能为空")
        });

        _mockCreateValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(async () =>
            await _service.CreateAsync(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnProduct()
    {
        // Arrange
        var productId = 1;
        var product = new Product
        {
            Id = productId,
            Barcode = "6901234567890",
            Name = "测试商品",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Barcode.Should().Be(product.Barcode);
        result.Name.Should().Be(product.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var productId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByBarcodeAsync Tests

    [Fact]
    public async Task GetByBarcodeAsync_WithExistingBarcode_ShouldReturnProduct()
    {
        // Arrange
        var barcode = "6901234567890";
        var product = new Product
        {
            Id = 1,
            Barcode = barcode,
            Name = "测试商品",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByBarcodeAsync(barcode))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result!.Barcode.Should().Be(barcode);
    }

    [Fact]
    public async Task GetByBarcodeAsync_WithNonExistingBarcode_ShouldReturnNull()
    {
        // Arrange
        var barcode = "NONEXISTENT";
        _mockRepository.Setup(r => r.GetByBarcodeAsync(barcode))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetByBarcodeAsync(barcode);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetOrCreateByBarcodeAsync Tests

    [Fact]
    public async Task GetOrCreateByBarcodeAsync_WithExistingBarcode_ShouldReturnExistingProduct()
    {
        // Arrange
        var barcode = "6901234567890";
        var createdBy = "admin";
        var existingProduct = new Product
        {
            Id = 1,
            Barcode = barcode,
            Name = "已存在的商品",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByBarcodeAsync(barcode))
            .ReturnsAsync(existingProduct);

        // Act
        var result = await _service.GetOrCreateByBarcodeAsync(barcode, createdBy);

        // Assert
        result.Should().NotBeNull();
        result.Barcode.Should().Be(barcode);
        result.Name.Should().Be("已存在的商品");

        // 不应该调用AddAsync
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateByBarcodeAsync_WithNewBarcode_ShouldCreateNewProduct()
    {
        // Arrange
        var barcode = "6901234567890";
        var createdBy = "admin";

        _mockRepository.Setup(r => r.GetByBarcodeAsync(barcode))
            .ReturnsAsync((Product?)null);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.GetOrCreateByBarcodeAsync(barcode, createdBy);

        // Assert
        result.Should().NotBeNull();
        result.Barcode.Should().Be(barcode);
        result.Name.Should().Be($"批次-{barcode}"); // 自动生成的名称
        result.Specification.Should().Be("供应商提供");
        result.Unit.Should().Be("克");
        result.ReferencePrice.Should().Be(0);
        result.IsActive.Should().BeTrue();

        _mockRepository.Verify(r => r.AddAsync(It.Is<Product>(p =>
            p.Barcode == barcode &&
            p.Name == $"批次-{barcode}" &&
            p.IsActive == true
        )), Times.Once);

        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region GetActiveProductsAsync Tests

    [Fact]
    public async Task GetActiveProductsAsync_ShouldReturnOnlyActiveProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Barcode = "6901234567890", Name = "商品1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Barcode = "6901234567891", Name = "商品2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetActiveProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _service.GetActiveProductsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.IsActive);
    }

    #endregion

    #region SearchByNameAsync Tests

    [Fact]
    public async Task SearchByNameAsync_WithValidName_ShouldReturnMatchingProducts()
    {
        // Arrange
        var searchName = "测试";
        var products = new List<Product>
        {
            new() { Id = 1, Barcode = "6901234567890", Name = "测试商品1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Barcode = "6901234567891", Name = "测试商品2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.SearchByNameAsync(searchName))
            .ReturnsAsync(products);

        // Act
        var result = await _service.SearchByNameAsync(searchName);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Name.Contains(searchName));
    }

    [Fact]
    public async Task SearchByNameAsync_WithEmptyName_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Barcode = "6901234567890", Name = "商品1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Barcode = "6901234567891", Name = "商品2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _service.SearchByNameAsync("");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var request = new UpdateProductRequest
        {
            Id = 1,
            Barcode = "6901234567890",
            Name = "更新后的商品",
            Specification = "1kg",
            Unit = "kg",
            ReferencePrice = 29.99m,
            IsActive = false
        };

        var existingProduct = new Product
        {
            Id = 1,
            Barcode = "6901234567890",
            Name = "旧商品名",
            Specification = "500g",
            Unit = "g",
            ReferencePrice = 19.99m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(existingProduct);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(request.Name);
        result.Specification.Should().Be(request.Specification);
        result.Unit.Should().Be(request.Unit);
        result.ReferencePrice.Should().Be(request.ReferencePrice);
        result.IsActive.Should().Be(request.IsActive);

        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Product>(p =>
            p.Id == request.Id &&
            p.Name == request.Name &&
            p.IsActive == request.IsActive
        )), Times.Once);

        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var request = new UpdateProductRequest
        {
            Id = 999,
            Barcode = "6901234567890",
            Name = "不存在的商品"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateBarcode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new UpdateProductRequest
        {
            Id = 1,
            Barcode = "6901234567891", // 尝试改成已存在的条形码
            Name = "商品1"
        };

        var existingProduct = new Product
        {
            Id = 1,
            Barcode = "6901234567890",
            Name = "商品1",
            CreatedAt = DateTime.UtcNow
        };

        var duplicateProduct = new Product
        {
            Id = 2,
            Barcode = "6901234567891",
            Name = "商品2",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(existingProduct);

        _mockRepository.Setup(r => r.GetByBarcodeAsync(request.Barcode))
            .ReturnsAsync(duplicateProduct);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.UpdateAsync(request));

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var productId = 1;
        var product = new Product
        {
            Id = productId,
            Barcode = "6901234567890",
            Name = "待删除商品",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteAsync(productId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(product), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var productId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.DeleteAsync(productId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
    }

    #endregion

    #region BarcodeExistsAsync Tests

    [Fact]
    public async Task BarcodeExistsAsync_WithExistingBarcode_ShouldReturnTrue()
    {
        // Arrange
        var barcode = "6901234567890";
        var product = new Product
        {
            Id = 1,
            Barcode = barcode,
            Name = "测试商品",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByBarcodeAsync(barcode))
            .ReturnsAsync(product);

        // Act
        var result = await _service.BarcodeExistsAsync(barcode);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task BarcodeExistsAsync_WithNonExistingBarcode_ShouldReturnFalse()
    {
        // Arrange
        var barcode = "NONEXISTENT";
        _mockRepository.Setup(r => r.GetByBarcodeAsync(barcode))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.BarcodeExistsAsync(barcode);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task BarcodeExistsAsync_WithExcludeId_ShouldIgnoreSpecifiedProduct()
    {
        // Arrange
        var barcode = "6901234567890";
        var excludeId = 1;
        var product = new Product
        {
            Id = excludeId,
            Barcode = barcode,
            Name = "测试商品",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByBarcodeAsync(barcode))
            .ReturnsAsync(product);

        // Act
        var result = await _service.BarcodeExistsAsync(barcode, excludeId);

        // Assert
        result.Should().BeFalse(); // 因为找到的商品ID等于excludeId
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Barcode = "6901234567890", Name = "商品1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Barcode = "6901234567891", Name = "商品2", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Barcode = "6901234567892", Name = "商品3", IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    #endregion
}
