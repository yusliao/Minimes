using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Minimes.Application.DTOs.Customer;
using Minimes.Application.Services;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;
using Moq;
using Xunit;

namespace Minimes.Tests.Application.Services;

/// <summary>
/// CustomerService单元测试
/// </summary>
public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<IValidator<CreateCustomerRequest>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateCustomerRequest>> _mockUpdateValidator;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockCreateValidator = new Mock<IValidator<CreateCustomerRequest>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateCustomerRequest>>();

        // 默认验证通过
        _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateCustomerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new CustomerService(_mockRepository.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateCustomer()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Code = "C001",
            Name = "测试客户",
            ContactPerson = "张三",
            Phone = "13800138000",
            Address = "测试地址"
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(request.Code))
            .ReturnsAsync((Customer?)null);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be(request.Code);
        result.Name.Should().Be(request.Name);
        result.ContactPerson.Should().Be(request.ContactPerson);
        result.Phone.Should().Be(request.Phone);
        result.Address.Should().Be(request.Address);
        result.IsActive.Should().BeTrue();

        _mockRepository.Verify(r => r.AddAsync(It.Is<Customer>(c =>
            c.Code == request.Code &&
            c.Name == request.Name &&
            c.IsActive == true
        )), Times.Once);

        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Code = "C001",
            Name = "测试客户"
        };

        var existingCustomer = new Customer
        {
            Id = 1,
            Code = "C001",
            Name = "现有客户",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(request.Code))
            .ReturnsAsync(existingCustomer);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldThrowValidationException()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Code = "",
            Name = ""
        };

        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Code", "客户代码不能为空"),
            new ValidationFailure("Name", "客户名称不能为空")
        });

        _mockCreateValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(async () =>
            await _service.CreateAsync(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnCustomer()
    {
        // Arrange
        var customerId = 1;
        var customer = new Customer
        {
            Id = customerId,
            Code = "C001",
            Name = "测试客户",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.GetByIdAsync(customerId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(customerId);
        result.Code.Should().Be(customer.Code);
        result.Name.Should().Be(customer.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var customerId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.GetByIdAsync(customerId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByCodeAsync Tests

    [Fact]
    public async Task GetByCodeAsync_WithExistingCode_ShouldReturnCustomer()
    {
        // Arrange
        var code = "C001";
        var customer = new Customer
        {
            Id = 1,
            Code = code,
            Name = "测试客户",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(code))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.GetByCodeAsync(code);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(code);
    }

    [Fact]
    public async Task GetByCodeAsync_WithNonExistingCode_ShouldReturnNull()
    {
        // Arrange
        var code = "NONEXISTENT";
        _mockRepository.Setup(r => r.GetByCodeAsync(code))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.GetByCodeAsync(code);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetActiveCustomersAsync Tests

    [Fact]
    public async Task GetActiveCustomersAsync_ShouldReturnOnlyActiveCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new() { Id = 1, Code = "C001", Name = "客户1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Code = "C002", Name = "客户2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetActiveCustomersAsync())
            .ReturnsAsync(customers);

        // Act
        var result = await _service.GetActiveCustomersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.IsActive);
    }

    #endregion

    #region SearchByNameAsync Tests

    [Fact]
    public async Task SearchByNameAsync_WithValidName_ShouldReturnMatchingCustomers()
    {
        // Arrange
        var searchName = "测试";
        var customers = new List<Customer>
        {
            new() { Id = 1, Code = "C001", Name = "测试客户1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Code = "C002", Name = "测试客户2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.SearchByNameAsync(searchName))
            .ReturnsAsync(customers);

        // Act
        var result = await _service.SearchByNameAsync(searchName);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Name.Contains(searchName));
    }

    [Fact]
    public async Task SearchByNameAsync_WithEmptyName_ShouldReturnAllCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new() { Id = 1, Code = "C001", Name = "客户1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Code = "C002", Name = "客户2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(customers);

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
    public async Task UpdateAsync_WithValidData_ShouldUpdateCustomer()
    {
        // Arrange
        var request = new UpdateCustomerRequest
        {
            Id = 1,
            Code = "C001",
            Name = "更新后的客户",
            ContactPerson = "李四",
            Phone = "13900139000",
            Address = "新地址",
            IsActive = false
        };

        var existingCustomer = new Customer
        {
            Id = 1,
            Code = "C001",
            Name = "旧客户名",
            ContactPerson = "张三",
            Phone = "13800138000",
            Address = "旧地址",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(existingCustomer);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(request.Name);
        result.ContactPerson.Should().Be(request.ContactPerson);
        result.Phone.Should().Be(request.Phone);
        result.Address.Should().Be(request.Address);
        result.IsActive.Should().Be(request.IsActive);

        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Customer>(c =>
            c.Id == request.Id &&
            c.Name == request.Name &&
            c.IsActive == request.IsActive
        )), Times.Once);

        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var request = new UpdateCustomerRequest
        {
            Id = 999,
            Code = "C999",
            Name = "不存在的客户"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new UpdateCustomerRequest
        {
            Id = 1,
            Code = "C002", // 尝试改成已存在的代码
            Name = "客户1"
        };

        var existingCustomer = new Customer
        {
            Id = 1,
            Code = "C001",
            Name = "客户1",
            CreatedAt = DateTime.UtcNow
        };

        var duplicateCustomer = new Customer
        {
            Id = 2,
            Code = "C002",
            Name = "客户2",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(existingCustomer);

        _mockRepository.Setup(r => r.GetByCodeAsync(request.Code))
            .ReturnsAsync(duplicateCustomer);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.UpdateAsync(request));

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var customerId = 1;
        var customer = new Customer
        {
            Id = customerId,
            Code = "C001",
            Name = "待删除客户",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Customer>()))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteAsync(customerId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(customer), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var customerId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.DeleteAsync(customerId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Customer>()), Times.Never);
    }

    #endregion

    #region CodeExistsAsync Tests

    [Fact]
    public async Task CodeExistsAsync_WithExistingCode_ShouldReturnTrue()
    {
        // Arrange
        var code = "C001";
        var customer = new Customer
        {
            Id = 1,
            Code = code,
            Name = "测试客户",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(code))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.CodeExistsAsync(code);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CodeExistsAsync_WithNonExistingCode_ShouldReturnFalse()
    {
        // Arrange
        var code = "NONEXISTENT";
        _mockRepository.Setup(r => r.GetByCodeAsync(code))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.CodeExistsAsync(code);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CodeExistsAsync_WithExcludeId_ShouldIgnoreSpecifiedCustomer()
    {
        // Arrange
        var code = "C001";
        var excludeId = 1;
        var customer = new Customer
        {
            Id = excludeId,
            Code = code,
            Name = "测试客户",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(code))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.CodeExistsAsync(code, excludeId);

        // Assert
        result.Should().BeFalse(); // 因为找到的客户ID等于excludeId
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new() { Id = 1, Code = "C001", Name = "客户1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Code = "C002", Name = "客户2", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Code = "C003", Name = "客户3", IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(customers);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    #endregion
}
