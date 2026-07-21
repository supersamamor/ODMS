using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApproverSetup",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ApprovalSetupType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkflowName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    WorkflowDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovalType = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EmailSubject = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EmailBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproverSetup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", maxLength: 36, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AffectedColumns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryKey = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

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
                name: "DataSource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ServerAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DatabaseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AuthenticationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordEncrypted = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ConnectionStringEncrypted = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DataSourceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UploadedFilePath = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    GeneratedTableName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ImportStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ImportErrorMessage = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LastImportedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSource", x => x.Id);
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
                name: "HTMLTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HTMLTemplateName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HTMLTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HTMLFooterTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomCss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orientation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PaperSize = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MarginTop = table.Column<int>(type: "int", nullable: false),
                    MarginBottom = table.Column<int>(type: "int", nullable: false),
                    MarginLeft = table.Column<int>(type: "int", nullable: false),
                    MarginRight = table.Column<int>(type: "int", nullable: false),
                    CustomSwitch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CssLibraries = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HTMLTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Milestone",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ReportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportOrChartType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDistinct = table.Column<bool>(type: "bit", nullable: false),
                    QueryString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOnDashboard = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOnReportModule = table.Column<bool>(type: "bit", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    HtmlTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HTMLFooterTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orientation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaperSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarginTop = table.Column<int>(type: "int", nullable: false),
                    MarginBottom = table.Column<int>(type: "int", nullable: false),
                    MarginLeft = table.Column<int>(type: "int", nullable: false),
                    MarginRight = table.Column<int>(type: "int", nullable: false),
                    SpanWidth = table.Column<int>(type: "int", nullable: false),
                    DrillDownReportId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataSourceId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportingWeek",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportingWeek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadProcessor",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UploadType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetEntityId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExceptionFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadProcessor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebhookApi",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GrantType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    WithinPrivateNetwork = table.Column<bool>(type: "bit", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AuthenticationUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HMAC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AdditionalConfigurationJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiresIn = table.Column<int>(type: "int", nullable: true),
                    RefreshExpiresIn = table.Column<int>(type: "int", nullable: true),
                    RequestDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookApi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ApproverSetupId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DataId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRecord_ApproverSetup_ApproverSetupId",
                        column: x => x.ApproverSetupId,
                        principalTable: "ApproverSetup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApproverAssignment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ApproverType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApproverSetupId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    ApproverUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    ApproverRoleId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproverAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApproverAssignment_ApproverSetup_ApproverSetupId",
                        column: x => x.ApproverSetupId,
                        principalTable: "ApproverSetup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeliveryTower = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DemandType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BusinessUnitId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    TechnologyBusinessPartnerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaselineStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaselineEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBudget = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ProjectDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProjectManagerId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    DeputyProjectManagerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    ActiveStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SOWFileName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    NoSOW = table.Column<bool>(type: "bit", nullable: false)
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
                        name: "FK_Project_Employee_DeputyProjectManagerId",
                        column: x => x.DeputyProjectManagerId,
                        principalTable: "Employee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Project_Employee_ProjectManagerId",
                        column: x => x.ProjectManagerId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_Employee_TechnologyBusinessPartnerId",
                        column: x => x.TechnologyBusinessPartnerId,
                        principalTable: "Employee",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportAIIntegration",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ReportId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Input = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportAIIntegration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportAIIntegration_Report_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Report",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportQueryFilter",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ReportId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomDropdownValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DropdownTableKeyAndValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DropdownFilter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportQueryFilter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportQueryFilter_Report_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Report",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportRoleAssignment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ReportId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportRoleAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportRoleAssignment_Report_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Report",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WebhookEventAssignment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WebhookApiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Route = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookEventAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookEventAssignment_WebhookApi_WebhookApiId",
                        column: x => x.WebhookApiId,
                        principalTable: "WebhookApi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Approval",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ApproverUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ApprovalRecordId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    StatusUpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailSendingStatus = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EmailSendingRemarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailSendingDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approval_ApprovalRecord_ApprovalRecordId",
                        column: x => x.ApprovalRecordId,
                        principalTable: "ApprovalRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeliveryTower = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DemandType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BusinessUnitId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    TechnologyBusinessPartnerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaselineStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaselineEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBudget = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ProjectDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProjectManagerId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    DeputyProjectManagerId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    ActiveStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SOWFileName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    NoSOW = table.Column<bool>(type: "bit", nullable: false)
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
                name: "ProjectMilestone",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    MilestoneId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMilestone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMilestone_Milestone_MilestoneId",
                        column: x => x.MilestoneId,
                        principalTable: "Milestone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectMilestone_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RiskIssue",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    DateRaised = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskIssue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskIssue_Employee_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RiskIssue_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatusReport",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ReportingWeekId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OverallHealth = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActualSpend = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ScheduleVarianceWeeks = table.Column<int>(type: "int", nullable: true),
                    ScheduleStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BudgetStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Phase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReviewedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusReport_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatusReport_ReportingWeek_ReportingWeekId",
                        column: x => x.ReportingWeekId,
                        principalTable: "ReportingWeek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MemberLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamMembers_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WebhookLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WebhookEventAssignmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeStarted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateTimeEnded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessResponse = table.Column<bool>(type: "bit", nullable: false),
                    ProcessResponseStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessResponseStatusError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParametarizedRoute = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    LockedByInstance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookLogs_WebhookEventAssignment_WebhookEventAssignmentId",
                        column: x => x.WebhookEventAssignmentId,
                        principalTable: "WebhookEventAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembersHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProjectHistoryId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MemberLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Accomplishment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    StatusReportId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "StatusReportHealthIndicator",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    StatusReportId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusReportHealthIndicator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusReportHealthIndicator_StatusReport_StatusReportId",
                        column: x => x.StatusReportId,
                        principalTable: "StatusReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatusReportMilestone",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    StatusReportId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusReportMilestone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusReportMilestone_StatusReport_StatusReportId",
                        column: x => x.StatusReportId,
                        principalTable: "StatusReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StatusReportRiskIssue",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    StatusReportId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    DateRaised = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusReportRiskIssue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusReportRiskIssue_Employee_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatusReportRiskIssue_StatusReport_StatusReportId",
                        column: x => x.StatusReportId,
                        principalTable: "StatusReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_Approval_ApprovalRecordId",
                table: "Approval",
                column: "ApprovalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval_ApproverUserId",
                table: "Approval",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Approval_CreatedBy",
                table: "Approval",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Approval_EmailSendingStatus",
                table: "Approval",
                column: "EmailSendingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Approval_Entity",
                table: "Approval",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_Approval_Id",
                table: "Approval",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Approval_LastModifiedBy",
                table: "Approval",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Approval_LastModifiedDate",
                table: "Approval",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Approval_Status",
                table: "Approval",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecord_ApproverSetupId",
                table: "ApprovalRecord",
                column: "ApproverSetupId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecord_CreatedBy",
                table: "ApprovalRecord",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecord_DataId",
                table: "ApprovalRecord",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecord_Entity",
                table: "ApprovalRecord",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecord_Id",
                table: "ApprovalRecord",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecord_LastModifiedBy",
                table: "ApprovalRecord",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecord_LastModifiedDate",
                table: "ApprovalRecord",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecord_Status",
                table: "ApprovalRecord",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverAssignment_ApproverSetupId_ApproverUserId_ApproverRoleId",
                table: "ApproverAssignment",
                columns: new[] { "ApproverSetupId", "ApproverUserId", "ApproverRoleId" },
                unique: true,
                filter: "[ApproverUserId] IS NOT NULL AND [ApproverRoleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverAssignment_CreatedBy",
                table: "ApproverAssignment",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverAssignment_Entity",
                table: "ApproverAssignment",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverAssignment_Id",
                table: "ApproverAssignment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverAssignment_LastModifiedBy",
                table: "ApproverAssignment",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverAssignment_LastModifiedDate",
                table: "ApproverAssignment",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverSetup_CreatedBy",
                table: "ApproverSetup",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverSetup_Entity",
                table: "ApproverSetup",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverSetup_Id",
                table: "ApproverSetup",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverSetup_LastModifiedBy",
                table: "ApproverSetup",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverSetup_LastModifiedDate",
                table: "ApproverSetup",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ApproverSetup_WorkflowName_ApprovalSetupType_TableName_Entity",
                table: "ApproverSetup",
                columns: new[] { "WorkflowName", "ApprovalSetupType", "TableName", "Entity" },
                unique: true,
                filter: "[WorkflowName] IS NOT NULL AND [TableName] IS NOT NULL AND [Entity] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_DateTime",
                table: "AuditLogs",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Id",
                table: "AuditLogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PrimaryKey",
                table: "AuditLogs",
                column: "PrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TraceId",
                table: "AuditLogs",
                column: "TraceId");

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
                name: "IX_DataSource_CreatedBy",
                table: "DataSource",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataSource_Entity",
                table: "DataSource",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DataSource_Id",
                table: "DataSource",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DataSource_LastModifiedBy",
                table: "DataSource",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataSource_LastModifiedDate",
                table: "DataSource",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DataSource_Name",
                table: "DataSource",
                column: "Name",
                unique: true);

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
                name: "IX_HTMLTemplate_HTMLTemplateName",
                table: "HTMLTemplate",
                column: "HTMLTemplateName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_CreatedBy",
                table: "Milestone",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_Entity",
                table: "Milestone",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_Id",
                table: "Milestone",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_LastModifiedBy",
                table: "Milestone",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_LastModifiedDate",
                table: "Milestone",
                column: "LastModifiedDate");

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

            migrationBuilder.CreateIndex(
                name: "IX_Project_BusinessUnitId",
                table: "Project",
                column: "BusinessUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CreatedBy",
                table: "Project",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Project_DeputyProjectManagerId",
                table: "Project",
                column: "DeputyProjectManagerId");

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
                name: "IX_Project_ProjectCode",
                table: "Project",
                column: "ProjectCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectManagerId",
                table: "Project",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_TechnologyBusinessPartnerId",
                table: "Project",
                column: "TechnologyBusinessPartnerId");

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
                name: "IX_ProjectMilestone_CreatedBy",
                table: "ProjectMilestone",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestone_Entity",
                table: "ProjectMilestone",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestone_Id",
                table: "ProjectMilestone",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestone_LastModifiedBy",
                table: "ProjectMilestone",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestone_LastModifiedDate",
                table: "ProjectMilestone",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestone_MilestoneId",
                table: "ProjectMilestone",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestone_ProjectId",
                table: "ProjectMilestone",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_CreatedBy",
                table: "Report",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Report_DataSourceId",
                table: "Report",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Entity",
                table: "Report",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Id",
                table: "Report",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Report_LastModifiedBy",
                table: "Report",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Report_LastModifiedDate",
                table: "Report",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAIIntegration_CreatedBy",
                table: "ReportAIIntegration",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAIIntegration_Entity",
                table: "ReportAIIntegration",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAIIntegration_Id",
                table: "ReportAIIntegration",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAIIntegration_LastModifiedBy",
                table: "ReportAIIntegration",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAIIntegration_LastModifiedDate",
                table: "ReportAIIntegration",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReportAIIntegration_ReportId",
                table: "ReportAIIntegration",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportingWeek_CreatedBy",
                table: "ReportingWeek",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportingWeek_Entity",
                table: "ReportingWeek",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ReportingWeek_Id",
                table: "ReportingWeek",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportingWeek_LastModifiedBy",
                table: "ReportingWeek",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportingWeek_LastModifiedDate",
                table: "ReportingWeek",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReportingWeek_StartDate",
                table: "ReportingWeek",
                column: "StartDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportingWeek_WeekNumber_Year",
                table: "ReportingWeek",
                columns: new[] { "WeekNumber", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportQueryFilter_CreatedBy",
                table: "ReportQueryFilter",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportQueryFilter_Entity",
                table: "ReportQueryFilter",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ReportQueryFilter_Id",
                table: "ReportQueryFilter",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportQueryFilter_LastModifiedBy",
                table: "ReportQueryFilter",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportQueryFilter_LastModifiedDate",
                table: "ReportQueryFilter",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReportQueryFilter_ReportId",
                table: "ReportQueryFilter",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRoleAssignment_CreatedBy",
                table: "ReportRoleAssignment",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRoleAssignment_Entity",
                table: "ReportRoleAssignment",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRoleAssignment_Id",
                table: "ReportRoleAssignment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRoleAssignment_LastModifiedBy",
                table: "ReportRoleAssignment",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRoleAssignment_LastModifiedDate",
                table: "ReportRoleAssignment",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRoleAssignment_ReportId",
                table: "ReportRoleAssignment",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_CreatedBy",
                table: "RiskIssue",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_Entity",
                table: "RiskIssue",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_Id",
                table: "RiskIssue",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_LastModifiedBy",
                table: "RiskIssue",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_LastModifiedDate",
                table: "RiskIssue",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_OwnerId",
                table: "RiskIssue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_ProjectId",
                table: "RiskIssue",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReport_CreatedBy",
                table: "StatusReport",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReport_Entity",
                table: "StatusReport",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReport_Id",
                table: "StatusReport",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReport_LastModifiedBy",
                table: "StatusReport",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReport_LastModifiedDate",
                table: "StatusReport",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReport_ProjectId",
                table: "StatusReport",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReport_ReportingWeekId",
                table: "StatusReport",
                column: "ReportingWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportHealthIndicator_CreatedBy",
                table: "StatusReportHealthIndicator",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportHealthIndicator_Entity",
                table: "StatusReportHealthIndicator",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportHealthIndicator_Id",
                table: "StatusReportHealthIndicator",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportHealthIndicator_LastModifiedBy",
                table: "StatusReportHealthIndicator",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportHealthIndicator_LastModifiedDate",
                table: "StatusReportHealthIndicator",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportHealthIndicator_StatusReportId",
                table: "StatusReportHealthIndicator",
                column: "StatusReportId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportMilestone_CreatedBy",
                table: "StatusReportMilestone",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportMilestone_Entity",
                table: "StatusReportMilestone",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportMilestone_Id",
                table: "StatusReportMilestone",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportMilestone_LastModifiedBy",
                table: "StatusReportMilestone",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportMilestone_LastModifiedDate",
                table: "StatusReportMilestone",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportMilestone_StatusReportId",
                table: "StatusReportMilestone",
                column: "StatusReportId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportRiskIssue_CreatedBy",
                table: "StatusReportRiskIssue",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportRiskIssue_Entity",
                table: "StatusReportRiskIssue",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportRiskIssue_Id",
                table: "StatusReportRiskIssue",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportRiskIssue_LastModifiedBy",
                table: "StatusReportRiskIssue",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportRiskIssue_LastModifiedDate",
                table: "StatusReportRiskIssue",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportRiskIssue_OwnerId",
                table: "StatusReportRiskIssue",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusReportRiskIssue_StatusReportId",
                table: "StatusReportRiskIssue",
                column: "StatusReportId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_CreatedBy",
                table: "TeamMembers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_EmployeeId",
                table: "TeamMembers",
                column: "EmployeeId");

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

            migrationBuilder.CreateIndex(
                name: "IX_UploadProcessor_CreatedBy",
                table: "UploadProcessor",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UploadProcessor_Entity",
                table: "UploadProcessor",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_UploadProcessor_Id",
                table: "UploadProcessor",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UploadProcessor_LastModifiedBy",
                table: "UploadProcessor",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UploadProcessor_LastModifiedDate",
                table: "UploadProcessor",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookApi_Name",
                table: "WebhookApi",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEventAssignment_EventName_WebhookApiId",
                table: "WebhookEventAssignment",
                columns: new[] { "EventName", "WebhookApiId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEventAssignment_WebhookApiId",
                table: "WebhookEventAssignment",
                column: "WebhookApiId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookLogs_DataId",
                table: "WebhookLogs",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookLogs_PendingJobs",
                table: "WebhookLogs",
                column: "CreatedDate",
                filter: "[Status] = '$Pending'")
                .Annotation("SqlServer:Include", new[] { "WebhookEventAssignmentId", "DataId", "Payload", "ParametarizedRoute" });

            migrationBuilder.CreateIndex(
                name: "IX_WebhookLogs_WebhookEventAssignmentId",
                table: "WebhookLogs",
                column: "WebhookEventAssignmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accomplishment");

            migrationBuilder.DropTable(
                name: "Approval");

            migrationBuilder.DropTable(
                name: "ApproverAssignment");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "DataSource");

            migrationBuilder.DropTable(
                name: "HTMLTemplate");

            migrationBuilder.DropTable(
                name: "NextStep");

            migrationBuilder.DropTable(
                name: "ProjectMilestone");

            migrationBuilder.DropTable(
                name: "ReportAIIntegration");

            migrationBuilder.DropTable(
                name: "ReportQueryFilter");

            migrationBuilder.DropTable(
                name: "ReportRoleAssignment");

            migrationBuilder.DropTable(
                name: "RiskIssue");

            migrationBuilder.DropTable(
                name: "StatusReportHealthIndicator");

            migrationBuilder.DropTable(
                name: "StatusReportMilestone");

            migrationBuilder.DropTable(
                name: "StatusReportRiskIssue");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "TeamMembersHistory");

            migrationBuilder.DropTable(
                name: "UploadProcessor");

            migrationBuilder.DropTable(
                name: "WebhookLogs");

            migrationBuilder.DropTable(
                name: "ApprovalRecord");

            migrationBuilder.DropTable(
                name: "Milestone");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "StatusReport");

            migrationBuilder.DropTable(
                name: "ProjectHistory");

            migrationBuilder.DropTable(
                name: "WebhookEventAssignment");

            migrationBuilder.DropTable(
                name: "ApproverSetup");

            migrationBuilder.DropTable(
                name: "ReportingWeek");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "WebhookApi");

            migrationBuilder.DropTable(
                name: "BusinessUnit");

            migrationBuilder.DropTable(
                name: "Employee");
        }
    }
}
