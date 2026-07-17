using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBSC.ODMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiSqlPromptTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    SystemType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PromptTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiSqlPromptTemplate", x => x.Id);
                });

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
                name: "DashboardTheme",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDarkMode = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryColorHex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ColorPaletteJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenerationAlgorithm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsSystemDefault = table.Column<bool>(type: "bit", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardTheme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataSource",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SystemType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConnectionMode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ServerAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DatabaseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AuthenticationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordEncrypted = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ConnectionStringEncrypted = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastTestedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastTestStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                name: "ReportType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChartRenderer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinColumnsRequired = table.Column<int>(type: "int", nullable: true),
                    MaxColumnsRequired = table.Column<int>(type: "int", nullable: true),
                    RequiresXAxis = table.Column<bool>(type: "bit", nullable: false),
                    RequiresYAxis = table.Column<bool>(type: "bit", nullable: false),
                    RequiresSeriesGrouping = table.Column<bool>(type: "bit", nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportType", x => x.Id);
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
                name: "Dashboard",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DashboardThemeId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    OwnerUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    IsTemplate = table.Column<bool>(type: "bit", nullable: false),
                    RefreshIntervalSeconds = table.Column<int>(type: "int", nullable: true),
                    LastPublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dashboard_DashboardTheme_DashboardThemeId",
                        column: x => x.DashboardThemeId,
                        principalTable: "DashboardTheme",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DashboardQuery",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DataSourceId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SqlQueryText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueryHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsParameterized = table.Column<bool>(type: "bit", nullable: false),
                    GeneratedByAI = table.Column<bool>(type: "bit", nullable: false),
                    ValidationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastExecutionDurationMs = table.Column<int>(type: "int", nullable: true),
                    LastExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastExecutionErrorRemarks = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardQuery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardQuery_DataSource_DataSourceId",
                        column: x => x.DataSourceId,
                        principalTable: "DataSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DataSourceSchemaCache",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DataSourceId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    SchemaName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SqlDataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrdinalPosition = table.Column<int>(type: "int", nullable: true),
                    IsNullable = table.Column<bool>(type: "bit", nullable: false),
                    InferredSemanticType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RefreshedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSourceSchemaCache", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataSourceSchemaCache_DataSource_DataSourceId",
                        column: x => x.DataSourceId,
                        principalTable: "DataSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DataUploadBatch",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DataSourceId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StagingTableName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RowCount = table.Column<int>(type: "int", nullable: true),
                    ColumnCount = table.Column<int>(type: "int", nullable: true),
                    ImportStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorRemarks = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataUploadBatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataUploadBatch_DataSource_DataSourceId",
                        column: x => x.DataSourceId,
                        principalTable: "DataSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "DashboardAccess",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DashboardId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    GranteeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GranteeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccessLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardAccess_Dashboard_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AiSqlGenerationRequest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DataSourceId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    NaturalLanguagePrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedSqlQueryText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DashboardQueryId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    ConfidenceScore = table.Column<int>(type: "int", nullable: true),
                    ValidationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ErrorRemarks = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiSqlGenerationRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiSqlGenerationRequest_DashboardQuery_DashboardQueryId",
                        column: x => x.DashboardQueryId,
                        principalTable: "DashboardQuery",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AiSqlGenerationRequest_DataSource_DataSourceId",
                        column: x => x.DataSourceId,
                        principalTable: "DataSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardQueryParameter",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DashboardQueryId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ControlType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    LookupSourceQuery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardQueryParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardQueryParameter_DashboardQuery_DashboardQueryId",
                        column: x => x.DashboardQueryId,
                        principalTable: "DashboardQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardQueryResultCache",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DashboardQueryId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ParameterSetHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ResultJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowCount = table.Column<int>(type: "int", nullable: true),
                    CacheSizeBytes = table.Column<int>(type: "int", nullable: true),
                    CachedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsStale = table.Column<bool>(type: "bit", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardQueryResultCache", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardQueryResultCache_DashboardQuery_DashboardQueryId",
                        column: x => x.DashboardQueryId,
                        principalTable: "DashboardQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardQueryResultColumn",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DashboardQueryId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    OrdinalPosition = table.Column<int>(type: "int", nullable: false),
                    SqlDataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InferredRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsAggregatable = table.Column<bool>(type: "bit", nullable: false),
                    DefaultAggregation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FormatString = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardQueryResultColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardQueryResultColumn_DashboardQuery_DashboardQueryId",
                        column: x => x.DashboardQueryId,
                        principalTable: "DashboardQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardRefreshJob",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DashboardQueryId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    TriggerType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QueuedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: true),
                    RowsCached = table.Column<int>(type: "int", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: true),
                    MaxRetries = table.Column<int>(type: "int", nullable: true),
                    ErrorRemarks = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardRefreshJob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardRefreshJob_DashboardQuery_DashboardQueryId",
                        column: x => x.DashboardQueryId,
                        principalTable: "DashboardQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardWidget",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DashboardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DashboardQueryId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ReportTypeId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    XAxisColumnName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    YAxisColumnsJson = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SeriesColumnName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AggregationOverride = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DrillDownDashboardId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    GridPositionX = table.Column<int>(type: "int", nullable: false),
                    GridPositionY = table.Column<int>(type: "int", nullable: false),
                    GridWidth = table.Column<int>(type: "int", nullable: false),
                    GridHeight = table.Column<int>(type: "int", nullable: false),
                    RefreshIntervalOverrideSeconds = table.Column<int>(type: "int", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: true),
                    DrillDownDashboardId1 = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardWidget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardWidget_DashboardQuery_DashboardQueryId",
                        column: x => x.DashboardQueryId,
                        principalTable: "DashboardQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DashboardWidget_Dashboard_DrillDownDashboardId",
                        column: x => x.DrillDownDashboardId,
                        principalTable: "Dashboard",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DashboardWidget_Dashboard_DrillDownDashboardId1",
                        column: x => x.DrillDownDashboardId1,
                        principalTable: "Dashboard",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DashboardWidget_ReportType_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DataUploadColumn",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DataUploadBatchId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DetectedDataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MappedSqlDataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrdinalPosition = table.Column<int>(type: "int", nullable: true),
                    SampleValue = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataUploadColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataUploadColumn_DataUploadBatch_DataUploadBatchId",
                        column: x => x.DataUploadBatchId,
                        principalTable: "DataUploadBatch",
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

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlGenerationRequest_CreatedBy",
                table: "AiSqlGenerationRequest",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlGenerationRequest_DashboardQueryId",
                table: "AiSqlGenerationRequest",
                column: "DashboardQueryId");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlGenerationRequest_DataSourceId",
                table: "AiSqlGenerationRequest",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlGenerationRequest_Entity",
                table: "AiSqlGenerationRequest",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlGenerationRequest_Id",
                table: "AiSqlGenerationRequest",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlGenerationRequest_LastModifiedBy",
                table: "AiSqlGenerationRequest",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlGenerationRequest_LastModifiedDate",
                table: "AiSqlGenerationRequest",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlPromptTemplate_CreatedBy",
                table: "AiSqlPromptTemplate",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlPromptTemplate_Entity",
                table: "AiSqlPromptTemplate",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlPromptTemplate_Id",
                table: "AiSqlPromptTemplate",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlPromptTemplate_LastModifiedBy",
                table: "AiSqlPromptTemplate",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AiSqlPromptTemplate_LastModifiedDate",
                table: "AiSqlPromptTemplate",
                column: "LastModifiedDate");

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
                name: "IX_Dashboard_Code",
                table: "Dashboard",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_CreatedBy",
                table: "Dashboard",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_DashboardThemeId",
                table: "Dashboard",
                column: "DashboardThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_Entity",
                table: "Dashboard",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_Id",
                table: "Dashboard",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_LastModifiedBy",
                table: "Dashboard",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_LastModifiedDate",
                table: "Dashboard",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardAccess_CreatedBy",
                table: "DashboardAccess",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardAccess_DashboardId",
                table: "DashboardAccess",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardAccess_Entity",
                table: "DashboardAccess",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardAccess_Id",
                table: "DashboardAccess",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardAccess_LastModifiedBy",
                table: "DashboardAccess",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardAccess_LastModifiedDate",
                table: "DashboardAccess",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQuery_CreatedBy",
                table: "DashboardQuery",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQuery_DataSourceId",
                table: "DashboardQuery",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQuery_Entity",
                table: "DashboardQuery",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQuery_Id",
                table: "DashboardQuery",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQuery_LastModifiedBy",
                table: "DashboardQuery",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQuery_LastModifiedDate",
                table: "DashboardQuery",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQuery_Name",
                table: "DashboardQuery",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQuery_QueryHash",
                table: "DashboardQuery",
                column: "QueryHash",
                unique: true,
                filter: "[QueryHash] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryParameter_CreatedBy",
                table: "DashboardQueryParameter",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryParameter_DashboardQueryId",
                table: "DashboardQueryParameter",
                column: "DashboardQueryId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryParameter_Entity",
                table: "DashboardQueryParameter",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryParameter_Id",
                table: "DashboardQueryParameter",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryParameter_LastModifiedBy",
                table: "DashboardQueryParameter",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryParameter_LastModifiedDate",
                table: "DashboardQueryParameter",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultCache_CreatedBy",
                table: "DashboardQueryResultCache",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultCache_DashboardQueryId",
                table: "DashboardQueryResultCache",
                column: "DashboardQueryId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultCache_Entity",
                table: "DashboardQueryResultCache",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultCache_Id",
                table: "DashboardQueryResultCache",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultCache_LastModifiedBy",
                table: "DashboardQueryResultCache",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultCache_LastModifiedDate",
                table: "DashboardQueryResultCache",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultColumn_CreatedBy",
                table: "DashboardQueryResultColumn",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultColumn_DashboardQueryId",
                table: "DashboardQueryResultColumn",
                column: "DashboardQueryId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultColumn_Entity",
                table: "DashboardQueryResultColumn",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultColumn_Id",
                table: "DashboardQueryResultColumn",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultColumn_LastModifiedBy",
                table: "DashboardQueryResultColumn",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardQueryResultColumn_LastModifiedDate",
                table: "DashboardQueryResultColumn",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardRefreshJob_CreatedBy",
                table: "DashboardRefreshJob",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardRefreshJob_DashboardQueryId",
                table: "DashboardRefreshJob",
                column: "DashboardQueryId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardRefreshJob_Entity",
                table: "DashboardRefreshJob",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardRefreshJob_Id",
                table: "DashboardRefreshJob",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardRefreshJob_LastModifiedBy",
                table: "DashboardRefreshJob",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardRefreshJob_LastModifiedDate",
                table: "DashboardRefreshJob",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardTheme_Code",
                table: "DashboardTheme",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DashboardTheme_CreatedBy",
                table: "DashboardTheme",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardTheme_Entity",
                table: "DashboardTheme",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardTheme_Id",
                table: "DashboardTheme",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardTheme_LastModifiedBy",
                table: "DashboardTheme",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardTheme_LastModifiedDate",
                table: "DashboardTheme",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidget_CreatedBy",
                table: "DashboardWidget",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidget_DashboardQueryId",
                table: "DashboardWidget",
                column: "DashboardQueryId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidget_DrillDownDashboardId",
                table: "DashboardWidget",
                column: "DrillDownDashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidget_DrillDownDashboardId1",
                table: "DashboardWidget",
                column: "DrillDownDashboardId1");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidget_Entity",
                table: "DashboardWidget",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidget_Id",
                table: "DashboardWidget",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidget_LastModifiedBy",
                table: "DashboardWidget",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidget_LastModifiedDate",
                table: "DashboardWidget",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidget_ReportTypeId",
                table: "DashboardWidget",
                column: "ReportTypeId");

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
                name: "IX_DataSourceSchemaCache_CreatedBy",
                table: "DataSourceSchemaCache",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceSchemaCache_DataSourceId_SchemaName_TableName_ColumnName",
                table: "DataSourceSchemaCache",
                columns: new[] { "DataSourceId", "SchemaName", "TableName", "ColumnName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceSchemaCache_Entity",
                table: "DataSourceSchemaCache",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceSchemaCache_Id",
                table: "DataSourceSchemaCache",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceSchemaCache_LastModifiedBy",
                table: "DataSourceSchemaCache",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceSchemaCache_LastModifiedDate",
                table: "DataSourceSchemaCache",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadBatch_CreatedBy",
                table: "DataUploadBatch",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadBatch_DataSourceId",
                table: "DataUploadBatch",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadBatch_Entity",
                table: "DataUploadBatch",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadBatch_Id",
                table: "DataUploadBatch",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadBatch_LastModifiedBy",
                table: "DataUploadBatch",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadBatch_LastModifiedDate",
                table: "DataUploadBatch",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadColumn_CreatedBy",
                table: "DataUploadColumn",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadColumn_DataUploadBatchId",
                table: "DataUploadColumn",
                column: "DataUploadBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadColumn_Entity",
                table: "DataUploadColumn",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadColumn_Id",
                table: "DataUploadColumn",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadColumn_LastModifiedBy",
                table: "DataUploadColumn",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DataUploadColumn_LastModifiedDate",
                table: "DataUploadColumn",
                column: "LastModifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_HTMLTemplate_HTMLTemplateName",
                table: "HTMLTemplate",
                column: "HTMLTemplateName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Report_CreatedBy",
                table: "Report",
                column: "CreatedBy");

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
                name: "IX_ReportType_Code",
                table: "ReportType",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportType_CreatedBy",
                table: "ReportType",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportType_Entity",
                table: "ReportType",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_ReportType_Id",
                table: "ReportType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReportType_LastModifiedBy",
                table: "ReportType",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportType_LastModifiedDate",
                table: "ReportType",
                column: "LastModifiedDate");

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
                name: "AiSqlGenerationRequest");

            migrationBuilder.DropTable(
                name: "AiSqlPromptTemplate");

            migrationBuilder.DropTable(
                name: "Approval");

            migrationBuilder.DropTable(
                name: "ApproverAssignment");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "DashboardAccess");

            migrationBuilder.DropTable(
                name: "DashboardQueryParameter");

            migrationBuilder.DropTable(
                name: "DashboardQueryResultCache");

            migrationBuilder.DropTable(
                name: "DashboardQueryResultColumn");

            migrationBuilder.DropTable(
                name: "DashboardRefreshJob");

            migrationBuilder.DropTable(
                name: "DashboardWidget");

            migrationBuilder.DropTable(
                name: "DataSourceSchemaCache");

            migrationBuilder.DropTable(
                name: "DataUploadColumn");

            migrationBuilder.DropTable(
                name: "HTMLTemplate");

            migrationBuilder.DropTable(
                name: "ReportAIIntegration");

            migrationBuilder.DropTable(
                name: "ReportQueryFilter");

            migrationBuilder.DropTable(
                name: "ReportRoleAssignment");

            migrationBuilder.DropTable(
                name: "UploadProcessor");

            migrationBuilder.DropTable(
                name: "WebhookLogs");

            migrationBuilder.DropTable(
                name: "ApprovalRecord");

            migrationBuilder.DropTable(
                name: "DashboardQuery");

            migrationBuilder.DropTable(
                name: "Dashboard");

            migrationBuilder.DropTable(
                name: "ReportType");

            migrationBuilder.DropTable(
                name: "DataUploadBatch");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "WebhookEventAssignment");

            migrationBuilder.DropTable(
                name: "ApproverSetup");

            migrationBuilder.DropTable(
                name: "DashboardTheme");

            migrationBuilder.DropTable(
                name: "DataSource");

            migrationBuilder.DropTable(
                name: "WebhookApi");
        }
    }
}
