using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApproverSetupDeliveryTower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryTower",
                table: "ApproverSetup",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTower",
                table: "ApproverSetup");
        }
    }
}
