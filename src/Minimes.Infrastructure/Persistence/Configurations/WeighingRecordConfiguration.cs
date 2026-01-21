using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimes.Domain.Entities;

namespace Minimes.Infrastructure.Persistence.Configurations;

/// <summary>
/// 称重记录实体配置 - 简化版
/// </summary>
public class WeighingRecordConfiguration : IEntityTypeConfiguration<WeighingRecord>
{
    public void Configure(EntityTypeBuilder<WeighingRecord> builder)
    {
        builder.ToTable("WeighingRecords");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Barcode)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Weight)
            .IsRequired()
            .HasPrecision(18, 3);

        builder.Property(w => w.ProcessStage)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(w => w.Remarks)
            .HasMaxLength(1000);

        builder.Property(w => w.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        // 索引
        builder.HasIndex(w => w.CreatedAt).IsDescending();
        builder.HasIndex(w => w.Barcode);
        builder.HasIndex(w => w.ProcessStage);

        // 唯一索引：同一条码+同一加工环节不能重复
        builder.HasIndex(w => new { w.Barcode, w.ProcessStage }).IsUnique();
    }
}
