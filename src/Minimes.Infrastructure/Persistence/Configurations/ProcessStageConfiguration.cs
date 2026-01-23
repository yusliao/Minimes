using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimes.Domain.Entities;

namespace Minimes.Infrastructure.Persistence.Configurations;

/// <summary>
/// 工序实体配置
/// </summary>
public class ProcessStageConfiguration : IEntityTypeConfiguration<ProcessStage>
{
    public void Configure(EntityTypeBuilder<ProcessStage> builder)
    {
        builder.ToTable("ProcessStages");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.DisplayOrder)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.StageType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.IncludeInLossRate)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        // 索引
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.DisplayOrder);
        builder.HasIndex(p => p.IsActive);
    }
}
