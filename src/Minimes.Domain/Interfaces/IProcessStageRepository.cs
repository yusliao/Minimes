using Minimes.Domain.Entities;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 工序仓储接口 - 工序管理的数据访问接口
/// </summary>
public interface IProcessStageRepository : IRepository<ProcessStage>
{
    /// <summary>
    /// 根据工序代码获取工序
    /// </summary>
    Task<ProcessStage?> GetByCodeAsync(string code);

    /// <summary>
    /// 获取所有激活的工序（按DisplayOrder排序）
    /// </summary>
    Task<List<ProcessStage>> GetActiveStagesAsync();

    /// <summary>
    /// 检查工序代码是否已存在
    /// </summary>
    /// <param name="code">工序代码</param>
    /// <param name="excludeId">排除的工序ID（用于编辑时检查）</param>
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);

    /// <summary>
    /// 检查工序是否被使用（是否有关联的称重记录）
    /// </summary>
    Task<bool> IsStageInUseAsync(int stageId);

    /// <summary>
    /// 获取激活工序的数量
    /// </summary>
    Task<int> GetActiveStageCountAsync();
}
