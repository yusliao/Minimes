using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessStageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessStage",
                table: "WeighingRecords",
                newName: "ProcessStageId");

            migrationBuilder.RenameIndex(
                name: "IX_WeighingRecords_ProcessStage",
                table: "WeighingRecords",
                newName: "IX_WeighingRecords_ProcessStageId");

            migrationBuilder.RenameIndex(
                name: "IX_WeighingRecords_Barcode_ProcessStage",
                table: "WeighingRecords",
                newName: "IX_WeighingRecords_Barcode_ProcessStageId");

            migrationBuilder.CreateTable(
                name: "ProcessStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    StageType = table.Column<int>(type: "INTEGER", nullable: false),
                    IncludeInLossRate = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStages_Code",
                table: "ProcessStages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStages_DisplayOrder",
                table: "ProcessStages",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStages_IsActive",
                table: "ProcessStages",
                column: "IsActive");

            // 插入初始工序数据（必须在添加外键之前）
            migrationBuilder.InsertData(
                table: "ProcessStages",
                columns: new[] { "Id", "Code", "Name", "DisplayOrder", "IsActive", "StageType", "IncludeInLossRate", "Description", "CreatedAt" },
                values: new object[,]
                {
                    { 1, "RECEIVING", "原料入库", 1, true, 1, true, "供应商送货，原料入库称重", DateTime.UtcNow },
                    { 2, "PROCESSING", "加工过程", 2, true, 2, true, "分割、去骨、腌制等加工环节称重", DateTime.UtcNow },
                    { 3, "SHIPPING", "成品出库", 3, true, 3, true, "最终成品包装后出库称重", DateTime.UtcNow }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_WeighingRecords_ProcessStages_ProcessStageId",
                table: "WeighingRecords",
                column: "ProcessStageId",
                principalTable: "ProcessStages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeighingRecords_ProcessStages_ProcessStageId",
                table: "WeighingRecords");

            migrationBuilder.DropTable(
                name: "ProcessStages");

            migrationBuilder.RenameColumn(
                name: "ProcessStageId",
                table: "WeighingRecords",
                newName: "ProcessStage");

            migrationBuilder.RenameIndex(
                name: "IX_WeighingRecords_ProcessStageId",
                table: "WeighingRecords",
                newName: "IX_WeighingRecords_ProcessStage");

            migrationBuilder.RenameIndex(
                name: "IX_WeighingRecords_Barcode_ProcessStageId",
                table: "WeighingRecords",
                newName: "IX_WeighingRecords_Barcode_ProcessStage");
        }
    }
}
