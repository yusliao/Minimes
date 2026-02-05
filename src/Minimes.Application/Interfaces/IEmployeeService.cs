using Minimes.Application.DTOs.Employee;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 员工服务接口
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// 创建员工
    /// </summary>
    Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request);

    /// <summary>
    /// 根据ID获取员工
    /// </summary>
    Task<EmployeeResponse?> GetByIdAsync(int id);

    /// <summary>
    /// 根据员工代码获取员工
    /// </summary>
    Task<EmployeeResponse?> GetByCodeAsync(string code);

    /// <summary>
    /// 获取所有活跃员工
    /// </summary>
    Task<IEnumerable<EmployeeResponse>> GetActiveEmployeesAsync();

    /// <summary>
    /// 按名称搜索员工
    /// </summary>
    Task<IEnumerable<EmployeeResponse>> SearchByNameAsync(string name);

    /// <summary>
    /// 更新员工
    /// </summary>
    Task<EmployeeResponse?> UpdateAsync(UpdateEmployeeRequest request);

    /// <summary>
    /// 删除员工
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 检查员工代码是否已存在（用于验证唯一性）
    /// </summary>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);

    /// <summary>
    /// 获取所有员工
    /// </summary>
    Task<IEnumerable<EmployeeResponse>> GetAllAsync();
}
