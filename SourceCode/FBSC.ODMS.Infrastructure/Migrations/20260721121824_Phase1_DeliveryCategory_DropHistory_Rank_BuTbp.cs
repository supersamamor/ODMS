using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase1_DeliveryCategory_DropHistory_Rank_BuTbp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamMembersHistory");

            migrationBuilder.DropTable(
                name: "ProjectHistory");

            migrationBuilder.DropIndex(
                name: "IX_ApproverSetup_WorkflowName_ApprovalSetupType_TableName_Entity",
                table: "ApproverSetup");

            migrationBuilder.RenameColumn(
                name: "DeliveryTower",
                table: "Project",
                newName: "DeliveryCategory");

            migrationBuilder.RenameColumn(
                name: "DeliveryTower",
                table: "ApproverSetup",
                newName: "DeliveryCategory");

            migrationBuilder.AddColumn<string>(
                name: "Rank",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApprovalSetupType",
                table: "ApproverSetup",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "BusinessUnitTechnologyBusinessPartner",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    BusinessUnitId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnitTechnologyBusinessPartner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessUnitTechnologyBusinessPartner_BusinessUnit_BusinessUnitId",
                        column: x => x.BusinessUnitId,
                        principalTable: "BusinessUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessUnitTechnologyBusinessPartner_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApproverSetup_TableName_DeliveryCategory",
                table: "ApproverSetup",
                columns: new[] { "TableName", "DeliveryCategory" },
                unique: true,
                filter: "[TableName] IS NOT NULL AND [DeliveryCategory] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnitTechnologyBusinessPartner_BusinessUnitId_EmployeeId",
                table: "BusinessUnitTechnologyBusinessPartner",
                columns: new[] { "BusinessUnitId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnitTechnologyBusinessPartner_CreatedBy",
                table: "BusinessUnitTechnologyBusinessPartner",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnitTechnologyBusinessPartner_EmployeeId",
                table: "BusinessUnitTechnologyBusinessPartner",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnitTechnologyBusinessPartner_Entity",
                table: "BusinessUnitTechnologyBusinessPartner",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnitTechnologyBusinessPartner_Id",
                table: "BusinessUnitTechnologyBusinessPartner",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnitTechnologyBusinessPartner_LastModifiedBy",
                table: "BusinessUnitTechnologyBusinessPartner",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnitTechnologyBusinessPartner_LastModifiedDate",
                table: "BusinessUnitTechnologyBusinessPartner",
                column: "LastModifiedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessUnitTechnologyBusinessPartner");

            migrationBuilder.DropIndex(
                name: "IX_ApproverSetup_TableName_DeliveryCategory",
                table: "ApproverSetup");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Employee");

            migrationBuilder.RenameColumn(
                name: "DeliveryCategory",
                table: "Project",
                newName: "DeliveryTower");

            migrationBuilder.RenameColumn(
                name: "DeliveryCategory",
                table: "ApproverSetup",
                newName: "DeliveryTower");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovalSetupType",
                table: "ApproverSetup",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ProjectHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    BusinessUnitId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    DeputyProjectManagerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ProjectManagerId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    TechnologyBusinessPartnerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    ActiveStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedBudget = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    BaselineEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaselineStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryTower = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DemandType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoSOW = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ProjectDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SOWFileName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
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
                        name: "FK_ProjectHistory_Employee_DeputyProjectManagerId",
                        column: x => x.DeputyProjectManagerId,
                        principalTable: "Employee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectHistory_Employee_ProjectManagerId",
                        column: x => x.ProjectManagerId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectHistory_Employee_TechnologyBusinessPartnerId",
                        column: x => x.TechnologyBusinessPartnerId,
                        principalTable: "Employee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectHistory_Project_ProjectId",
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
                    EmployeeId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ProjectHistoryId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MemberLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembersHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembersHistory_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamMembersHistory_ProjectHistory_ProjectHistoryId",
                        column: x => x.ProjectHistoryId,
                        principalTable: "ProjectHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApproverSetup_WorkflowName_ApprovalSetupType_TableName_Entity",
                table: "ApproverSetup",
                columns: new[] { "WorkflowName", "ApprovalSetupType", "TableName", "Entity" },
                unique: true,
                filter: "[WorkflowName] IS NOT NULL AND [TableName] IS NOT NULL AND [Entity] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_BusinessUnitId",
                table: "ProjectHistory",
                column: "BusinessUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_CreatedBy",
                table: "ProjectHistory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistory_DeputyProjectManagerId",
                table: "ProjectHistory",
                column: "DeputyProjectManagerId");

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
                name: "IX_ProjectHistory_TechnologyBusinessPartnerId",
                table: "ProjectHistory",
                column: "TechnologyBusinessPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembersHistory_CreatedBy",
                table: "TeamMembersHistory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembersHistory_EmployeeId",
                table: "TeamMembersHistory",
                column: "EmployeeId");

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
    }
}
