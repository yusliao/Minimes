using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameCustomerToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 重命名表（保留数据）
            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Employees");

            // 重命名索引
            migrationBuilder.RenameIndex(
                name: "IX_Customers_Code",
                newName: "IX_Employees_Code",
                table: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_IsActive",
                newName: "IX_Employees_IsActive",
                table: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_Name",
                newName: "IX_Employees_Name",
                table: "Employees");

            // 添加EmployeeCode字段到QRCodes表
            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "QRCodes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 删除QRCodes表的EmployeeCode字段
            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "QRCodes");

            // 重命名索引（改回原名）
            migrationBuilder.RenameIndex(
                name: "IX_Employees_Code",
                newName: "IX_Customers_Code",
                table: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_IsActive",
                newName: "IX_Customers_IsActive",
                table: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_Name",
                newName: "IX_Customers_Name",
                table: "Employees");

            // 重命名表（改回原名）
            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Customers");
        }
    }
}
