using Minimes.Domain.Entities;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 二维码仓储接口 - 二维码管理的数据访问接口
/// </summary>
public interface IQRCodeRepository : IRepository<QRCode>
{
    /// <summary>
    /// 根据二维码内容获取二维码
    /// </summary>
    Task<QRCode?> GetByContentAsync(string content);

    /// <summary>
    /// 根据肉类类型ID获取二维码列表
    /// </summary>
    Task<List<QRCode>> GetByMeatTypeIdAsync(int meatTypeId);

    /// <summary>
    /// 根据批次号获取二维码列表
    /// </summary>
    Task<List<QRCode>> GetByBatchNumberAsync(string batchNumber);

    /// <summary>
    /// 获取所有激活的二维码（按创建时间倒序）
    /// </summary>
    Task<List<QRCode>> GetActiveAsync();

    /// <summary>
    /// 更新打印次数
    /// </summary>
    /// <param name="id">二维码ID</param>
    Task UpdatePrintCountAsync(int id);

    /// <summary>
    /// 检查二维码内容是否已存在
    /// </summary>
    /// <param name="content">二维码内容</param>
    /// <param name="excludeId">排除的二维码ID（用于编辑时检查）</param>
    Task<bool> ContentExistsAsync(string content, int? excludeId = null);

    /// <summary>
    /// 获取二维码总数
    /// </summary>
    Task<int> GetQRCodeCountAsync();

    /// <summary>
    /// 获取激活的二维码数量
    /// </summary>
    Task<int> GetActiveQRCodeCountAsync();

    /// <summary>
    /// 根据员工编码获取二维码列表
    /// </summary>
    Task<List<QRCode>> GetByEmployeeCodeAsync(string employeeCode);

    /// <summary>
    /// 根据员工编码停用相关的二维码（级联操作）
    /// </summary>
    Task DeactivateByEmployeeCodeAsync(string employeeCode);

    /// <summary>
    /// 根据肉类类型ID停用相关的二维码（级联操作）
    /// </summary>
    Task DeactivateByMeatTypeIdAsync(int meatTypeId);

    /// <summary>
    /// 获取员工关联的二维码数量
    /// </summary>
    Task<int> GetCountByEmployeeCodeAsync(string employeeCode);
}
