using Minimes.Application.DTOs.ProcessStage;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 工序服务实现 - 工序管理的业务逻辑
/// </summary>
public class ProcessStageService : IProcessStageService
{
    private readonly IProcessStageRepository _repository;
    private readonly IWeighingRecordRepository _weighingRecordRepository;

    public ProcessStageService(
        IProcessStageRepository repository,
        IWeighingRecordRepository weighingRecordRepository)
    {
        _repository = repository;
        _weighingRecordRepository = weighingRecordRepository;
    }

    public async Task<ProcessStageResponse> CreateAsync(CreateProcessStageRequest request)
    {
        // 检查工序代码是否已存在
        if (await _repository.CodeExistsAsync(request.Code))
        {
            throw new InvalidOperationException($"工序代码 '{request.Code}' 已存在！");
        }

        // 解析工序类型
        if (!Enum.TryParse<StageType>(request.StageType, out var stageType))
        {
            throw new ArgumentException($"无效的工序类型：{request.StageType}");
        }

        // 创建工序实体
        var stage = new ProcessStage
        {
            Code = request.Code.ToUpper(), // 统一转大写
            Name = request.Name,
            DisplayOrder = request.DisplayOrder,
            IsActive = true, // 新建工序默认激活
            StageType = stageType,
            IncludeInLossRate = request.IncludeInLossRate,
            Description = request.Description,
            CreatedAt = DateTime.Now
        };

        var created = await _repository.AddAsync(stage);
        await _repository.SaveChangesAsync();

        return await ToResponseAsync(created);
    }

    public async Task<ProcessStageResponse?> GetByIdAsync(int id)
    {
        var stage = await _repository.GetByIdAsync(id);
        return stage == null ? null : await ToResponseAsync(stage);
    }

    public async Task<ProcessStageResponse?> GetByCodeAsync(string code)
    {
        var stage = await _repository.GetByCodeAsync(code);
        return stage == null ? null : await ToResponseAsync(stage);
    }

    public async Task<List<ProcessStageResponse>> GetAllAsync()
    {
        var stages = await _repository.GetAllAsync();
        var orderedStages = stages.OrderBy(s => s.DisplayOrder).ToList();

        var responses = new List<ProcessStageResponse>();
        foreach (var stage in orderedStages)
        {
            responses.Add(await ToResponseAsync(stage));
        }
        return responses;
    }

    public async Task<List<ProcessStageResponse>> GetActiveStagesAsync()
    {
        var stages = await _repository.GetActiveStagesAsync();

        var responses = new List<ProcessStageResponse>();
        foreach (var stage in stages)
        {
            responses.Add(await ToResponseAsync(stage));
        }
        return responses;
    }

    public async Task<ProcessStageResponse> UpdateAsync(int id, UpdateProcessStageRequest request)
    {
        var stage = await _repository.GetByIdAsync(id);
        if (stage == null)
        {
            throw new InvalidOperationException($"工序ID {id} 不存在！");
        }

        // 解析工序类型
        if (!Enum.TryParse<StageType>(request.StageType, out var stageType))
        {
            throw new ArgumentException($"无效的工序类型：{request.StageType}");
        }

        // 如果要停用工序，检查是否是最后一个激活工序
        if (stage.IsActive && !request.IsActive)
        {
            var activeCount = await _repository.GetActiveStageCountAsync();
            if (activeCount <= 1)
            {
                throw new InvalidOperationException("至少保留一个激活工序！");
            }
        }

        // 更新工序信息
        stage.Name = request.Name;
        stage.DisplayOrder = request.DisplayOrder;
        stage.IsActive = request.IsActive;
        stage.StageType = stageType;
        stage.IncludeInLossRate = request.IncludeInLossRate;
        stage.Description = request.Description;
        stage.UpdatedAt = DateTime.Now;

        await _repository.UpdateAsync(stage);
        await _repository.SaveChangesAsync();

        return await ToResponseAsync(stage);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var stage = await _repository.GetByIdAsync(id);
        if (stage == null)
        {
            return false;
        }

        // 检查工序是否被使用
        if (await _repository.IsStageInUseAsync(id))
        {
            throw new InvalidOperationException("该工序已被使用，不能删除！请先停用该工序。");
        }

        await _repository.DeleteAsync(stage);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(int id)
    {
        var stage = await _repository.GetByIdAsync(id);
        if (stage == null)
        {
            return false;
        }

        // 如果要停用工序，检查是否是最后一个激活工序
        if (stage.IsActive)
        {
            var activeCount = await _repository.GetActiveStageCountAsync();
            if (activeCount <= 1)
            {
                throw new InvalidOperationException("至少保留一个激活工序！");
            }
        }

        stage.IsActive = !stage.IsActive;
        stage.UpdatedAt = DateTime.Now;

        await _repository.UpdateAsync(stage);
        await _repository.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 转换为响应DTO
    /// </summary>
    private async Task<ProcessStageResponse> ToResponseAsync(ProcessStage stage)
    {
        // 获取该工序下的记录数量
        var records = await _weighingRecordRepository.GetAllAsync();
        var recordCount = records.Count(r => r.ProcessStageId == stage.Id);

        return new ProcessStageResponse
        {
            Id = stage.Id,
            Code = stage.Code,
            Name = stage.Name,
            DisplayOrder = stage.DisplayOrder,
            IsActive = stage.IsActive,
            StageType = stage.StageType.ToString(),
            IncludeInLossRate = stage.IncludeInLossRate,
            Description = stage.Description,
            CreatedAt = stage.CreatedAt,
            RecordCount = recordCount
        };
    }
}
