using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimes.Domain.Entities;

namespace Minimes.Infrastructure.Persistence.Configurations;

/// <summary>
/// 二维码实体配置
/// </summary>
public class QRCodeConfiguration : IEntityTypeConfiguration<QRCode>
{
    public void Configure(EntityTypeBuilder<QRCode> builder)
    {
        builder.ToTable("QRCodes");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(q => q.MeatTypeId)
            .IsRequired();

        builder.Property(q => q.Content)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(q => q.ImageBase64)
            .HasColumnType("text");

        builder.Property(q => q.BatchNumber)
            .HasMaxLength(50);

        builder.Property(q => q.PrintCount)
            .IsRequired();

        builder.Property(q => q.LastPrintedAt);

        builder.Property(q => q.IsActive)
            .IsRequired();

        builder.Property(q => q.Remarks)
            .HasMaxLength(500);

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        // 索引
        builder.HasIndex(q => q.Content).IsUnique();
        builder.HasIndex(q => q.MeatTypeId);
        builder.HasIndex(q => q.BatchNumber);
        builder.HasIndex(q => q.IsActive);

        // 外键关系
        builder.HasOne(q => q.MeatType)
            .WithMany()
            .HasForeignKey(q => q.MeatTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
