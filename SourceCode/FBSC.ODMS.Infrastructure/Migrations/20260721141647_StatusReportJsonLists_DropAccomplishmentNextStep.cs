using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StatusReportJsonLists_DropAccomplishmentNextStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accomplishment");

            migrationBuilder.DropTable(
                name: "NextStep");

            migrationBuilder.AddColumn<string>(
                name: "Accomplishments",
                table: "StatusReport",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NextSteps",
                table: "StatusReport",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accomplishments",
                table: "StatusReport");

            migrationBuilder.DropColumn(
                name: "NextSteps",
                table: "StatusReport");

            migrationBuilder.CreateTable(
                name: "Accomplishment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    StatusReportId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accomplishment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accomplishment_StatusReport_StatusReportId",
                        column: x => x.StatusReportId,
                        principalTable: "StatusReport",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NextStep",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    StatusReportId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NextStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NextStep_StatusReport_StatusReportId",
                        column: x => x.StatusReportId,
                        principalTable: "StatusReport",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accomplishment_CreatedBy",
                table: "Accomplishment",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Accomplishment_Entity",
                table: "Accomplishment",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_Accomplishment_Id",
                table: "Accomplishment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Accomplishment_LastModifiedBy",
                table: "Accomplishment",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Accomplishment_LastModifiedDate",
                table: "Accomplishment",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Accomplishment_StatusReportId",
                table: "Accomplishment",
                column: "StatusReportId");

            migrationBuilder.CreateIndex(
                name: "IX_NextStep_CreatedBy",
                table: "NextStep",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NextStep_Entity",
                table: "NextStep",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_NextStep_Id",
                table: "NextStep",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NextStep_LastModifiedBy",
                table: "NextStep",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NextStep_LastModifiedDate",
                table: "NextStep",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_NextStep_StatusReportId",
                table: "NextStep",
                column: "StatusReportId");
        }
    }
}
