using Minimes.Application.DTOs.ProcessStage;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 工序服务接口 - 工序管理的业务逻辑接口
/// </summary>
public interface IProcessStageService
{
    /// <summary>
    /// 创建工序
    /// </summary>
    Task<ProcessStageResponse> CreateAsync(CreateProcessStageRequest request);

    /// <summary>
    /// 根据ID获取工序
    /// </summary>
    Task<ProcessStageResponse?> GetByIdAsync(int id);

    /// <summary>
    /// 根据工序代码获取工序
    /// </summary>
    Task<ProcessStageResponse?> GetByCodeAsync(string code);

    /// <summary>
    /// 获取所有工序（按DisplayOrder排序）
    /// </summary>
    Task<List<ProcessStageResponse>> GetAllAsync();

    /// <summary>
    /// 获取所有激活的工序（按DisplayOrder排序）
    /// </summary>
    Task<List<ProcessStageResponse>> GetActiveStagesAsync();

    /// <summary>
    /// 更新工序
    /// </summary>
    Task<ProcessStageResponse> UpdateAsync(int id, UpdateProcessStageRequest request);

    /// <summary>
    /// 删除工序（如果被使用则不能删除）
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 切换工序激活状态（至少保留一个激活工序）
    /// </summary>
    Task<bool> ToggleActiveAsync(int id);
}
