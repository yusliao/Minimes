using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimes.Domain.Entities;

namespace Minimes.Infrastructure.Persistence.Configurations;

/// <summary>
/// 商品实体配置
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Barcode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Specification)
            .HasMaxLength(500);

        builder.Property(p => p.Unit)
            .HasMaxLength(50);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        // 唯一索引：条形码不能重复
        builder.HasIndex(p => p.Barcode)
            .IsUnique();

        // 索引：商品名称（常用于搜索）
        builder.HasIndex(p => p.Name);

        // 索引：激活状态
        builder.HasIndex(p => p.IsActive);

        // 注意：不配置与WeighingRecord的导航关系
        // 在服务层中通过外键手动查询，保持Clean Architecture分离
    }
}
