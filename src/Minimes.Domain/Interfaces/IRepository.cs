using Minimes.Domain.Entities;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 通用仓储接口 - 定义基础CRUD操作
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// 添加实体
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// 更新实体
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// 删除实体
    /// </summary>
    Task DeleteAsync(T entity);

    /// <summary>
    /// 保存更改
    /// </summary>
    Task<int> SaveChangesAsync();
}
