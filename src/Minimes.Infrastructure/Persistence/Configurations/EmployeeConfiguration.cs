using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimes.Domain.Entities;

namespace Minimes.Infrastructure.Persistence.Configurations;

/// <summary>
/// 员工实体配置
/// </summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ContactPerson)
            .HasMaxLength(100);

        builder.Property(e => e.Phone)
            .HasMaxLength(50);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // 唯一索引：员工编码不能重复
        builder.HasIndex(e => e.Code)
            .IsUnique();

        // 索引：员工名称（常用于搜索）
        builder.HasIndex(e => e.Name);

        // 索引：激活状态
        builder.HasIndex(e => e.IsActive);

        // 注意：不配置与WeighingRecord的导航关系
        // 在服务层中通过外键手动查询，保持Clean Architecture分离
    }
}
