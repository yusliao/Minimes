using FluentValidation;
using Minimes.Application.DTOs.Employee;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 员工服务实现
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IQRCodeRepository _qrCodeRepository;
    private readonly IValidator<CreateEmployeeRequest> _createValidator;
    private readonly IValidator<UpdateEmployeeRequest> _updateValidator;

    public EmployeeService(
        IEmployeeRepository repository,
        IQRCodeRepository qrCodeRepository,
        IValidator<CreateEmployeeRequest> createValidator,
        IValidator<UpdateEmployeeRequest> updateValidator)
    {
        _repository = repository;
        _qrCodeRepository = qrCodeRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
    {
        // 验证请求
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"员工创建验证失败: {errors}");
        }

        // 检查代码是否已存在
        if (await CodeExistsAsync(request.Code))
        {
            throw new InvalidOperationException($"员工代码 '{request.Code}' 已存在");
        }

        var employee = new Employee
        {
            Code = request.Code,
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Address = request.Address,
            IsActive = true
        };

        await _repository.AddAsync(employee);
        await _repository.SaveChangesAsync();

        return await MapToResponseAsync(employee);
    }

    public async Task<EmployeeResponse?> GetByIdAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id);
        return employee == null ? null : await MapToResponseAsync(employee);
    }

    public async Task<EmployeeResponse?> GetByCodeAsync(string code)
    {
        var employee = await _repository.GetByCodeAsync(code);
        return employee == null ? null : await MapToResponseAsync(employee);
    }

    public async Task<IEnumerable<EmployeeResponse>> GetActiveEmployeesAsync()
    {
        var employees = await _repository.GetActiveEmployeesAsync();
        var responses = new List<EmployeeResponse>();
        foreach (var employee in employees)
        {
            responses.Add(await MapToResponseAsync(employee));
        }
        return responses;
    }

    public async Task<IEnumerable<EmployeeResponse>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await GetAllAsync();
        }

        var employees = await _repository.SearchByNameAsync(name);
        var responses = new List<EmployeeResponse>();
        foreach (var employee in employees)
        {
            responses.Add(await MapToResponseAsync(employee));
        }
        return responses;
    }

    public async Task<EmployeeResponse?> UpdateAsync(UpdateEmployeeRequest request)
    {
        // 验证请求
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"员工更新验证失败: {errors}");
        }

        var employee = await _repository.GetByIdAsync(request.Id);
        if (employee == null)
        {
            return null;
        }

        // 检查新代码是否已被其他员工使用
        if (employee.Code != request.Code && await CodeExistsAsync(request.Code))
        {
            throw new InvalidOperationException($"员工代码 '{request.Code}' 已存在");
        }

        // 级联停用逻辑：如果员工被停用，停用相关的二维码
        if (employee.IsActive && !request.IsActive)
        {
            await _qrCodeRepository.DeactivateByEmployeeCodeAsync(employee.Code);
        }

        employee.Code = request.Code;
        employee.Name = request.Name;
        employee.ContactPerson = request.ContactPerson;
        employee.Phone = request.Phone;
        employee.Address = request.Address;
        employee.IsActive = request.IsActive;

        await _repository.UpdateAsync(employee);
        await _repository.SaveChangesAsync();

        return await MapToResponseAsync(employee);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee == null)
        {
            return false;
        }

        // 级联停用逻辑：删除前先停用相关的二维码
        await _qrCodeRepository.DeactivateByEmployeeCodeAsync(employee.Code);

        await _repository.DeleteAsync(employee);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var employee = await _repository.GetByCodeAsync(code);
        if (employee == null)
        {
            return false;
        }

        return excludeId == null || employee.Id != excludeId;
    }

    public async Task<IEnumerable<EmployeeResponse>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();
        var responses = new List<EmployeeResponse>();
        foreach (var employee in employees)
        {
            responses.Add(await MapToResponseAsync(employee));
        }
        return responses;
    }

    /// <summary>
    /// 映射实体到响应DTO（包含QRCodeCount计算）
    /// </summary>
    private async Task<EmployeeResponse> MapToResponseAsync(Employee employee)
    {
        var qrCodeCount = await _qrCodeRepository.GetCountByEmployeeCodeAsync(employee.Code);

        return new EmployeeResponse
        {
            Id = employee.Id,
            Code = employee.Code,
            Name = employee.Name,
            ContactPerson = employee.ContactPerson,
            Phone = employee.Phone,
            Address = employee.Address,
            IsActive = employee.IsActive,
            QRCodeCount = qrCodeCount,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }
}
