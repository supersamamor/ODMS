using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSequenceCounter_BUCode_RiskIssueCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "BusinessUnit",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            // Backfill placeholder unique codes (BU000001, BU000002, ...) for any
            // pre-existing rows so the unique index below can be created. Admins
            // should review/replace these; the field is required going forward.
            migrationBuilder.Sql(@"
;WITH numbered AS (
    SELECT [Code], ROW_NUMBER() OVER (ORDER BY [Id]) AS rn
    FROM [BusinessUnit]
    WHERE [Code] = ''
)
UPDATE numbered
   SET [Code] = 'BU' + RIGHT('000000' + CAST(rn AS varchar(6)), 6);");

            migrationBuilder.CreateTable(
                name: "SequenceCounter",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenceCounter", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_Code",
                table: "RiskIssue",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_Code",
                table: "BusinessUnit",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SequenceCounter");

            migrationBuilder.DropIndex(
                name: "IX_RiskIssue_Code",
                table: "RiskIssue");

            migrationBuilder.DropIndex(
                name: "IX_BusinessUnit_Code",
                table: "BusinessUnit");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "BusinessUnit");
        }
    }
}
