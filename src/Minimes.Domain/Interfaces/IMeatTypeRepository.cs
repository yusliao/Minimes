using Minimes.Domain.Entities;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 肉类类型仓储接口 - 肉类类型管理的数据访问接口
/// </summary>
public interface IMeatTypeRepository : IRepository<MeatType>
{
    /// <summary>
    /// 根据类型代码获取肉类类型
    /// </summary>
    Task<MeatType?> GetByCodeAsync(string code);

    /// <summary>
    /// 获取所有激活的肉类类型（按DisplayOrder排序）
    /// </summary>
    Task<List<MeatType>> GetActiveTypesAsync();

    /// <summary>
    /// 检查类型代码是否已存在
    /// </summary>
    /// <param name="code">类型代码</param>
    /// <param name="excludeId">排除的类型ID（用于编辑时检查）</param>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);

    /// <summary>
    /// 检查肉类类型是否被使用（是否有关联的商品或称重记录）
    /// 预留接口，当前返回false，未来关联Product/WeighingRecord时启用
    /// </summary>
    Task<bool> IsTypeInUseAsync(int typeId);

    /// <summary>
    /// 获取激活肉类类型的数量
    /// </summary>
    Task<int> GetActiveTypeCountAsync();
}
