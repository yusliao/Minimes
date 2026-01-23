using Minimes.Application.DTOs.QRCode;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 二维码服务接口 - 二维码管理的业务逻辑接口
/// </summary>
public interface IQRCodeService
{
    /// <summary>
    /// 创建单个二维码
    /// </summary>
    Task<QRCodeResponse> CreateAsync(CreateQRCodeRequest request);

    /// <summary>
    /// 批量创建二维码
    /// </summary>
    Task<List<QRCodeResponse>> BatchCreateAsync(BatchCreateQRCodeRequest request);

    /// <summary>
    /// 根据ID获取二维码
    /// </summary>
    Task<QRCodeResponse?> GetByIdAsync(int id);

    /// <summary>
    /// 根据二维码内容获取二维码
    /// </summary>
    Task<QRCodeResponse?> GetByContentAsync(string content);

    /// <summary>
    /// 获取所有二维码（按创建时间倒序）
    /// </summary>
    Task<List<QRCodeResponse>> GetAllAsync();

    /// <summary>
    /// 获取所有激活的二维码（按创建时间倒序）
    /// </summary>
    Task<List<QRCodeResponse>> GetActiveAsync();

    /// <summary>
    /// 更新二维码
    /// </summary>
    Task<QRCodeResponse> UpdateAsync(int id, UpdateQRCodeRequest request);

    /// <summary>
    /// 删除二维码
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 切换二维码激活状态
    /// </summary>
    Task<bool> ToggleActiveAsync(int id);

    /// <summary>
    /// 记录打印（更新打印次数和最后打印时间）
    /// </summary>
    Task RecordPrintAsync(int id);

    /// <summary>
    /// 获取统计信息（总数、激活数、总打印次数）
    /// </summary>
    Task<QRCodeStatistics> GetStatisticsAsync();
}

/// <summary>
/// 二维码统计信息
/// </summary>
public class QRCodeStatistics
{
    /// <summary>
    /// 二维码总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 激活的二维码数量
    /// </summary>
    public int ActiveCount { get; set; }

    /// <summary>
    /// 总打印次数
    /// </summary>
    public int TotalPrintCount { get; set; }
}
