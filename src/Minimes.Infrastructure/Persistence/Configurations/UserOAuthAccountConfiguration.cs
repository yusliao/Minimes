using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimes.Domain.Entities;

namespace Minimes.Infrastructure.Persistence.Configurations;

/// <summary>
/// 用户OAuth账号实体配置
/// </summary>
public class UserOAuthAccountConfiguration : IEntityTypeConfiguration<UserOAuthAccount>
{
    public void Configure(EntityTypeBuilder<UserOAuthAccount> builder)
    {
        builder.ToTable("UserOAuthAccounts");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.ProviderType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.ProviderUserId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.ProviderName)
            .HasMaxLength(200);

        builder.Property(o => o.ProviderAvatar)
            .HasMaxLength(500);

        builder.Property(o => o.RefreshToken)
            .HasMaxLength(1000);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        // 唯一索引：同一提供商的账号只能绑定一次
        builder.HasIndex(o => new { o.ProviderType, o.ProviderUserId })
            .IsUnique();

        // 索引：用户ID（查询用户的所有OAuth账号）
        builder.HasIndex(o => o.UserId);

        // 关系配置：一个用户有多个OAuth账号
        builder.HasOne(o => o.User)
            .WithMany(u => u.OAuthAccounts)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade); // 删除用户时级联删除OAuth账号
    }
}
