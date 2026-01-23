using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimes.Domain.Entities;

namespace Minimes.Infrastructure.Persistence.Configurations;

/// <summary>
/// 肉类类型实体配置
/// </summary>
public class MeatTypeConfiguration : IEntityTypeConfiguration<MeatType>
{
    public void Configure(EntityTypeBuilder<MeatType> builder)
    {
        builder.ToTable("MeatTypes");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.DisplayOrder)
            .IsRequired();

        builder.Property(m => m.IsActive)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(500);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        // 索引
        builder.HasIndex(m => m.Code).IsUnique();
        builder.HasIndex(m => m.DisplayOrder);
        builder.HasIndex(m => m.IsActive);
    }
}
