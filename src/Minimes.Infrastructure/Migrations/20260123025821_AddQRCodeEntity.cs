using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQRCodeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QRCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MeatTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ImageBase64 = table.Column<string>(type: "text", nullable: true),
                    BatchNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PrintCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPrintedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QRCodes_MeatTypes_MeatTypeId",
                        column: x => x.MeatTypeId,
                        principalTable: "MeatTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_BatchNumber",
                table: "QRCodes",
                column: "BatchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_Content",
                table: "QRCodes",
                column: "Content",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_IsActive",
                table: "QRCodes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_MeatTypeId",
                table: "QRCodes",
                column: "MeatTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QRCodes");
        }
    }
}
