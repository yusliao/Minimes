using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimes.Domain.Entities;

namespace Minimes.Infrastructure.Persistence.Configurations;

/// <summary>
/// 客户实体配置
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.ContactPerson)
            .HasMaxLength(100);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // 唯一索引：客户编码不能重复
        builder.HasIndex(c => c.Code)
            .IsUnique();

        // 索引：客户名称（常用于搜索）
        builder.HasIndex(c => c.Name);

        // 索引：激活状态
        builder.HasIndex(c => c.IsActive);

        // 注意：不配置与WeighingRecord的导航关系
        // 在服务层中通过外键手动查询，保持Clean Architecture分离
    }
}
