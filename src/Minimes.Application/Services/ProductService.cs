using FluentValidation;
using Minimes.Application.DTOs.Product;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 商品服务实现
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;

    public ProductService(
        IProductRepository repository,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        // 验证请求
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"商品创建验证失败: {errors}");
        }

        // 检查条形码是否已存在
        if (await BarcodeExistsAsync(request.Barcode))
        {
            throw new InvalidOperationException($"条形码 '{request.Barcode}' 已存在");
        }

        var product = new Product
        {
            Barcode = request.Barcode,
            Name = request.Name,
            Specification = request.Specification,
            Unit = request.Unit,
            ReferencePrice = request.ReferencePrice,
            IsActive = true
        };

        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();

        return MapToResponse(product);
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : MapToResponse(product);
    }

    public async Task<ProductResponse?> GetByBarcodeAsync(string barcode)
    {
        var product = await _repository.GetByBarcodeAsync(barcode);
        return product == null ? null : MapToResponse(product);
    }

    public async Task<ProductResponse> GetOrCreateByBarcodeAsync(string barcode, string createdBy)
    {
        // 先尝试查询已有商品
        var existing = await _repository.GetByBarcodeAsync(barcode);
        if (existing != null)
        {
            return MapToResponse(existing);
        }

        // 如果不存在，自动创建新批次商品
        var newProduct = new Product
        {
            Barcode = barcode,
            Name = $"批次-{barcode}", // 自动生成名称
            Specification = "供应商提供",
            Unit = "克",
            ReferencePrice = 0, // 生产管理场景不需要价格
            IsActive = true
        };

        await _repository.AddAsync(newProduct);
        await _repository.SaveChangesAsync();

        return MapToResponse(newProduct);
    }

    public async Task<IEnumerable<ProductResponse>> GetActiveProductsAsync()
    {
        var products = await _repository.GetActiveProductsAsync();
        return products.Select(MapToResponse);
    }

    public async Task<IEnumerable<ProductResponse>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await GetAllAsync();
        }

        var products = await _repository.SearchByNameAsync(name);
        return products.Select(MapToResponse);
    }

    public async Task<ProductResponse?> UpdateAsync(UpdateProductRequest request)
    {
        // 验证请求
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"商品更新验证失败: {errors}");
        }

        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null)
        {
            return null;
        }

        // 检查新条形码是否已被其他商品使用
        if (product.Barcode != request.Barcode && await BarcodeExistsAsync(request.Barcode))
        {
            throw new InvalidOperationException($"条形码 '{request.Barcode}' 已存在");
        }

        product.Barcode = request.Barcode;
        product.Name = request.Name;
        product.Specification = request.Specification;
        product.Unit = request.Unit;
        product.ReferencePrice = request.ReferencePrice;
        product.IsActive = request.IsActive;

        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();

        return MapToResponse(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }

        await _repository.DeleteAsync(product);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BarcodeExistsAsync(string barcode, int? excludeId = null)
    {
        var product = await _repository.GetByBarcodeAsync(barcode);
        if (product == null)
        {
            return false;
        }

        return excludeId == null || product.Id != excludeId;
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(MapToResponse);
    }

    /// <summary>
    /// 映射实体到响应DTO
    /// </summary>
    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Barcode = product.Barcode,
            Name = product.Name,
            Specification = product.Specification,
            Unit = product.Unit ?? "kg",
            ReferencePrice = product.ReferencePrice,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
