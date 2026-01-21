using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Minimes.Application.DTOs.User;
using Minimes.Application.Services;
using Minimes.Domain.Entities;
using Minimes.Domain.Enums;
using Minimes.Domain.Interfaces;
using Minimes.Domain.Security;
using Moq;
using Xunit;

namespace Minimes.Tests.Application.Services;

/// <summary>
/// UserService单元测试
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IValidator<RegisterRequest>> _mockRegisterValidator;
    private readonly Mock<IValidator<ChangePasswordRequest>> _mockChangePasswordValidator;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRegisterValidator = new Mock<IValidator<RegisterRequest>>();
        _mockChangePasswordValidator = new Mock<IValidator<ChangePasswordRequest>>();

        // 默认情况下，验证通过
        _mockRegisterValidator.Setup(v => v.ValidateAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockChangePasswordValidator.Setup(v => v.ValidateAsync(It.IsAny<ChangePasswordRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockRegisterValidator.Object,
            _mockChangePasswordValidator.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var username = "testuser";
        var password = "Test123456";
        var fullName = "测试用户";

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _userService.CreateAsync(username, password, fullName);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(username);
        result.FullName.Should().Be(fullName);
        result.Role.Should().Be(UserRole.Operator); // 默认角色
        result.IsActive.Should().BeTrue();

        _mockUserRepository.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Username == username &&
            u.FullName == fullName &&
            !string.IsNullOrEmpty(u.PasswordHash)
        )), Times.Once);
    }

    [Theory]
    [InlineData(null, "password", "Name")]
    [InlineData("", "password", "Name")]
    [InlineData("user", null, "Name")]
    [InlineData("user", "", "Name")]
    [InlineData("user", "password", null)]
    [InlineData("user", "password", "")]
    public async Task CreateAsync_WithInvalidData_ShouldThrowException(string? username, string? password, string? fullName)
    {
        // Act & Assert
        // 测试的是无效数据导致验证失败，但CreateAsync方法本身不检查null
        // 这里用!操作符强制传递，测试验证器的行为
        await Assert.ThrowsAnyAsync<ValidationException>(async () =>
            await _userService.CreateAsync(username!, password!, fullName!));
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnUser()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            FullName = "测试用户",
            Role = UserRole.Administrator,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Username.Should().Be(user.Username);
        result.FullName.Should().Be(user.FullName);
        result.Role.Should().Be(user.Role);
        result.IsActive.Should().Be(user.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByUsernameAsync Tests

    [Fact]
    public async Task GetByUsernameAsync_WithExistingUsername_ShouldReturnUser()
    {
        // Arrange
        var username = "admin";
        var user = new User
        {
            Id = 1,
            Username = username,
            FullName = "管理员",
            Role = UserRole.Administrator,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByUsernameAsync(username);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be(username);
        result.Role.Should().Be(UserRole.Administrator);
    }

    [Fact]
    public async Task GetByUsernameAsync_WithNonExistingUsername_ShouldReturnNull()
    {
        // Arrange
        var username = "nonexistent";
        _mockUserRepository.Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByUsernameAsync(username);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region ValidatePasswordAsync Tests

    [Fact]
    public async Task ValidatePasswordAsync_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var username = "testuser";
        var password = "Test123456";
        var hashedPassword = PasswordHashService.HashPassword(password);

        var user = new User
        {
            Id = 1,
            Username = username,
            PasswordHash = hashedPassword,
            FullName = "测试用户",
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.ValidatePasswordAsync(username, password);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidatePasswordAsync_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var username = "testuser";
        var correctPassword = "Test123456";
        var wrongPassword = "Wrong123456";
        var hashedPassword = PasswordHashService.HashPassword(correctPassword);

        var user = new User
        {
            Id = 1,
            Username = username,
            PasswordHash = hashedPassword,
            FullName = "测试用户",
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.ValidatePasswordAsync(username, wrongPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidatePasswordAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        var username = "nonexistent";
        var password = "Test123456";

        _mockUserRepository.Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.ValidatePasswordAsync(username, password);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ChangePasswordAsync Tests

    [Fact]
    public async Task ChangePasswordAsync_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var userId = 1;
        var oldPassword = "Old123456";
        var newPassword = "New123456";
        var hashedOldPassword = PasswordHashService.HashPassword(oldPassword);

        var user = new User
        {
            Id = userId,
            Username = "testuser",
            PasswordHash = hashedOldPassword,
            FullName = "测试用户",
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.ChangePasswordAsync(userId, oldPassword, newPassword);

        // Assert
        result.Should().BeTrue();
        _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u =>
            u.Id == userId &&
            u.PasswordHash != hashedOldPassword  // 密码应该已经改变
        )), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithIncorrectOldPassword_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var oldPassword = "Old123456";
        var wrongOldPassword = "Wrong123456";
        var newPassword = "New123456";
        var hashedOldPassword = PasswordHashService.HashPassword(oldPassword);

        var user = new User
        {
            Id = userId,
            Username = "testuser",
            PasswordHash = hashedOldPassword,
            FullName = "测试用户",
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.ChangePasswordAsync(userId, wrongOldPassword, newPassword);

        // Assert
        result.Should().BeFalse();
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.ChangePasswordAsync(userId, "Old123", "New123");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ResetPasswordAsync Tests

    [Fact]
    public async Task ResetPasswordAsync_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var userId = 1;
        var newPassword = "Reset123456";

        var user = new User
        {
            Id = userId,
            Username = "testuser",
            PasswordHash = "oldpasswordhash",
            FullName = "测试用户",
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.ResetPasswordAsync(userId, newPassword);

        // Assert
        result.Should().BeTrue();
        _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u =>
            u.Id == userId &&
            u.PasswordHash != "oldpasswordhash"
        )), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.ResetPasswordAsync(userId, "NewPassword123");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var userId = 1;
        var newFullName = "新姓名";
        var newIsActive = false;

        var user = new User
        {
            Id = userId,
            Username = "testuser",
            PasswordHash = "hash",
            FullName = "旧姓名",
            IsActive = true,
            Role = UserRole.Operator,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.UpdateAsync(userId, newFullName, newIsActive);

        // Assert
        result.Should().NotBeNull();
        result!.FullName.Should().Be(newFullName);
        result.IsActive.Should().Be(newIsActive);
        result.UpdatedAt.Should().NotBeNull();

        _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u =>
            u.Id == userId &&
            u.FullName == newFullName &&
            u.IsActive == newIsActive &&
            u.UpdatedAt != null
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingUser_ShouldReturnNull()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateAsync(userId, "新姓名", true);

        // Assert
        result.Should().BeNull();
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Username = "admin", FullName = "管理员", Role = UserRole.Administrator, IsActive = true, CreatedAt = DateTime.UtcNow },
            new User { Id = 2, Username = "operator", FullName = "操作员", Role = UserRole.Operator, IsActive = true, CreatedAt = DateTime.UtcNow },
            new User { Id = 3, Username = "disabled", FullName = "停用用户", Role = UserRole.Operator, IsActive = false, CreatedAt = DateTime.UtcNow }
        };

        _mockUserRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(u => u.Username == "admin");
        result.Should().Contain(u => u.Username == "operator");
        result.Should().Contain(u => u.Username == "disabled");
    }

    #endregion

    #region GetActiveUsersAsync Tests

    [Fact]
    public async Task GetActiveUsersAsync_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Username = "admin", FullName = "管理员", Role = UserRole.Administrator, IsActive = true, CreatedAt = DateTime.UtcNow },
            new User { Id = 2, Username = "operator", FullName = "操作员", Role = UserRole.Operator, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        _mockUserRepository.Setup(r => r.GetActiveUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetActiveUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.IsActive);
    }

    #endregion

    #region UsernameExistsAsync Tests

    [Fact]
    public async Task UsernameExistsAsync_WithExistingUsername_ShouldReturnTrue()
    {
        // Arrange
        var username = "existinguser";
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(username, null))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UsernameExistsAsync(username);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UsernameExistsAsync_WithNonExistingUsername_ShouldReturnFalse()
    {
        // Arrange
        var username = "nonexistent";
        _mockUserRepository.Setup(r => r.UsernameExistsAsync(username, null))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.UsernameExistsAsync(username);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UsernameExistsAsync_WithExcludeId_ShouldCallRepositoryWithExcludeId()
    {
        // Arrange
        var username = "testuser";
        var excludeId = 5;

        _mockUserRepository.Setup(r => r.UsernameExistsAsync(username, excludeId))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.UsernameExistsAsync(username, excludeId);

        // Assert
        result.Should().BeFalse();
        _mockUserRepository.Verify(r => r.UsernameExistsAsync(username, excludeId), Times.Once);
    }

    #endregion
}
