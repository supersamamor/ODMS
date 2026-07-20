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
    public DbSet<BusinessUnitState> BusinessUnit { get; set; } = default!;
    public DbSet<ProjectState> Project { get; set; } = default!;
    public DbSet<TeamMembersState> TeamMembers { get; set; } = default!;
    public DbSet<ProjectHistoryState> ProjectHistory { get; set; } = default!;
    public DbSet<TeamMembersHistoryState> TeamMembersHistory { get; set; } = default!;
    public DbSet<EmployeeState> Employee { get; set; } = default!;
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
        modelBuilder.Entity<ReportState>().Property(e => e.DataSourceId).HasMaxLength(36);
        modelBuilder.Entity<ReportState>().HasIndex(e => e.DataSourceId);
        modelBuilder.Entity<DataSourceState>().HasIndex(p => p.Name).IsUnique();

		
        modelBuilder.Entity<DataSourceState>().Property(e => e.Name).HasMaxLength(150);
		modelBuilder.Entity<DataSourceState>().Property(e => e.ServerAddress).HasMaxLength(200);
		modelBuilder.Entity<DataSourceState>().Property(e => e.DatabaseName).HasMaxLength(150);
		modelBuilder.Entity<DataSourceState>().Property(e => e.AuthenticationType).HasMaxLength(50);
		modelBuilder.Entity<DataSourceState>().Property(e => e.Username).HasMaxLength(100);
		modelBuilder.Entity<DataSourceState>().Property(e => e.PasswordEncrypted).HasMaxLength(450);
		modelBuilder.Entity<DataSourceState>().Property(e => e.ConnectionStringEncrypted).HasMaxLength(450);
		modelBuilder.Entity<DataSourceState>().Property(e => e.Description).HasMaxLength(450);
		modelBuilder.Entity<DataSourceState>().Property(e => e.DataSourceType).HasMaxLength(20);
		modelBuilder.Entity<DataSourceState>().Property(e => e.UploadedFilePath).HasMaxLength(450);
		modelBuilder.Entity<DataSourceState>().Property(e => e.GeneratedTableName).HasMaxLength(128);
		modelBuilder.Entity<DataSourceState>().Property(e => e.ImportStatus).HasMaxLength(20);
		modelBuilder.Entity<DataSourceState>().Property(e => e.ImportErrorMessage).HasMaxLength(450);
		modelBuilder.Entity<UploadProcessorState>().Property(e => e.TargetEntityId).HasMaxLength(36);
		
        
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

        modelBuilder.Entity<BusinessUnitState>().Property(e => e.Name).HasMaxLength(150);
        modelBuilder.Entity<ProjectState>().Property(e => e.ProjectName).HasMaxLength(255);
        modelBuilder.Entity<ProjectState>().Property(e => e.Priority).HasMaxLength(50);
        modelBuilder.Entity<ProjectState>().Property(e => e.ProjectDescription).HasMaxLength(1000);
        modelBuilder.Entity<ProjectState>().Property(e => e.HealthStatus).HasMaxLength(20);
        modelBuilder.Entity<ProjectState>().Property(e => e.Phase).HasMaxLength(100);
        modelBuilder.Entity<ProjectState>().Property(e => e.ScheduleStatus).HasMaxLength(50);
        modelBuilder.Entity<TeamMembersState>().Property(e => e.MemberName).HasMaxLength(255);
        modelBuilder.Entity<TeamMembersState>().Property(e => e.Role).HasMaxLength(100);
        modelBuilder.Entity<ProjectHistoryState>().Property(e => e.ProjectName).HasMaxLength(255);
        modelBuilder.Entity<ProjectHistoryState>().Property(e => e.Priority).HasMaxLength(50);
        modelBuilder.Entity<ProjectHistoryState>().Property(e => e.ProjectDescription).HasMaxLength(1000);
        modelBuilder.Entity<ProjectHistoryState>().Property(e => e.HealthStatus).HasMaxLength(20);
        modelBuilder.Entity<ProjectHistoryState>().Property(e => e.Phase).HasMaxLength(100);
        modelBuilder.Entity<ProjectHistoryState>().Property(e => e.ScheduleStatus).HasMaxLength(50);
        modelBuilder.Entity<TeamMembersHistoryState>().Property(e => e.MemberName).HasMaxLength(255);
        modelBuilder.Entity<TeamMembersHistoryState>().Property(e => e.Role).HasMaxLength(100);
        modelBuilder.Entity<EmployeeState>().Property(e => e.Email).HasMaxLength(255);
        modelBuilder.Entity<EmployeeState>().Property(e => e.EmployeeCode).HasMaxLength(450);
        modelBuilder.Entity<EmployeeState>().Property(e => e.Name).HasMaxLength(255);
        modelBuilder.Entity<EmployeeState>().Property(e => e.UserId).HasMaxLength(36);

        modelBuilder.Entity<BusinessUnitState>().HasMany(t => t.ProjectList).WithOne(l => l.BusinessUnit).HasForeignKey(t => t.BusinessUnitId);
        modelBuilder.Entity<EmployeeState>().HasMany(t => t.ProjectList).WithOne(l => l.Employee).HasForeignKey(t => t.ProjectManagerId);
        modelBuilder.Entity<ProjectState>().HasMany(t => t.TeamMembersList).WithOne(l => l.Project).HasForeignKey(t => t.ProjectId);
        modelBuilder.Entity<ProjectState>().HasMany(t => t.ProjectHistoryList).WithOne(l => l.Project).HasForeignKey(t => t.ProjectId);
        modelBuilder.Entity<BusinessUnitState>().HasMany(t => t.ProjectHistoryList).WithOne(l => l.BusinessUnit).HasForeignKey(t => t.BusinessUnitId);
        modelBuilder.Entity<EmployeeState>().HasMany(t => t.ProjectHistoryList).WithOne(l => l.Employee).HasForeignKey(t => t.ProjectManagerId);
        modelBuilder.Entity<ProjectHistoryState>().HasMany(t => t.TeamMembersHistoryList).WithOne(l => l.ProjectHistory).HasForeignKey(t => t.ProjectHistoryId);


        modelBuilder.ConfigureHTMLTemplateEntities();
		modelBuilder.ConfigureApiHubEntities();
        base.OnModelCreating(modelBuilder);
    }
}
