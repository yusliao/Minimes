using Minimes.Application.DTOs.MeatType;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 肉类类型服务实现 - 肉类类型管理的业务逻辑
/// </summary>
public class MeatTypeService : IMeatTypeService
{
    private readonly IMeatTypeRepository _repository;

    public MeatTypeService(IMeatTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<MeatTypeResponse> CreateAsync(CreateMeatTypeRequest request)
    {
        // 检查类型代码是否已存在
        if (await _repository.CodeExistsAsync(request.Code))
        {
            throw new InvalidOperationException($"类型代码 '{request.Code}' 已存在！");
        }

        // 创建肉类类型实体
        var meatType = new MeatType
        {
            Code = request.Code.ToUpper(), // 统一转大写
            Name = request.Name,
            DisplayOrder = request.DisplayOrder,
            IsActive = true, // 新建类型默认激活
            Description = request.Description,
            CreatedAt = DateTime.Now
        };

        var created = await _repository.AddAsync(meatType);
        await _repository.SaveChangesAsync();

        return ToResponse(created);
    }

    public async Task<MeatTypeResponse?> GetByIdAsync(int id)
    {
        var meatType = await _repository.GetByIdAsync(id);
        return meatType == null ? null : ToResponse(meatType);
    }

    public async Task<MeatTypeResponse?> GetByCodeAsync(string code)
    {
        var meatType = await _repository.GetByCodeAsync(code);
        return meatType == null ? null : ToResponse(meatType);
    }

    public async Task<List<MeatTypeResponse>> GetAllAsync()
    {
        var meatTypes = await _repository.GetAllAsync();
        var orderedTypes = meatTypes.OrderBy(t => t.DisplayOrder).ToList();

        return orderedTypes.Select(ToResponse).ToList();
    }

    public async Task<List<MeatTypeResponse>> GetActiveTypesAsync()
    {
        var meatTypes = await _repository.GetActiveTypesAsync();
        return meatTypes.Select(ToResponse).ToList();
    }

    public async Task<MeatTypeResponse> UpdateAsync(int id, UpdateMeatTypeRequest request)
    {
        var meatType = await _repository.GetByIdAsync(id);
        if (meatType == null)
        {
            throw new InvalidOperationException($"肉类类型ID {id} 不存在！");
        }

        // 如果要停用类型，检查是否是最后一个激活类型
        if (meatType.IsActive && !request.IsActive)
        {
            var activeCount = await _repository.GetActiveTypeCountAsync();
            if (activeCount <= 1)
            {
                throw new InvalidOperationException("至少保留一个激活类型！");
            }
        }

        // 更新肉类类型信息
        meatType.Name = request.Name;
        meatType.DisplayOrder = request.DisplayOrder;
        meatType.IsActive = request.IsActive;
        meatType.Description = request.Description;
        meatType.UpdatedAt = DateTime.Now;

        await _repository.UpdateAsync(meatType);
        await _repository.SaveChangesAsync();

        return ToResponse(meatType);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var meatType = await _repository.GetByIdAsync(id);
        if (meatType == null)
        {
            return false;
        }

        // 检查肉类类型是否被使用
        if (await _repository.IsTypeInUseAsync(id))
        {
            throw new InvalidOperationException("该肉类类型已被使用，不能删除！请先停用该类型。");
        }

        await _repository.DeleteAsync(meatType);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(int id)
    {
        var meatType = await _repository.GetByIdAsync(id);
        if (meatType == null)
        {
            return false;
        }

        // 如果要停用类型，检查是否是最后一个激活类型
        if (meatType.IsActive)
        {
            var activeCount = await _repository.GetActiveTypeCountAsync();
            if (activeCount <= 1)
            {
                throw new InvalidOperationException("至少保留一个激活类型！");
            }
        }

        meatType.IsActive = !meatType.IsActive;
        meatType.UpdatedAt = DateTime.Now;

        await _repository.UpdateAsync(meatType);
        await _repository.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 转换为响应DTO
    /// </summary>
    private MeatTypeResponse ToResponse(MeatType meatType)
    {
        return new MeatTypeResponse
        {
            Id = meatType.Id,
            Code = meatType.Code,
            Name = meatType.Name,
            DisplayOrder = meatType.DisplayOrder,
            IsActive = meatType.IsActive,
            Description = meatType.Description,
            CreatedAt = meatType.CreatedAt,
            RecordCount = 0 // 预留字段，未来关联Product/WeighingRecord时启用
        };
    }
}
