using Minimes.Application.DTOs.Device;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 设备管理服务接口
/// </summary>
public interface IDeviceManagementService
{
    #region 查询操作（Operator可用）

    /// <summary>
    /// 获取所有设备信息
    /// </summary>
    Task<IEnumerable<DeviceInfoDto>> GetAllDevicesAsync();

    /// <summary>
    /// 根据设备ID获取设备详情
    /// </summary>
    Task<DeviceDetailDto?> GetDeviceDetailAsync(string deviceId);

    /// <summary>
    /// 获取设备健康状态
    /// </summary>
    Task<DeviceHealthDto?> GetDeviceHealthAsync(string deviceId);

    /// <summary>
    /// 获取设备日志（最近N条）
    /// </summary>
    Task<IEnumerable<DeviceLogDto>> GetDeviceLogsAsync(string deviceId, int limit = 100);

    /// <summary>
    /// 获取设备配置
    /// </summary>
    Task<DeviceConfigurationDto?> GetDeviceConfigurationAsync(string deviceId);

    #endregion

    #region 控制操作（Admin专用）

    /// <summary>
    /// 连接设备
    /// </summary>
    Task<bool> ConnectDeviceAsync(string deviceId);

    /// <summary>
    /// 断开设备连接
    /// </summary>
    Task<bool> DisconnectDeviceAsync(string deviceId);

    /// <summary>
    /// 启动设备
    /// </summary>
    Task<bool> StartDeviceAsync(string deviceId);

    /// <summary>
    /// 停止设备
    /// </summary>
    Task<bool> StopDeviceAsync(string deviceId);

    /// <summary>
    /// 重启设备（断开后重新连接并启动）
    /// </summary>
    Task<bool> RestartDeviceAsync(string deviceId);

    #endregion

    #region 配置操作（Admin专用）

    /// <summary>
    /// 更新设备配置
    /// </summary>
    Task<bool> UpdateDeviceConfigurationAsync(string deviceId, DeviceConfigurationDto configuration);

    #endregion
}
