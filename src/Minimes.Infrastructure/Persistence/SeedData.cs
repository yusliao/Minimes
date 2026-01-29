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

        // 初始化肉类类型数据
        if (!context.MeatTypes.Any())
        {
            var meatTypes = new[]
            {
                new MeatType
                {
                    Id = 1,
                    Code = "PORK",
                    Name = "PORK",
                    DisplayOrder = 1,
                    IsActive = true,
                    Description = "猪肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 2,
                    Code = "BEEF",
                    Name = "BEEF",
                    DisplayOrder = 2,
                    IsActive = true,
                    Description = "牛肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 3,
                    Code = "MUTTON",
                    Name = "MUTTON",
                    DisplayOrder = 3,
                    IsActive = true,
                    Description = "羊肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 4,
                    Code = "CHICKEN",
                    Name = "CHICKEN",
                    DisplayOrder = 4,
                    IsActive = true,
                    Description = "鸡肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 5,
                    Code = "DUCK",
                    Name = "DUCK",
                    DisplayOrder = 5,
                    IsActive = true,
                    Description = "鸭肉及其制品",
                    CreatedAt = DateTime.UtcNow
                },
                new MeatType
                {
                    Id = 6,
                    Code = "FISH",
                    Name = "FISH",
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

        //// 演示账号
        //if (!context.Users.Any(u => u.Username == "demo"))
        //{
        //    context.Users.Add(new User
        //    {
        //        Username = "demo",
        //        PasswordHash = PasswordHashService.HashPassword("demo123"),
        //        FullName = "演示账号",
        //        Role = UserRole.Administrator,
        //        IsActive = true,
        //        CreatedAt = DateTime.UtcNow
        //    });
        //    context.SaveChanges();
        //}

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

        
    }
}
