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

        // 初始化工序数据（必须在WeighingRecord之前）
        if (!context.ProcessStages.Any())
        {
            var stages = new[]
            {
                new Domain.Entities.ProcessStage
                {
                    Id = 1,
                    Code = "RECEIVING",
                    Name = "原料入库",
                    DisplayOrder = 1,
                    IsActive = true,
                    StageType = StageType.Start,
                    IncludeInLossRate = true,
                    Description = "供应商送货，原料入库称重",
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.ProcessStage
                {
                    Id = 2,
                    Code = "PROCESSING",
                    Name = "加工过程",
                    DisplayOrder = 2,
                    IsActive = true,
                    StageType = StageType.Middle,
                    IncludeInLossRate = true,
                    Description = "分割、去骨、腌制等加工环节称重",
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.ProcessStage
                {
                    Id = 3,
                    Code = "SHIPPING",
                    Name = "成品出库",
                    DisplayOrder = 3,
                    IsActive = true,
                    StageType = StageType.End,
                    IncludeInLossRate = true,
                    Description = "最终成品包装后出库称重",
                    CreatedAt = DateTime.UtcNow
                }
            };
            context.ProcessStages.AddRange(stages);
            context.SaveChanges();
        }

        // 初始化肉类类型数据
        if (!context.MeatTypes.Any())
        {
            var meatTypes = new[]
            {
                new MeatType
                {
                    Id = 1,
                    Code = "PORK",
                    Name = "猪肉",
                    DisplayOrder = 1,
                    IsActive = true,
                    Description = "猪肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 2,
                    Code = "BEEF",
                    Name = "牛肉",
                    DisplayOrder = 2,
                    IsActive = true,
                    Description = "牛肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 3,
                    Code = "MUTTON",
                    Name = "羊肉",
                    DisplayOrder = 3,
                    IsActive = true,
                    Description = "羊肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 4,
                    Code = "CHICKEN",
                    Name = "鸡肉",
                    DisplayOrder = 4,
                    IsActive = true,
                    Description = "鸡肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 5,
                    Code = "DUCK",
                    Name = "鸭肉",
                    DisplayOrder = 5,
                    IsActive = true,
                    Description = "鸭肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 6,
                    Code = "FISH",
                    Name = "鱼肉",
                    DisplayOrder = 6,
                    IsActive = true,
                    Description = "鱼类及其制品",
                    CreatedAt = DateTime.UtcNow
                }
            };
            context.MeatTypes.AddRange(meatTypes);
            context.SaveChanges();
        }

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

        // 操作员账号（如果用户数<=2才插入）
        if (context.Users.Count() <= 2)
        {
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
        }

        // 测试称重记录（简化版：直接用条码）
        var testRecords = new[]
        {
            // 猪肉批次完整流程
            new WeighingRecord
            {
                Barcode = "PORK2026010701",
                Code = "2026010701",
                MeatTypeId = 1, // 猪肉
                Weight = 120.5m,
                ProcessStageId = 1, // 原料入库
                Remarks = "早班入库",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            },
            new WeighingRecord
            {
                Barcode = "PORK2026010701",
                Code = "2026010701",
                MeatTypeId = 1, // 猪肉
                Weight = 115.2m,
                ProcessStageId = 2, // 加工过程
                Remarks = "去骨分割",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new WeighingRecord
            {
                Barcode = "PORK2026010701",
                Code = "2026010701",
                MeatTypeId = 1, // 猪肉
                Weight = 110.8m,
                ProcessStageId = 3, // 成品出库
                Remarks = "装箱出库",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            // 牛肉批次
            new WeighingRecord
            {
                Barcode = "BEEF2026010701",
                Code = "2026010701",
                MeatTypeId = 2, // 牛肉
                Weight = 85.3m,
                ProcessStageId = 1, // 原料入库
                Remarks = "早班入库",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new WeighingRecord
            {
                Barcode = "BEEF2026010701",
                Code = "2026010701",
                MeatTypeId = 2, // 牛肉
                Weight = 81.7m,
                ProcessStageId = 2, // 加工过程
                Remarks = "去筋膜处理",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            // 鸡肉批次
            new WeighingRecord
            {
                Barcode = "CHICKEN2026010701",
                Code = "2026010701",
                MeatTypeId = 4, // 鸡肉
                Weight = 95.6m,
                ProcessStageId = 1, // 原料入库
                Remarks = "晚班入库",
                CreatedBy = "operator",
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        context.WeighingRecords.AddRange(testRecords);
        context.SaveChanges();
    }
}
