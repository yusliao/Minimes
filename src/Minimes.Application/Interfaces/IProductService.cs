using Minimes.Application.DTOs.Product;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 商品服务接口
/// </summary>
public interface IProductService
{
    /// <summary>
    /// 创建商品
    /// </summary>
    Task<ProductResponse> CreateAsync(CreateProductRequest request);

    /// <summary>
    /// 根据ID获取商品
    /// </summary>
    Task<ProductResponse?> GetByIdAsync(int id);

    /// <summary>
    /// 根据条形码获取商品
    /// </summary>
    Task<ProductResponse?> GetByBarcodeAsync(string barcode);

    /// <summary>
    /// 根据条形码获取或自动创建商品（生产管理场景）
    /// 如果条码不存在，自动创建一个新批次商品
    /// </summary>
    Task<ProductResponse> GetOrCreateByBarcodeAsync(string barcode, string createdBy);

    /// <summary>
    /// 获取所有活跃商品
    /// </summary>
    Task<IEnumerable<ProductResponse>> GetActiveProductsAsync();

    /// <summary>
    /// 按名称搜索商品
    /// </summary>
    Task<IEnumerable<ProductResponse>> SearchByNameAsync(string name);

    /// <summary>
    /// 更新商品
    /// </summary>
    Task<ProductResponse?> UpdateAsync(UpdateProductRequest request);

    /// <summary>
    /// 删除商品
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 检查条形码是否已存在（用于验证唯一性）
    /// </summary>
    Task<bool> BarcodeExistsAsync(string barcode, int? excludeId = null);

    /// <summary>
    /// 获取所有商品
    /// </summary>
    Task<IEnumerable<ProductResponse>> GetAllAsync();
}
