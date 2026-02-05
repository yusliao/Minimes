using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Minimes.Web.Hubs;
using Minimes.Web.Services;
using Moq;
using Xunit;

namespace Minimes.Tests.Web.Services;

/// <summary>
/// DeviceNotificationService单元测试
/// 艹，测试SignalR推送逻辑是否正确
/// </summary>
public class DeviceNotificationServiceTests
{
    private readonly Mock<IHubContext<HardwareHub>> _mockHubContext;
    private readonly Mock<IHubClients> _mockClients;
    private readonly Mock<IClientProxy> _mockClientProxy;
    private readonly Mock<ILogger<DeviceNotificationService>> _mockLogger;
    private readonly DeviceNotificationService _service;

    public DeviceNotificationServiceTests()
    {
        _mockHubContext = new Mock<IHubContext<HardwareHub>>();
        _mockClients = new Mock<IHubClients>();
        _mockClientProxy = new Mock<IClientProxy>();
        _mockLogger = new Mock<ILogger<DeviceNotificationService>>();

        // 设置Mock链：HubContext -> Clients -> All -> ClientProxy
        _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);
        _mockClients.Setup(c => c.All).Returns(_mockClientProxy.Object);

        _service = new DeviceNotificationService(
            _mockHubContext.Object,
            _mockLogger.Object);
    }

    #region NotifyDeviceStatusUpdateAsync Tests

    [Fact]
    public async Task NotifyDeviceStatusUpdateAsync_WithValidData_ShouldCallSendAsync()
    {
        // Arrange
        var deviceId = "device-001";
        var deviceType = "Scale";
        var oldState = "Disconnected";
        var newState = "Connected";

        // Act
        await _service.NotifyDeviceStatusUpdateAsync(deviceId, deviceType, oldState, newState);

        // Assert
        _mockClientProxy.Verify(
            c => c.SendCoreAsync(
                "ReceiveDeviceStatusUpdate",
                It.Is<object[]>(args =>
                    args.Length == 1 &&
                    args[0] != null),
                default),
            Times.Once,
            "应该调用SendCoreAsync推送设备状态更新");
    }

    [Fact]
    public async Task NotifyDeviceStatusUpdateAsync_WithException_ShouldLogError()
    {
        // Arrange
        var deviceId = "device-001";
        var deviceType = "Scale";
        var oldState = "Disconnected";
        var newState = "Connected";

        _mockClientProxy.Setup(c => c.SendCoreAsync(
            It.IsAny<string>(),
            It.IsAny<object[]>(),
            default))
            .ThrowsAsync(new Exception("SignalR推送失败"));

        // Act
        await _service.NotifyDeviceStatusUpdateAsync(deviceId, deviceType, oldState, newState);

        // Assert
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "推送失败时应该记录错误日志");
    }

    #endregion

    #region NotifyDeviceErrorAsync Tests

    [Fact]
    public async Task NotifyDeviceErrorAsync_WithValidData_ShouldCallSendAsync()
    {
        // Arrange
        var deviceId = "device-001";
        var deviceType = "Scale";
        var errorMessage = "设备连接超时";
        var severity = "Error";

        // Act
        await _service.NotifyDeviceErrorAsync(deviceId, deviceType, errorMessage, severity);

        // Assert
        _mockClientProxy.Verify(
            c => c.SendCoreAsync(
                "ReceiveDeviceError",
                It.Is<object[]>(args =>
                    args.Length == 1 &&
                    args[0] != null),
                default),
            Times.Once,
            "应该调用SendCoreAsync推送设备错误");
    }

    [Fact]
    public async Task NotifyDeviceErrorAsync_WithException_ShouldLogError()
    {
        // Arrange
        var deviceId = "device-001";
        var deviceType = "Scale";
        var errorMessage = "设备连接超时";
        var severity = "Error";

        _mockClientProxy.Setup(c => c.SendCoreAsync(
            It.IsAny<string>(),
            It.IsAny<object[]>(),
            default))
            .ThrowsAsync(new Exception("SignalR推送失败"));

        // Act
        await _service.NotifyDeviceErrorAsync(deviceId, deviceType, errorMessage, severity);

        // Assert
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "推送失败时应该记录错误日志");
    }

    #endregion

    #region NotifyDeviceListUpdateAsync Tests

    [Fact]
    public async Task NotifyDeviceListUpdateAsync_ShouldCallSendAsync()
    {
        // Act
        await _service.NotifyDeviceListUpdateAsync();

        // Assert
        _mockClientProxy.Verify(
            c => c.SendCoreAsync(
                "ReceiveDeviceListUpdate",
                It.Is<object[]>(args =>
                    args.Length == 1 &&
                    args[0] != null),
                default),
            Times.Once,
            "应该调用SendCoreAsync推送设备列表更新");
    }

    [Fact]
    public async Task NotifyDeviceListUpdateAsync_WithException_ShouldLogError()
    {
        // Arrange
        _mockClientProxy.Setup(c => c.SendCoreAsync(
            It.IsAny<string>(),
            It.IsAny<object[]>(),
            default))
            .ThrowsAsync(new Exception("SignalR推送失败"));

        // Act
        await _service.NotifyDeviceListUpdateAsync();

        // Assert
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "推送失败时应该记录错误日志");
    }

    #endregion
}
