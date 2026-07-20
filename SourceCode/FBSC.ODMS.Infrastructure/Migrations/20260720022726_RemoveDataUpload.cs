using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDataUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataUpload");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataUpload",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataUpload", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataUpload_CreatedBy",
                table: "DataUpload",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataUpload_Description",
                table: "DataUpload",
                column: "Description",
                unique: true,
                filter: "[Description] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DataUpload_Entity",
                table: "DataUpload",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DataUpload_Id",
                table: "DataUpload",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DataUpload_LastModifiedBy",
                table: "DataUpload",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataUpload_LastModifiedDate",
                table: "DataUpload",
                column: "LastModifiedDate");
        }
    }
}
