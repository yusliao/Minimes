namespace Minimes.Infrastructure.Devices.Protocols;

/// <summary>
/// 通信协议接口
/// </summary>
/// <typeparam name="TRaw">原始数据类型（byte[], string等）</typeparam>
/// <typeparam name="TData">业务数据类型（WeightData, DewPointData等）</typeparam>
public interface IProtocol<TRaw, TData> : IDisposable where TData : class
{
    /// <summary>协议名称</summary>
    string ProtocolName { get; }

    /// <summary>是否已连接</summary>
    bool IsConnected { get; }

    /// <summary>
    /// 建立连接
    /// </summary>
    Task<bool> ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 断开连接
    /// </summary>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取原始数据
    /// </summary>
    Task<TRaw?> ReadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 写入原始数据
    /// </summary>
    Task WriteAsync(TRaw data, CancellationToken cancellationToken = default);

    /// <summary>
    /// 解析原始数据为业务数据
    /// </summary>
    TData? Parse(TRaw rawData);

    /// <summary>
    /// 序列化业务数据为原始数据
    /// </summary>
    TRaw? Serialize(TData data);
}
