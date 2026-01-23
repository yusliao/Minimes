using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMeatTypeAndCodeToWeighingRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WeighingRecords_Barcode_ProcessStageId",
                table: "WeighingRecords");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "WeighingRecords",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MeatTypeId",
                table: "WeighingRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WeighingRecords_Code",
                table: "WeighingRecords",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_WeighingRecords_MeatTypeId",
                table: "WeighingRecords",
                column: "MeatTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeighingRecords_MeatTypes_MeatTypeId",
                table: "WeighingRecords",
                column: "MeatTypeId",
                principalTable: "MeatTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeighingRecords_MeatTypes_MeatTypeId",
                table: "WeighingRecords");

            migrationBuilder.DropIndex(
                name: "IX_WeighingRecords_Code",
                table: "WeighingRecords");

            migrationBuilder.DropIndex(
                name: "IX_WeighingRecords_MeatTypeId",
                table: "WeighingRecords");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "WeighingRecords");

            migrationBuilder.DropColumn(
                name: "MeatTypeId",
                table: "WeighingRecords");

            migrationBuilder.CreateIndex(
                name: "IX_WeighingRecords_Barcode_ProcessStageId",
                table: "WeighingRecords",
                columns: new[] { "Barcode", "ProcessStageId" },
                unique: true);
        }
    }
}
