using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMeatTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeatTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeatTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeatTypes_Code",
                table: "MeatTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeatTypes_DisplayOrder",
                table: "MeatTypes",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MeatTypes_IsActive",
                table: "MeatTypes",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeatTypes");
        }
    }
}
