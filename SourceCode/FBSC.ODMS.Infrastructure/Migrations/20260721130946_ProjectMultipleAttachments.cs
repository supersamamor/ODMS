using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProjectMultipleAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SOWFileName",
                table: "Project");

            migrationBuilder.CreateTable(
                name: "ProjectAttachment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAttachment_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_CreatedBy",
                table: "ProjectAttachment",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_Entity",
                table: "ProjectAttachment",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_Id",
                table: "ProjectAttachment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_LastModifiedBy",
                table: "ProjectAttachment",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_LastModifiedDate",
                table: "ProjectAttachment",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_ProjectId",
                table: "ProjectAttachment",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectAttachment");

            migrationBuilder.AddColumn<string>(
                name: "SOWFileName",
                table: "Project",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }
    }
}
