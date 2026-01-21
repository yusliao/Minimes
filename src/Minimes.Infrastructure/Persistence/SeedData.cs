using Minimes.Domain.Entities;
using Minimes.Domain.Enums;
using Minimes.Domain.Security;

namespace Minimes.Infrastructure.Persistence;

/// <summary>
/// 数据库种子数据 - 初始化基础数据
/// </summary>
public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        // 管理员账号
        if (!context.Users.Any(u => u.Username == "admin"))
        {
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = PasswordHashService.HashPassword("Admin123456"),
                FullName = "系统管理员",
                Role = UserRole.Administrator,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();
        }

        // 演示账号
        if (!context.Users.Any(u => u.Username == "demo"))
        {
            context.Users.Add(new User
            {
                Username = "demo",
                PasswordHash = PasswordHashService.HashPassword("demo123"),
                FullName = "演示账号",
                Role = UserRole.Administrator,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();
        }

        // 如果已有其他数据，跳过
        if (context.Users.Count() > 2)
        {
            return;
        }

        // 操作员账号
        context.Users.Add(new User
        {
            Username = "operator",
            PasswordHash = PasswordHashService.HashPassword("Operator123456"),
            FullName = "操作员",
            Role = UserRole.Operator,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        context.SaveChanges();

        // 测试称重记录（简化版：直接用条码）
        var testRecords = new[]
        {
            // 猪肉批次完整流程
            new WeighingRecord
            {
                Barcode = "PORK2026010701",
                Weight = 120.5m,
                ProcessStage = ProcessStage.Receiving,
                Remarks = "早班入库",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            },
            new WeighingRecord
            {
                Barcode = "PORK2026010701",
                Weight = 115.2m,
                ProcessStage = ProcessStage.Processing,
                Remarks = "去骨分割",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new WeighingRecord
            {
                Barcode = "PORK2026010701",
                Weight = 110.8m,
                ProcessStage = ProcessStage.Shipping,
                Remarks = "装箱出库",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            // 牛肉批次
            new WeighingRecord
            {
                Barcode = "BEEF2026010701",
                Weight = 85.3m,
                ProcessStage = ProcessStage.Receiving,
                Remarks = "早班入库",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new WeighingRecord
            {
                Barcode = "BEEF2026010701",
                Weight = 81.7m,
                ProcessStage = ProcessStage.Processing,
                Remarks = "去筋膜处理",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            // 鸡肉批次
            new WeighingRecord
            {
                Barcode = "CHICKEN2026010701",
                Weight = 95.6m,
                ProcessStage = ProcessStage.Receiving,
                Remarks = "晚班入库",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        context.WeighingRecords.AddRange(testRecords);
        context.SaveChanges();
    }
}
