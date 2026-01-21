using Microsoft.EntityFrameworkCore;
using Minimes.Domain.Entities;

namespace Minimes.Infrastructure.Persistence;

/// <summary>
/// EF Core数据库上下文 - 数据访问层核心
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 用户表
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// 客户表
    /// </summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>
    /// 商品表
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// 称重记录表
    /// </summary>
    public DbSet<WeighingRecord> WeighingRecords => Set<WeighingRecord>();

    /// <summary>
    /// 用户OAuth账号表
    /// </summary>
    public DbSet<UserOAuthAccount> UserOAuthAccounts => Set<UserOAuthAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 应用所有实体配置
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 自动设置时间戳
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
