using Minimes.Domain.Entities;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 客户仓储接口
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// 根据客户编码获取客户
    /// </summary>
    Task<Customer?> GetByCodeAsync(string code);

    /// <summary>
    /// 根据名称搜索客户
    /// </summary>
    Task<IEnumerable<Customer>> SearchByNameAsync(string name);

    /// <summary>
    /// 获取所有激活的客户
    /// </summary>
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();
}
