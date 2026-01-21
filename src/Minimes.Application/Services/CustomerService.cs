using FluentValidation;
using Minimes.Application.DTOs.Customer;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 客户服务实现
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IValidator<CreateCustomerRequest> _createValidator;
    private readonly IValidator<UpdateCustomerRequest> _updateValidator;

    public CustomerService(
        ICustomerRepository repository,
        IValidator<CreateCustomerRequest> createValidator,
        IValidator<UpdateCustomerRequest> updateValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
    {
        // 验证请求
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"客户创建验证失败: {errors}");
        }

        // 检查代码是否已存在
        if (await CodeExistsAsync(request.Code))
        {
            throw new InvalidOperationException($"客户代码 '{request.Code}' 已存在");
        }

        var customer = new Customer
        {
            Code = request.Code,
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Address = request.Address,
            IsActive = true
        };

        await _repository.AddAsync(customer);
        await _repository.SaveChangesAsync();

        return MapToResponse(customer);
    }

    public async Task<CustomerResponse?> GetByIdAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        return customer == null ? null : MapToResponse(customer);
    }

    public async Task<CustomerResponse?> GetByCodeAsync(string code)
    {
        var customer = await _repository.GetByCodeAsync(code);
        return customer == null ? null : MapToResponse(customer);
    }

    public async Task<IEnumerable<CustomerResponse>> GetActiveCustomersAsync()
    {
        var customers = await _repository.GetActiveCustomersAsync();
        return customers.Select(MapToResponse);
    }

    public async Task<IEnumerable<CustomerResponse>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await GetAllAsync();
        }

        var customers = await _repository.SearchByNameAsync(name);
        return customers.Select(MapToResponse);
    }

    public async Task<CustomerResponse?> UpdateAsync(UpdateCustomerRequest request)
    {
        // 验证请求
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"客户更新验证失败: {errors}");
        }

        var customer = await _repository.GetByIdAsync(request.Id);
        if (customer == null)
        {
            return null;
        }

        // 检查新代码是否已被其他客户使用
        if (customer.Code != request.Code && await CodeExistsAsync(request.Code))
        {
            throw new InvalidOperationException($"客户代码 '{request.Code}' 已存在");
        }

        customer.Code = request.Code;
        customer.Name = request.Name;
        customer.ContactPerson = request.ContactPerson;
        customer.Phone = request.Phone;
        customer.Address = request.Address;
        customer.IsActive = request.IsActive;

        await _repository.UpdateAsync(customer);
        await _repository.SaveChangesAsync();

        return MapToResponse(customer);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null)
        {
            return false;
        }

        await _repository.DeleteAsync(customer);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var customer = await _repository.GetByCodeAsync(code);
        if (customer == null)
        {
            return false;
        }

        return excludeId == null || customer.Id != excludeId;
    }

    public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
    {
        var customers = await _repository.GetAllAsync();
        return customers.Select(MapToResponse);
    }

    /// <summary>
    /// 映射实体到响应DTO
    /// </summary>
    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            Code = customer.Code,
            Name = customer.Name,
            ContactPerson = customer.ContactPerson,
            Phone = customer.Phone,
            Address = customer.Address,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
