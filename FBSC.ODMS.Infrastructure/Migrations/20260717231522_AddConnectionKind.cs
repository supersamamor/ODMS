using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DataUploadBatch_DataSourceId",
                table: "DataUploadBatch");

            migrationBuilder.AddColumn<string>(
                name: "SheetName",
                table: "DataUploadBatch",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionKind",
                table: "DataSource",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadBatch_DataSourceId_SheetName",
                table: "DataUploadBatch",
                columns: new[] { "DataSourceId", "SheetName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DataUploadBatch_DataSourceId_SheetName",
                table: "DataUploadBatch");

            migrationBuilder.DropColumn(
                name: "SheetName",
                table: "DataUploadBatch");

            migrationBuilder.DropColumn(
                name: "ConnectionKind",
                table: "DataSource");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadBatch_DataSourceId",
                table: "DataUploadBatch",
                column: "DataSourceId");
        }
    }
}
