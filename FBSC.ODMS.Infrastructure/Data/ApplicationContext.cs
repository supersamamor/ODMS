using FBSC.Common.Data;
using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Core.ODMS;
using Microsoft.EntityFrameworkCore;
using FBSC.HTMLTemplate.Extensions;
using FBSC.ApiHub.Extensions;

namespace FBSC.ODMS.Infrastructure.Data;

public class ApplicationContext(DbContextOptions<ApplicationContext> options,
                          IAuthenticatedUser authenticatedUser) : AuditableDbContext<ApplicationContext>(options, authenticatedUser)
{
    private readonly IAuthenticatedUser _authenticatedUser = authenticatedUser;
    public DbSet<ReportState> Report { get; set; } = default!;  
    public DbSet<ReportQueryFilterState> ReportQueryFilter { get; set; } = default!;
    public DbSet<ReportRoleAssignmentState> ReportRoleAssignment { get; set; } = default!;
	public DbSet<ReportAIIntegrationState> ReportAIIntegration { get; set; } = default!;
	public DbSet<UploadProcessorState> UploadProcessor { get; set; } = default!;
	public DbSet<ApprovalState> Approval { get; set; } = default!;
	public DbSet<ApproverSetupState> ApproverSetup { get; set; } = default!;
	public DbSet<ApproverAssignmentState> ApproverAssignment { get; set; } = default!;
	public DbSet<ApprovalRecordState> ApprovalRecord { get; set; } = default!;
 
    public DbSet<DataSourceState> DataSource { get; set; } = default!;
	public DbSet<DataSourceSchemaCacheState> DataSourceSchemaCache { get; set; } = default!;
	public DbSet<DataUploadBatchState> DataUploadBatch { get; set; } = default!;
	public DbSet<DataUploadColumnState> DataUploadColumn { get; set; } = default!;
	public DbSet<ReportTypeState> ReportType { get; set; } = default!;
	public DbSet<DashboardQueryState> DashboardQuery { get; set; } = default!;
	public DbSet<DashboardQueryParameterState> DashboardQueryParameter { get; set; } = default!;
	public DbSet<DashboardQueryResultColumnState> DashboardQueryResultColumn { get; set; } = default!;
	public DbSet<DashboardQueryResultCacheState> DashboardQueryResultCache { get; set; } = default!;
	public DbSet<DashboardThemeState> DashboardTheme { get; set; } = default!;
	public DbSet<DashboardState> Dashboard { get; set; } = default!;
	public DbSet<DashboardWidgetState> DashboardWidget { get; set; } = default!;
	public DbSet<AiSqlPromptTemplateState> AiSqlPromptTemplate { get; set; } = default!;
	public DbSet<AiSqlGenerationRequestState> AiSqlGenerationRequest { get; set; } = default!;
	public DbSet<DashboardRefreshJobState> DashboardRefreshJob { get; set; } = default!;
	public DbSet<DashboardAccessState> DashboardAccess { get; set; } = default!;
	
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                                                   .SelectMany(t => t.GetProperties())
                                                   .Where(p => p.ClrType == typeof(decimal)
                                                               || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,6)");
        }
		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties()
                                               .Where(p => p.Name.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase)
                                               || p.Name.Equals("LastModifiedBy", StringComparison.OrdinalIgnoreCase)
                                               || p.Name.Equals("Entity", StringComparison.OrdinalIgnoreCase)
                                               || p.Name.Equals("LastModifiedDate", StringComparison.OrdinalIgnoreCase)
                                               || p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)))
            {
                if (!property.Name.Equals("LastModifiedDate", StringComparison.OrdinalIgnoreCase))
                {
                    property.SetMaxLength(36);
                }
                entityType.AddIndex(property);
            }
        }
        #region Disable Cascade Delete
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
        .SelectMany(t => t.GetForeignKeys())
        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
        foreach (var fk in cascadeFKs)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
        #endregion
        modelBuilder.Entity<Audit>().Property(e => e.PrimaryKey).HasMaxLength(120);
        modelBuilder.Entity<Audit>().HasIndex(p => p.PrimaryKey);
		modelBuilder.Entity<Audit>().HasIndex(p => p.TraceId);
        modelBuilder.Entity<Audit>().HasIndex(p => p.DateTime);
		modelBuilder.Entity<UploadProcessorState>().HasQueryFilter(e => _authenticatedUser.Entity!.Equals(Core.Constants.Entities.Default, StringComparison.CurrentCultureIgnoreCase) || e.Entity == _authenticatedUser.Entity);      
        // NOTE: DO NOT CREATE EXTENSION METHOD FOR QUERY FILTER!!!
        // It causes filter to be evaluated before user has signed in
        //Template:[InsertNewEFFluentAttributesTextHere]
        modelBuilder.Entity<DataSourceState>().HasIndex(p => p.Name).IsUnique();
		modelBuilder.Entity<ReportTypeState>().HasIndex(p => p.Code).IsUnique();
		modelBuilder.Entity<DashboardQueryState>().HasIndex(p => p.Name).IsUnique();
		modelBuilder.Entity<DashboardQueryState>().HasIndex(p => p.QueryHash).IsUnique();
		modelBuilder.Entity<DashboardThemeState>().HasIndex(p => p.Code).IsUnique();
		modelBuilder.Entity<DashboardState>().HasIndex(p => p.Code).IsUnique();
		
        modelBuilder.Entity<DataSourceState>().Property(e => e.Name).HasMaxLength(150);
		modelBuilder.Entity<DataSourceState>().Property(e => e.SystemType).HasMaxLength(50);
		modelBuilder.Entity<DataSourceState>().Property(e => e.ConnectionKind).HasMaxLength(30);
		modelBuilder.Entity<DataSourceState>().Property(e => e.ConnectionMode).HasMaxLength(30);
		modelBuilder.Entity<DataSourceState>().Property(e => e.ServerAddress).HasMaxLength(200);
		modelBuilder.Entity<DataSourceState>().Property(e => e.DatabaseName).HasMaxLength(150);
		modelBuilder.Entity<DataSourceState>().Property(e => e.AuthenticationType).HasMaxLength(50);
		modelBuilder.Entity<DataSourceState>().Property(e => e.Username).HasMaxLength(100);
		modelBuilder.Entity<DataSourceState>().Property(e => e.PasswordEncrypted).HasMaxLength(450);
		modelBuilder.Entity<DataSourceState>().Property(e => e.ConnectionStringEncrypted).HasMaxLength(450);
		modelBuilder.Entity<DataSourceState>().Property(e => e.Description).HasMaxLength(450);
		modelBuilder.Entity<DataSourceState>().Property(e => e.LastTestStatus).HasMaxLength(50);
		modelBuilder.Entity<DataSourceSchemaCacheState>().Property(e => e.SchemaName).HasMaxLength(100);
		modelBuilder.Entity<DataSourceSchemaCacheState>().Property(e => e.TableName).HasMaxLength(150);
		modelBuilder.Entity<DataSourceSchemaCacheState>().Property(e => e.ColumnName).HasMaxLength(150);
		modelBuilder.Entity<DataSourceSchemaCacheState>().Property(e => e.SqlDataType).HasMaxLength(50);
		modelBuilder.Entity<DataSourceSchemaCacheState>().Property(e => e.InferredSemanticType).HasMaxLength(20);
		modelBuilder.Entity<DataSourceSchemaCacheState>().HasIndex(e => new { e.DataSourceId, e.SchemaName, e.TableName, e.ColumnName }).IsUnique();
		modelBuilder.Entity<DataUploadBatchState>().Property(e => e.FileName).HasMaxLength(260);
		modelBuilder.Entity<DataUploadBatchState>().Property(e => e.SheetName).HasMaxLength(128);
		modelBuilder.Entity<DataUploadBatchState>().Property(e => e.FileType).HasMaxLength(20);
		modelBuilder.Entity<DataUploadBatchState>().Property(e => e.UploadedBy).HasMaxLength(100);
		modelBuilder.Entity<DataUploadBatchState>().Property(e => e.StagingTableName).HasMaxLength(150);
		modelBuilder.Entity<DataUploadBatchState>().Property(e => e.ImportStatus).HasMaxLength(50);
		modelBuilder.Entity<DataUploadBatchState>().Property(e => e.ErrorRemarks).HasMaxLength(450);
		modelBuilder.Entity<DataUploadBatchState>().HasIndex(e => new { e.DataSourceId, e.SheetName }).IsUnique();
		modelBuilder.Entity<DataUploadColumnState>().Property(e => e.ColumnName).HasMaxLength(150);
		modelBuilder.Entity<DataUploadColumnState>().Property(e => e.DetectedDataType).HasMaxLength(50);
		modelBuilder.Entity<DataUploadColumnState>().Property(e => e.MappedSqlDataType).HasMaxLength(50);
		modelBuilder.Entity<DataUploadColumnState>().Property(e => e.SampleValue).HasMaxLength(450);
		modelBuilder.Entity<ReportTypeState>().Property(e => e.Code).HasMaxLength(30);
		modelBuilder.Entity<ReportTypeState>().Property(e => e.Name).HasMaxLength(100);
		modelBuilder.Entity<ReportTypeState>().Property(e => e.ChartRenderer).HasMaxLength(50);
		modelBuilder.Entity<ReportTypeState>().Property(e => e.IconClass).HasMaxLength(100);
		modelBuilder.Entity<DashboardQueryState>().Property(e => e.Name).HasMaxLength(150);
		modelBuilder.Entity<DashboardQueryState>().Property(e => e.Description).HasMaxLength(450);
		modelBuilder.Entity<DashboardQueryState>().Property(e => e.QueryHash).HasMaxLength(64);
		modelBuilder.Entity<DashboardQueryState>().Property(e => e.ValidationStatus).HasMaxLength(50);
		modelBuilder.Entity<DashboardQueryState>().Property(e => e.LastExecutionErrorRemarks).HasMaxLength(450);
		modelBuilder.Entity<DashboardQueryParameterState>().Property(e => e.ParameterName).HasMaxLength(100);
		modelBuilder.Entity<DashboardQueryParameterState>().Property(e => e.DataType).HasMaxLength(50);
		modelBuilder.Entity<DashboardQueryParameterState>().Property(e => e.ControlType).HasMaxLength(50);
		modelBuilder.Entity<DashboardQueryParameterState>().Property(e => e.DefaultValue).HasMaxLength(450);
		modelBuilder.Entity<DashboardQueryResultColumnState>().Property(e => e.ColumnName).HasMaxLength(150);
		modelBuilder.Entity<DashboardQueryResultColumnState>().Property(e => e.SqlDataType).HasMaxLength(50);
		modelBuilder.Entity<DashboardQueryResultColumnState>().Property(e => e.InferredRole).HasMaxLength(50);
		modelBuilder.Entity<DashboardQueryResultColumnState>().Property(e => e.DefaultAggregation).HasMaxLength(20);
		modelBuilder.Entity<DashboardQueryResultColumnState>().Property(e => e.FormatString).HasMaxLength(50);
		modelBuilder.Entity<DashboardQueryResultCacheState>().Property(e => e.ParameterSetHash).HasMaxLength(64);
		modelBuilder.Entity<DashboardThemeState>().Property(e => e.Code).HasMaxLength(30);
		modelBuilder.Entity<DashboardThemeState>().Property(e => e.Name).HasMaxLength(100);
		modelBuilder.Entity<DashboardThemeState>().Property(e => e.PrimaryColorHex).HasMaxLength(10);
		modelBuilder.Entity<DashboardThemeState>().Property(e => e.GenerationAlgorithm).HasMaxLength(50);
		modelBuilder.Entity<DashboardState>().Property(e => e.Code).HasMaxLength(50);
		modelBuilder.Entity<DashboardState>().Property(e => e.Name).HasMaxLength(150);
		modelBuilder.Entity<DashboardState>().Property(e => e.Description).HasMaxLength(450);
		modelBuilder.Entity<DashboardState>().Property(e => e.Category).HasMaxLength(100);
		modelBuilder.Entity<DashboardState>().Property(e => e.OwnerUserId).HasMaxLength(100);
		modelBuilder.Entity<DashboardWidgetState>().Property(e => e.Title).HasMaxLength(150);
		modelBuilder.Entity<DashboardWidgetState>().Property(e => e.XAxisColumnName).HasMaxLength(150);
		modelBuilder.Entity<DashboardWidgetState>().Property(e => e.YAxisColumnsJson).HasMaxLength(450);
		modelBuilder.Entity<DashboardWidgetState>().Property(e => e.SeriesColumnName).HasMaxLength(150);
		modelBuilder.Entity<DashboardWidgetState>().Property(e => e.AggregationOverride).HasMaxLength(20);
		modelBuilder.Entity<AiSqlPromptTemplateState>().Property(e => e.SystemType).HasMaxLength(50);
		modelBuilder.Entity<AiSqlGenerationRequestState>().Property(e => e.ValidationStatus).HasMaxLength(50);
		modelBuilder.Entity<AiSqlGenerationRequestState>().Property(e => e.ErrorRemarks).HasMaxLength(450);
		modelBuilder.Entity<AiSqlGenerationRequestState>().Property(e => e.RequestedBy).HasMaxLength(100);
		modelBuilder.Entity<DashboardRefreshJobState>().Property(e => e.TriggerType).HasMaxLength(20);
		modelBuilder.Entity<DashboardRefreshJobState>().Property(e => e.Status).HasMaxLength(50);
		modelBuilder.Entity<DashboardRefreshJobState>().Property(e => e.ErrorRemarks).HasMaxLength(450);
		modelBuilder.Entity<DashboardAccessState>().Property(e => e.GranteeType).HasMaxLength(20);
		modelBuilder.Entity<DashboardAccessState>().Property(e => e.GranteeId).HasMaxLength(100);
		modelBuilder.Entity<DashboardAccessState>().Property(e => e.AccessLevel).HasMaxLength(20);
		
        modelBuilder.Entity<DataSourceState>().HasMany(t => t.DataSourceSchemaCacheList).WithOne(l => l.DataSource).HasForeignKey(t => t.DataSourceId);
		modelBuilder.Entity<DataSourceState>().HasMany(t => t.DataUploadBatchList).WithOne(l => l.DataSource).HasForeignKey(t => t.DataSourceId);
		modelBuilder.Entity<DataUploadBatchState>().HasMany(t => t.DataUploadColumnList).WithOne(l => l.DataUploadBatch).HasForeignKey(t => t.DataUploadBatchId);
		modelBuilder.Entity<DataSourceState>().HasMany(t => t.DashboardQueryList).WithOne(l => l.DataSource).HasForeignKey(t => t.DataSourceId);
		modelBuilder.Entity<DashboardQueryState>().HasMany(t => t.DashboardQueryParameterList).WithOne(l => l.DashboardQuery).HasForeignKey(t => t.DashboardQueryId);
		modelBuilder.Entity<DashboardQueryState>().HasMany(t => t.DashboardQueryResultColumnList).WithOne(l => l.DashboardQuery).HasForeignKey(t => t.DashboardQueryId);
		modelBuilder.Entity<DashboardQueryState>().HasMany(t => t.DashboardQueryResultCacheList).WithOne(l => l.DashboardQuery).HasForeignKey(t => t.DashboardQueryId);
		modelBuilder.Entity<DashboardThemeState>().HasMany(t => t.DashboardList).WithOne(l => l.DashboardTheme).HasForeignKey(t => t.DashboardThemeId);
		modelBuilder.Entity<DashboardState>().HasMany(t => t.DashboardWidgetList).WithOne(l => l.Dashboard).HasForeignKey(t => t.DashboardId);
		modelBuilder.Entity<DashboardQueryState>().HasMany(t => t.DashboardWidgetList).WithOne(l => l.DashboardQuery).HasForeignKey(t => t.DashboardQueryId);
		modelBuilder.Entity<ReportTypeState>().HasMany(t => t.DashboardWidgetList).WithOne(l => l.ReportType).HasForeignKey(t => t.ReportTypeId);
		modelBuilder.Entity<DashboardState>().HasMany(t => t.DashboardWidgetList).WithOne(l => l.Dashboard).HasForeignKey(t => t.DrillDownDashboardId);
		modelBuilder.Entity<DataSourceState>().HasMany(t => t.AiSqlGenerationRequestList).WithOne(l => l.DataSource).HasForeignKey(t => t.DataSourceId);
		modelBuilder.Entity<DashboardQueryState>().HasMany(t => t.AiSqlGenerationRequestList).WithOne(l => l.DashboardQuery).HasForeignKey(t => t.DashboardQueryId);
		modelBuilder.Entity<DashboardQueryState>().HasMany(t => t.DashboardRefreshJobList).WithOne(l => l.DashboardQuery).HasForeignKey(t => t.DashboardQueryId);
		modelBuilder.Entity<DashboardState>().HasMany(t => t.DashboardAccessList).WithOne(l => l.Dashboard).HasForeignKey(t => t.DashboardId);
		
		modelBuilder.Entity<ApprovalRecordState>().HasIndex(l => l.DataId);
		modelBuilder.Entity<ApprovalRecordState>().Property(e => e.DataId).HasMaxLength(36);
		modelBuilder.Entity<ApprovalRecordState>().Property(e => e.ApproverSetupId).HasMaxLength(36);
		modelBuilder.Entity<ApprovalRecordState>().HasIndex(l => l.ApproverSetupId);
		modelBuilder.Entity<ApprovalRecordState>().HasIndex(l => l.Status);
		modelBuilder.Entity<ApprovalRecordState>().Property(e => e.Status).HasMaxLength(450);
		modelBuilder.Entity<ApprovalState>().HasIndex(l => l.ApproverUserId);
		modelBuilder.Entity<ApprovalState>().HasIndex(l => l.Status);
		modelBuilder.Entity<ApprovalState>().HasIndex(l => l.EmailSendingStatus);
		modelBuilder.Entity<ApprovalState>().Property(e => e.ApproverUserId).HasMaxLength(36);
		modelBuilder.Entity<ApprovalState>().Property(e => e.Status).HasMaxLength(450);
		modelBuilder.Entity<ApprovalState>().Property(e => e.EmailSendingStatus).HasMaxLength(450);
		modelBuilder.Entity<ApproverSetupState>().Property(e => e.TableName).HasMaxLength(450);
		modelBuilder.Entity<ApproverSetupState>().Property(e => e.ApprovalType).HasMaxLength(450);
		modelBuilder.Entity<ApproverSetupState>().Property(e => e.EmailSubject).HasMaxLength(450);
		modelBuilder.Entity<ApproverSetupState>().Property(e => e.WorkflowName).HasMaxLength(450);
		modelBuilder.Entity<ApproverSetupState>().HasIndex(e => new { e.WorkflowName, e.ApprovalSetupType, e.TableName, e.Entity }).IsUnique();
		modelBuilder.Entity<ApproverAssignmentState>().Property(e => e.ApproverUserId).HasMaxLength(36);
		modelBuilder.Entity<ApproverAssignmentState>().Property(e => e.ApproverRoleId).HasMaxLength(36);
		modelBuilder.Entity<ApproverAssignmentState>().HasIndex(e => new { e.ApproverSetupId, e.ApproverUserId, e.ApproverRoleId }).IsUnique();
		modelBuilder.Entity<UploadProcessorState>().Property(e => e.FileType).HasMaxLength(20);
        modelBuilder.Entity<UploadProcessorState>().Property(e => e.Path).HasMaxLength(450);
        modelBuilder.Entity<UploadProcessorState>().Property(e => e.Status).HasMaxLength(20);
        modelBuilder.Entity<UploadProcessorState>().Property(e => e.Module).HasMaxLength(50);
        modelBuilder.Entity<UploadProcessorState>().Property(e => e.UploadType).HasMaxLength(50);    
		modelBuilder.ConfigureHTMLTemplateEntities();
		modelBuilder.ConfigureApiHubEntities();
        base.OnModelCreating(modelBuilder);
    }
}
