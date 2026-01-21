using Minimes.Domain.Entities;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 商品仓储接口
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// 根据条形码获取商品
    /// </summary>
    Task<Product?> GetByBarcodeAsync(string barcode);

    /// <summary>
    /// 根据名称搜索商品
    /// </summary>
    Task<IEnumerable<Product>> SearchByNameAsync(string name);

    /// <summary>
    /// 获取所有激活的商品
    /// </summary>
    Task<IEnumerable<Product>> GetActiveProductsAsync();
}
