using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBUAndProjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessUnit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BusinessUnitId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedBudget = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ProjectDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProjectManagerId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    HealthStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScheduleStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_BusinessUnit_BusinessUnitId",
                        column: x => x.BusinessUnitId,
                        principalTable: "BusinessUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Employee_ProjectManagerId",
                        column: x => x.ProjectManagerId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BusinessUnitId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedBudget = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ProjectDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProjectManagerId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    HealthStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScheduleStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectHistory_BusinessUnit_BusinessUnitId",
                        column: x => x.BusinessUnitId,
                        principalTable: "BusinessUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectHistory_Employee_ProjectManagerId",
                        column: x => x.ProjectManagerId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectHistory_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembersHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectHistoryId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembersHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembersHistory_ProjectHistory_ProjectHistoryId",
                        column: x => x.ProjectHistoryId,
                        principalTable: "ProjectHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_CreatedBy",
                table: "BusinessUnit",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_Entity",
                table: "BusinessUnit",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_Id",
                table: "BusinessUnit",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_LastModifiedBy",
                table: "BusinessUnit",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnit_LastModifiedDate",
                table: "BusinessUnit",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CreatedBy",
                table: "Employee",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Entity",
                table: "Employee",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Id",
                table: "Employee",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_LastModifiedBy",
                table: "Employee",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_LastModifiedDate",
                table: "Employee",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Project_BusinessUnitId",
                table: "Project",
                column: "BusinessUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CreatedBy",
                table: "Project",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Entity",
                table: "Project",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Id",
                table: "Project",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Project_LastModifiedBy",
                table: "Project",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Project_LastModifiedDate",
                table: "Project",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectManagerId",
                table: "Project",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_BusinessUnitId",
                table: "ProjectHistory",
                column: "BusinessUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_CreatedBy",
                table: "ProjectHistory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_Entity",
                table: "ProjectHistory",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_Id",
                table: "ProjectHistory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_LastModifiedBy",
                table: "ProjectHistory",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_LastModifiedDate",
                table: "ProjectHistory",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_ProjectId",
                table: "ProjectHistory",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_ProjectManagerId",
                table: "ProjectHistory",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_CreatedBy",
                table: "TeamMembers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_Entity",
                table: "TeamMembers",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_Id",
                table: "TeamMembers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_LastModifiedBy",
                table: "TeamMembers",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_LastModifiedDate",
                table: "TeamMembers",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ProjectId",
                table: "TeamMembers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembersHistory_CreatedBy",
                table: "TeamMembersHistory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembersHistory_Entity",
                table: "TeamMembersHistory",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembersHistory_Id",
                table: "TeamMembersHistory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembersHistory_LastModifiedBy",
                table: "TeamMembersHistory",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembersHistory_LastModifiedDate",
                table: "TeamMembersHistory",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembersHistory_ProjectHistoryId",
                table: "TeamMembersHistory",
                column: "ProjectHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "TeamMembersHistory");

            migrationBuilder.DropTable(
                name: "ProjectHistory");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "BusinessUnit");

            migrationBuilder.DropTable(
                name: "Employee");
        }
    }
}
