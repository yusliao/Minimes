using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Minimes.Application.DTOs.Employee;
using Minimes.Application.Services;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;
using Moq;
using Xunit;

namespace Minimes.Tests.Application.Services;

/// <summary>
/// EmployeeService单元测试
/// </summary>
public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _mockRepository;
    private readonly Mock<IQRCodeRepository> _mockQRCodeRepository;
    private readonly Mock<IValidator<CreateEmployeeRequest>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateEmployeeRequest>> _mockUpdateValidator;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _mockRepository = new Mock<IEmployeeRepository>();
        _mockQRCodeRepository = new Mock<IQRCodeRepository>();
        _mockCreateValidator = new Mock<IValidator<CreateEmployeeRequest>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateEmployeeRequest>>();

        // 默认验证通过
        _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateEmployeeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateEmployeeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new EmployeeService(_mockRepository.Object, _mockQRCodeRepository.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateEmployee()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            Code = "E001",
            Name = "测试员工",
            ContactPerson = "张三",
            Phone = "13800138000",
            Address = "测试地址"
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(request.Code))
            .ReturnsAsync((Employee?)null);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Employee>()))
            .ReturnsAsync((Employee e) => e);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockQRCodeRepository.Setup(r => r.GetCountByEmployeeCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(0);

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

        _mockRepository.Verify(r => r.AddAsync(It.Is<Employee>(e =>
            e.Code == request.Code &&
            e.Name == request.Name &&
            e.IsActive == true
        )), Times.Once);

        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            Code = "E001",
            Name = "测试员工"
        };

        var existingEmployee = new Employee
        {
            Id = 1,
            Code = "E001",
            Name = "现有员工",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(request.Code))
            .ReturnsAsync(existingEmployee);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateAsync(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldThrowValidationException()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            Code = "",
            Name = ""
        };

        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Code", "员工代码不能为空"),
            new ValidationFailure("Name", "员工名称不能为空")
        });

        _mockCreateValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(async () =>
            await _service.CreateAsync(request));

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnEmployee()
    {
        // Arrange
        var employeeId = 1;
        var employee = new Employee
        {
            Id = employeeId,
            Code = "E001",
            Name = "测试员工",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        _mockQRCodeRepository.Setup(r => r.GetCountByEmployeeCodeAsync(employee.Code))
            .ReturnsAsync(5);

        // Act
        var result = await _service.GetByIdAsync(employeeId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(employeeId);
        result.Code.Should().Be(employee.Code);
        result.Name.Should().Be(employee.Name);
        result.QRCodeCount.Should().Be(5);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var employeeId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.GetByIdAsync(employeeId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByCodeAsync Tests

    [Fact]
    public async Task GetByCodeAsync_WithExistingCode_ShouldReturnEmployee()
    {
        // Arrange
        var code = "E001";
        var employee = new Employee
        {
            Id = 1,
            Code = code,
            Name = "测试员工",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(code))
            .ReturnsAsync(employee);

        _mockQRCodeRepository.Setup(r => r.GetCountByEmployeeCodeAsync(code))
            .ReturnsAsync(3);

        // Act
        var result = await _service.GetByCodeAsync(code);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(code);
        result.QRCodeCount.Should().Be(3);
    }

    [Fact]
    public async Task GetByCodeAsync_WithNonExistingCode_ShouldReturnNull()
    {
        // Arrange
        var code = "NONEXISTENT";
        _mockRepository.Setup(r => r.GetByCodeAsync(code))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.GetByCodeAsync(code);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetActiveEmployeesAsync Tests

    [Fact]
    public async Task GetActiveEmployeesAsync_ShouldReturnOnlyActiveEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new() { Id = 1, Code = "E001", Name = "员工1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Code = "E002", Name = "员工2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetActiveEmployeesAsync())
            .ReturnsAsync(employees);

        _mockQRCodeRepository.Setup(r => r.GetCountByEmployeeCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(2);

        // Act
        var result = await _service.GetActiveEmployeesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.IsActive);
    }

    #endregion

    #region SearchByNameAsync Tests

    [Fact]
    public async Task SearchByNameAsync_WithValidName_ShouldReturnMatchingEmployees()
    {
        // Arrange
        var searchName = "测试";
        var employees = new List<Employee>
        {
            new() { Id = 1, Code = "E001", Name = "测试员工1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Code = "E002", Name = "测试员工2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.SearchByNameAsync(searchName))
            .ReturnsAsync(employees);

        _mockQRCodeRepository.Setup(r => r.GetCountByEmployeeCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.SearchByNameAsync(searchName);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.Name.Contains(searchName));
    }

    [Fact]
    public async Task SearchByNameAsync_WithEmptyName_ShouldReturnAllEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new() { Id = 1, Code = "E001", Name = "员工1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Code = "E002", Name = "员工2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(employees);

        _mockQRCodeRepository.Setup(r => r.GetCountByEmployeeCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(0);

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
    public async Task UpdateAsync_WithValidData_ShouldUpdateEmployee()
    {
        // Arrange
        var request = new UpdateEmployeeRequest
        {
            Id = 1,
            Code = "E001",
            Name = "更新后的员工",
            ContactPerson = "李四",
            Phone = "13900139000",
            Address = "新地址",
            IsActive = true
        };

        var existingEmployee = new Employee
        {
            Id = 1,
            Code = "E001",
            Name = "旧员工名",
            ContactPerson = "张三",
            Phone = "13800138000",
            Address = "旧地址",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(existingEmployee);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockQRCodeRepository.Setup(r => r.GetCountByEmployeeCodeAsync(existingEmployee.Code))
            .ReturnsAsync(3);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(request.Name);
        result.ContactPerson.Should().Be(request.ContactPerson);
        result.Phone.Should().Be(request.Phone);
        result.Address.Should().Be(request.Address);
        result.IsActive.Should().Be(request.IsActive);

        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Employee>(e =>
            e.Id == request.Id &&
            e.Name == request.Name &&
            e.IsActive == request.IsActive
        )), Times.Once);

        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenDeactivatingEmployee_ShouldDeactivateRelatedQRCodes()
    {
        // Arrange
        var request = new UpdateEmployeeRequest
        {
            Id = 1,
            Code = "E001",
            Name = "员工1",
            IsActive = false // 停用员工
        };

        var existingEmployee = new Employee
        {
            Id = 1,
            Code = "E001",
            Name = "员工1",
            IsActive = true, // 原本是激活的
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(existingEmployee);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockQRCodeRepository.Setup(r => r.DeactivateByEmployeeCodeAsync(existingEmployee.Code))
            .Returns(Task.CompletedTask);

        _mockQRCodeRepository.Setup(r => r.GetCountByEmployeeCodeAsync(existingEmployee.Code))
            .ReturnsAsync(5);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.IsActive.Should().BeFalse();

        // 验证级联停用QRCode被调用
        _mockQRCodeRepository.Verify(r => r.DeactivateByEmployeeCodeAsync(existingEmployee.Code), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var request = new UpdateEmployeeRequest
        {
            Id = 999,
            Code = "E999",
            Name = "不存在的员工"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.UpdateAsync(request);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateCode_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new UpdateEmployeeRequest
        {
            Id = 1,
            Code = "E002", // 尝试改成已存在的代码
            Name = "员工1"
        };

        var existingEmployee = new Employee
        {
            Id = 1,
            Code = "E001",
            Name = "员工1",
            CreatedAt = DateTime.UtcNow
        };

        var duplicateEmployee = new Employee
        {
            Id = 2,
            Code = "E002",
            Name = "员工2",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(existingEmployee);

        _mockRepository.Setup(r => r.GetByCodeAsync(request.Code))
            .ReturnsAsync(duplicateEmployee);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.UpdateAsync(request));

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Never);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var employeeId = 1;
        var employee = new Employee
        {
            Id = employeeId,
            Code = "E001",
            Name = "待删除员工",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Employee>()))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockQRCodeRepository.Setup(r => r.DeactivateByEmployeeCodeAsync(employee.Code))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(employeeId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(employee), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

        // 验证级联停用QRCode被调用
        _mockQRCodeRepository.Verify(r => r.DeactivateByEmployeeCodeAsync(employee.Code), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeactivateRelatedQRCodes()
    {
        // Arrange
        var employeeId = 1;
        var employee = new Employee
        {
            Id = employeeId,
            Code = "E001",
            Name = "待删除员工",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
            .ReturnsAsync(employee);

        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Employee>()))
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockQRCodeRepository.Setup(r => r.DeactivateByEmployeeCodeAsync(employee.Code))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(employeeId);

        // Assert
        result.Should().BeTrue();

        // 验证删除前先停用相关QRCode
        _mockQRCodeRepository.Verify(r => r.DeactivateByEmployeeCodeAsync(employee.Code), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var employeeId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(employeeId))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.DeleteAsync(employeeId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Employee>()), Times.Never);
        _mockQRCodeRepository.Verify(r => r.DeactivateByEmployeeCodeAsync(It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region CodeExistsAsync Tests

    [Fact]
    public async Task CodeExistsAsync_WithExistingCode_ShouldReturnTrue()
    {
        // Arrange
        var code = "E001";
        var employee = new Employee
        {
            Id = 1,
            Code = code,
            Name = "测试员工",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(code))
            .ReturnsAsync(employee);

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
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.CodeExistsAsync(code);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CodeExistsAsync_WithExcludeId_ShouldIgnoreSpecifiedEmployee()
    {
        // Arrange
        var code = "E001";
        var excludeId = 1;
        var employee = new Employee
        {
            Id = excludeId,
            Code = code,
            Name = "测试员工",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByCodeAsync(code))
            .ReturnsAsync(employee);

        // Act
        var result = await _service.CodeExistsAsync(code, excludeId);

        // Assert
        result.Should().BeFalse(); // 因为找到的员工ID等于excludeId
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new() { Id = 1, Code = "E001", Name = "员工1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Code = "E002", Name = "员工2", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Code = "E003", Name = "员工3", IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(employees);

        _mockQRCodeRepository.Setup(r => r.GetCountByEmployeeCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(2);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    #endregion
}

