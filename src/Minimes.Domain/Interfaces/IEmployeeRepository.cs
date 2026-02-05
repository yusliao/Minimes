using Minimes.Domain.Entities;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 员工仓储接口
/// </summary>
public interface IEmployeeRepository : IRepository<Employee>
{
    /// <summary>
    /// 根据员工编码获取员工
    /// </summary>
    Task<Employee?> GetByCodeAsync(string code);

    /// <summary>
    /// 根据名称搜索员工
    /// </summary>
    Task<IEnumerable<Employee>> SearchByNameAsync(string name);

    /// <summary>
    /// 获取所有激活的员工
    /// </summary>
    Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
}
