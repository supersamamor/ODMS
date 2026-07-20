using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDataSourceTypeOnDataSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetEntityId",
                table: "UploadProcessor",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataSourceType",
                table: "DataSource",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GeneratedTableName",
                table: "DataSource",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportErrorMessage",
                table: "DataSource",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportStatus",
                table: "DataSource",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastImportedDate",
                table: "DataSource",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedFilePath",
                table: "DataSource",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetEntityId",
                table: "UploadProcessor");

            migrationBuilder.DropColumn(
                name: "DataSourceType",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "GeneratedTableName",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "ImportErrorMessage",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "ImportStatus",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "LastImportedDate",
                table: "DataSource");

            migrationBuilder.DropColumn(
                name: "UploadedFilePath",
                table: "DataSource");
        }
    }
}
