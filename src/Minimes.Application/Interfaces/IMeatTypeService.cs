using Minimes.Application.DTOs.MeatType;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 肉类类型服务接口 - 肉类类型管理的业务逻辑接口
/// </summary>
public interface IMeatTypeService
{
    /// <summary>
    /// 创建肉类类型
    /// </summary>
    Task<MeatTypeResponse> CreateAsync(CreateMeatTypeRequest request);

    /// <summary>
    /// 根据ID获取肉类类型
    /// </summary>
    Task<MeatTypeResponse?> GetByIdAsync(int id);

    /// <summary>
    /// 根据类型代码获取肉类类型
    /// </summary>
    Task<MeatTypeResponse?> GetByCodeAsync(string code);

    /// <summary>
    /// 获取所有肉类类型（按DisplayOrder排序）
    /// </summary>
    Task<List<MeatTypeResponse>> GetAllAsync();

    /// <summary>
    /// 获取所有激活的肉类类型（按DisplayOrder排序）
    /// </summary>
    Task<List<MeatTypeResponse>> GetActiveTypesAsync();

    /// <summary>
    /// 更新肉类类型
    /// </summary>
    Task<MeatTypeResponse> UpdateAsync(int id, UpdateMeatTypeRequest request);

    /// <summary>
    /// 删除肉类类型（如果被使用则不能删除）
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 切换肉类类型激活状态（至少保留一个激活类型）
    /// </summary>
    Task<bool> ToggleActiveAsync(int id);

    /// <summary>
    /// 重新生成指定肉类类型的所有二维码（为所有激活员工生成）
    /// </summary>
    /// <param name="id">肉类类型ID</param>
    /// <returns>生成的二维码数量</returns>
    Task<int> RegenerateQRCodesAsync(int id);
}
