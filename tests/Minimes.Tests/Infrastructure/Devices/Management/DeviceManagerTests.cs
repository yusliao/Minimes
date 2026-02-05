using FluentAssertions;
using Microsoft.Extensions.Logging;
using Minimes.Application.Interfaces;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Management;
using Minimes.Infrastructure.Devices.Models;
using Minimes.Infrastructure.Devices.Models.EventArgs;
using Moq;
using Xunit;

namespace Minimes.Tests.Infrastructure.Devices.Management;

/// <summary>
/// DeviceManager单元测试
/// 艹，测试设备管理器的事件处理和通知推送逻辑
/// </summary>
public class DeviceManagerTests : IDisposable
{
    private readonly Mock<ILogger<DeviceManager>> _mockLogger;
    private readonly Mock<DeviceLogManager> _mockLogManager;
    private readonly Mock<IDeviceNotificationService> _mockNotificationService;
    private readonly DeviceManager _deviceManager;

    public DeviceManagerTests()
    {
        _mockLogger = new Mock<ILogger<DeviceManager>>();

        // DeviceLogManager需要ILogger<DeviceLogManager>
        var mockLogManagerLogger = new Mock<ILogger<DeviceLogManager>>();
        _mockLogManager = new Mock<DeviceLogManager>(mockLogManagerLogger.Object);

        _mockNotificationService = new Mock<IDeviceNotificationService>();

        _deviceManager = new DeviceManager(
            _mockLogger.Object,
            _mockLogManager.Object,
            _mockNotificationService.Object);
    }

    public void Dispose()
    {
        _deviceManager?.Dispose();
    }

    #region RegisterDevice Tests

    [Fact]
    public void RegisterDevice_WithValidDevice_ShouldCallNotifyDeviceListUpdate()
    {
        // Arrange
        var mockDevice = CreateMockDevice("device-001", "Scale");

        // Act
        _deviceManager.RegisterDevice(mockDevice.Object);

        // Assert
        _mockNotificationService.Verify(
            n => n.NotifyDeviceListUpdateAsync(),
            Times.Once,
            "注册设备后应该推送设备列表更新");
    }

    [Fact]
    public void RegisterDevice_WithDuplicateDeviceId_ShouldThrowException()
    {
        // Arrange
        var mockDevice1 = CreateMockDevice("device-001", "Scale");
        var mockDevice2 = CreateMockDevice("device-001", "BarcodeScanner");

        _deviceManager.RegisterDevice(mockDevice1.Object);

        // Act & Assert
        var act = () => _deviceManager.RegisterDevice(mockDevice2.Object);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*设备ID已存在*");
    }

    #endregion

    #region UnregisterDevice Tests

    [Fact]
    public void UnregisterDevice_WithExistingDevice_ShouldCallNotifyDeviceListUpdate()
    {
        // Arrange
        var mockDevice = CreateMockDevice("device-001", "Scale");
        _deviceManager.RegisterDevice(mockDevice.Object);

        // Act
        _deviceManager.UnregisterDevice("device-001");

        // Assert
        _mockNotificationService.Verify(
            n => n.NotifyDeviceListUpdateAsync(),
            Times.Exactly(2), // 一次注册，一次注销
            "注销设备后应该推送设备列表更新");
    }

    [Fact]
    public void UnregisterDevice_WithNonExistingDevice_ShouldNotCallNotifyDeviceListUpdate()
    {
        // Act
        _deviceManager.UnregisterDevice("non-existing-device");

        // Assert
        _mockNotificationService.Verify(
            n => n.NotifyDeviceListUpdateAsync(),
            Times.Never,
            "设备不存在时不应该推送列表更新");
    }

    #endregion

    #region Device Event Tests

    [Fact]
    public void DeviceStatusChanged_ShouldCallNotifyDeviceStatusUpdate()
    {
        // Arrange
        var mockDevice = CreateMockDevice("device-001", "Scale");
        _deviceManager.RegisterDevice(mockDevice.Object);

        var eventArgs = new DeviceStatusEventArgs(
            "device-001",
            DeviceState.Disconnected,
            DeviceState.Connected,
            "设备已连接");

        // Act
        mockDevice.Raise(d => d.StatusChanged += null, mockDevice.Object, eventArgs);

        // 等待异步操作完成（fire-and-forget模式）
        Thread.Sleep(100);

        // Assert
        _mockNotificationService.Verify(
            n => n.NotifyDeviceStatusUpdateAsync(
                "device-001",
                "Scale",
                "Disconnected",
                "Connected"),
            Times.Once,
            "设备状态变化时应该推送状态更新");
    }

    [Fact]
    public void DeviceErrorOccurred_ShouldCallNotifyDeviceError()
    {
        // Arrange
        var mockDevice = CreateMockDevice("device-001", "Scale");
        _deviceManager.RegisterDevice(mockDevice.Object);

        var eventArgs = new DeviceErrorEventArgs(
            "device-001",
            "设备连接超时",
            ErrorSeverity.Error,
            null,
            true);

        // Act
        mockDevice.Raise(d => d.ErrorOccurred += null, mockDevice.Object, eventArgs);

        // 等待异步操作完成（fire-and-forget模式）
        Thread.Sleep(100);

        // Assert
        _mockNotificationService.Verify(
            n => n.NotifyDeviceErrorAsync(
                "device-001",
                "Scale",
                "设备连接超时",
                "Error"),
            Times.Once,
            "设备错误时应该推送错误信息");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 创建Mock设备
    /// 艹，这个辅助方法用于创建测试用的Mock设备
    /// </summary>
    private Mock<IDevice<object>> CreateMockDevice(string deviceId, string deviceType)
    {
        var mockDevice = new Mock<IDevice<object>>();

        // 设置设备ID
        mockDevice.Setup(d => d.DeviceId).Returns(deviceId);

        // 设置设备元数据
        var metadata = new DeviceMetadata
        {
            DeviceType = deviceType,
            DeviceName = $"{deviceType}-{deviceId}",
            Manufacturer = "Test Manufacturer",
            Model = "Test Model",
            ProtocolType = "Test Protocol",
            DataType = "object"
        };
        mockDevice.Setup(d => d.Metadata).Returns(metadata);

        // 设置设备状态
        var status = new DeviceStatus
        {
            State = DeviceState.Disconnected,
            Description = "设备未连接"
        };
        mockDevice.Setup(d => d.Status).Returns(status);

        // 设置IsConnected和IsRunning
        mockDevice.Setup(d => d.IsConnected).Returns(false);
        mockDevice.Setup(d => d.IsRunning).Returns(false);

        return mockDevice;
    }

    #endregion
}
