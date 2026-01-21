using Minimes.Application.DTOs.Customer;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 客户服务接口
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// 创建客户
    /// </summary>
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);

    /// <summary>
    /// 根据ID获取客户
    /// </summary>
    Task<CustomerResponse?> GetByIdAsync(int id);

    /// <summary>
    /// 根据客户代码获取客户
    /// </summary>
    Task<CustomerResponse?> GetByCodeAsync(string code);

    /// <summary>
    /// 获取所有活跃客户
    /// </summary>
    Task<IEnumerable<CustomerResponse>> GetActiveCustomersAsync();

    /// <summary>
    /// 按名称搜索客户
    /// </summary>
    Task<IEnumerable<CustomerResponse>> SearchByNameAsync(string name);

    /// <summary>
    /// 更新客户
    /// </summary>
    Task<CustomerResponse?> UpdateAsync(UpdateCustomerRequest request);

    /// <summary>
    /// 删除客户
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 检查客户代码是否已存在（用于验证唯一性）
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);

    /// <summary>
    /// 获取所有客户
    /// </summary>
    Task<IEnumerable<CustomerResponse>> GetAllAsync();
}
