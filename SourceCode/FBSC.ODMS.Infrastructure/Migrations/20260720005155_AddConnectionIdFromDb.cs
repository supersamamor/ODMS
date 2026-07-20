using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionIdFromDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataSourceId",
                table: "Report",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Report_DataSourceId",
                table: "Report",
                column: "DataSourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Report_DataSourceId",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "DataSourceId",
                table: "Report");
        }
    }
}
