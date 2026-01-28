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

        builder.Property(w => w.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.Weight)
            .IsRequired()
            .HasPrecision(18, 3);

        // 肉类类型外键关系
        builder.HasOne(w => w.MeatType)
            .WithMany()
            .HasForeignKey(w => w.MeatTypeId)
            .OnDelete(DeleteBehavior.Restrict);  // 防止级联删除

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
        builder.HasIndex(w => w.Code);
        builder.HasIndex(w => w.MeatTypeId);
    }
}
