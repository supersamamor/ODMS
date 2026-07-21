using FBSC.Common.Data;
using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Core.ODMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
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
    public DbSet<ProjectAttachmentState> ProjectAttachment { get; set; } = default!;
    public DbSet<BusinessUnitTechnologyBusinessPartnerState> BusinessUnitTechnologyBusinessPartner { get; set; } = default!;
    public DbSet<EmployeeState> Employee { get; set; } = default!;
    public DbSet<StatusReportState> StatusReport { get; set; } = default!;
    public DbSet<StatusReportHealthIndicatorState> StatusReportHealthIndicator { get; set; } = default!;
    public DbSet<MilestoneState> Milestone { get; set; } = default!;
    public DbSet<ProjectMilestoneState> ProjectMilestone { get; set; } = default!;
    public DbSet<RiskIssueState> RiskIssue { get; set; } = default!;
    public DbSet<ReportingWeekState> ReportingWeek { get; set; } = default!;
    public DbSet<StatusReportMilestoneState> StatusReportMilestone { get; set; } = default!;
    public DbSet<StatusReportRiskIssueState> StatusReportRiskIssue { get; set; } = default!;
    public DbSet<SequenceCounterState> SequenceCounter { get; set; } = default!;

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
        modelBuilder.Entity<ApproverSetupState>().Property(e => e.DeliveryCategory).HasMaxLength(50);
        modelBuilder.Entity<ApproverSetupState>().Property(e => e.ApprovalType).HasMaxLength(450);
        modelBuilder.Entity<ApproverSetupState>().Property(e => e.EmailSubject).HasMaxLength(450);
        modelBuilder.Entity<ApproverSetupState>().Property(e => e.WorkflowName).HasMaxLength(450);
        // Composite unique so multiple setups can exist for the same table under
        // different delivery categories (one fallback with null category per table).
        modelBuilder.Entity<ApproverSetupState>().HasIndex(e => new { e.TableName, e.DeliveryCategory }).IsUnique();
        modelBuilder.Entity<ApproverAssignmentState>().Property(e => e.ApproverUserId).HasMaxLength(36);
        modelBuilder.Entity<ApproverAssignmentState>().Property(e => e.ApproverRoleId).HasMaxLength(36);
        modelBuilder.Entity<ApproverAssignmentState>().HasIndex(e => new { e.ApproverSetupId, e.ApproverUserId, e.ApproverRoleId }).IsUnique();
        modelBuilder.Entity<UploadProcessorState>().Property(e => e.FileType).HasMaxLength(20);
        modelBuilder.Entity<UploadProcessorState>().Property(e => e.Path).HasMaxLength(450);
        modelBuilder.Entity<UploadProcessorState>().Property(e => e.Status).HasMaxLength(20);
        modelBuilder.Entity<UploadProcessorState>().Property(e => e.Module).HasMaxLength(50);
        modelBuilder.Entity<UploadProcessorState>().Property(e => e.UploadType).HasMaxLength(50);

        modelBuilder.Entity<BusinessUnitState>().Property(e => e.Name).HasMaxLength(150);
        modelBuilder.Entity<BusinessUnitState>().Property(e => e.Code).HasMaxLength(20);
        modelBuilder.Entity<BusinessUnitState>().HasIndex(e => e.Code).IsUnique();

        // Sequence counter: Key/Value only (no BaseEntity), Key is the PK.
        modelBuilder.Entity<SequenceCounterState>().HasKey(e => e.Key);
        modelBuilder.Entity<SequenceCounterState>().Property(e => e.Key).HasMaxLength(100);
        modelBuilder.Entity<SequenceCounterState>().ToTable("SequenceCounter");

        // RiskIssue code: unique per the auto-generated R-/I- sequence.
        modelBuilder.Entity<RiskIssueState>().Property(e => e.Code).HasMaxLength(20);
        modelBuilder.Entity<RiskIssueState>().HasIndex(e => e.Code).IsUnique();
        // Project column lengths per the ODMS DatabaseStructure workbook.
        modelBuilder.Entity<ProjectState>().Property(e => e.ProjectCode).HasMaxLength(12);
        modelBuilder.Entity<ProjectState>().Property(e => e.ProjectName).HasMaxLength(255);
        modelBuilder.Entity<ProjectState>().Property(e => e.DeliveryCategory).HasMaxLength(50);
        modelBuilder.Entity<ProjectState>().Property(e => e.DemandType).HasMaxLength(100);
        modelBuilder.Entity<ProjectState>().Property(e => e.Priority).HasMaxLength(50);
        modelBuilder.Entity<ProjectState>().Property(e => e.ProjectDescription).HasMaxLength(1000);
        modelBuilder.Entity<ProjectState>().Property(e => e.ActiveStatus).HasMaxLength(50);
        modelBuilder.Entity<ProjectState>().HasIndex(e => e.ProjectCode).IsUnique();
        modelBuilder.Entity<ProjectAttachmentState>().Property(e => e.StoredFileName).HasMaxLength(450);
        modelBuilder.Entity<ProjectAttachmentState>().Property(e => e.OriginalFileName).HasMaxLength(450);
        modelBuilder.Entity<ProjectAttachmentState>().Property(e => e.ContentType).HasMaxLength(255);
        modelBuilder.Entity<TeamMembersState>().Property(e => e.Role).HasMaxLength(100);
        modelBuilder.Entity<TeamMembersState>().Property(e => e.MemberLevel).HasMaxLength(50);
        modelBuilder.Entity<EmployeeState>().Property(e => e.Email).HasMaxLength(255);
        modelBuilder.Entity<EmployeeState>().Property(e => e.EmployeeCode).HasMaxLength(450);
        modelBuilder.Entity<EmployeeState>().Property(e => e.Name).HasMaxLength(255);
        modelBuilder.Entity<EmployeeState>().Property(e => e.Rank).HasMaxLength(50);
        modelBuilder.Entity<EmployeeState>().Property(e => e.UserId).HasMaxLength(36);

        modelBuilder.Entity<BusinessUnitState>().HasMany(t => t.ProjectList).WithOne(l => l.BusinessUnit).HasForeignKey(t => t.BusinessUnitId);
        modelBuilder.Entity<EmployeeState>().HasMany(t => t.ProjectList).WithOne(l => l.Employee).HasForeignKey(t => t.ProjectManagerId);
        modelBuilder.Entity<ProjectState>().HasMany(t => t.TeamMembersList).WithOne(l => l.Project).HasForeignKey(t => t.ProjectId);
        // A project's SOW/supporting files - same relationship shape (and global
        // Restrict delete convention) as the other Project child collections.
        modelBuilder.Entity<ProjectState>().HasMany(t => t.ProjectAttachmentList).WithOne(l => l.Project).HasForeignKey(t => t.ProjectId);
        // Additional Employee references (deputy PM, technology business partner,
        // team-member employee). NoAction avoids SQL Server's multiple-cascade-path
        // restriction - the PM FK above already owns the cascade from Employee.
        modelBuilder.Entity<ProjectState>().HasOne(t => t.DeputyProjectManager).WithMany().HasForeignKey(t => t.DeputyProjectManagerId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<ProjectState>().HasOne(t => t.TechnologyBusinessPartner).WithMany().HasForeignKey(t => t.TechnologyBusinessPartnerId).OnDelete(DeleteBehavior.NoAction);
        // Optional employee: a member row may be "(Unknown)" (null EmployeeId).
        modelBuilder.Entity<TeamMembersState>().HasOne(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);

        // BusinessUnit <-> Technology Business Partner (Employee) mapping table.
        modelBuilder.Entity<BusinessUnitState>().HasMany(t => t.TechnologyBusinessPartnerList).WithOne(l => l.BusinessUnit).HasForeignKey(t => t.BusinessUnitId);
        modelBuilder.Entity<BusinessUnitTechnologyBusinessPartnerState>().HasOne(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<BusinessUnitTechnologyBusinessPartnerState>().HasIndex(e => new { e.BusinessUnitId, e.EmployeeId }).IsUnique();

        modelBuilder.Entity<StatusReportState>().Property(e => e.OverallHealth).HasMaxLength(20);
        modelBuilder.Entity<StatusReportState>().Property(e => e.Status).HasMaxLength(50);
        modelBuilder.Entity<StatusReportState>().Property(e => e.ScheduleStatus).HasMaxLength(50);
        modelBuilder.Entity<StatusReportState>().Property(e => e.BudgetStatus).HasMaxLength(50);
        modelBuilder.Entity<StatusReportState>().Property(e => e.Phase).HasMaxLength(100);
        modelBuilder.Entity<StatusReportState>().Property(e => e.ReviewComments).HasMaxLength(1000);
        modelBuilder.Entity<StatusReportHealthIndicatorState>().Property(e => e.Area).HasMaxLength(50);
        modelBuilder.Entity<StatusReportHealthIndicatorState>().Property(e => e.Status).HasMaxLength(20);
        modelBuilder.Entity<StatusReportHealthIndicatorState>().Property(e => e.Comment).HasMaxLength(500);
        modelBuilder.Entity<MilestoneState>().Property(e => e.Name).HasMaxLength(255);
        modelBuilder.Entity<ProjectMilestoneState>().Property(e => e.Status).HasMaxLength(50);
        modelBuilder.Entity<RiskIssueState>().Property(e => e.Code).HasMaxLength(20);
        modelBuilder.Entity<RiskIssueState>().Property(e => e.Type).HasMaxLength(20);
        modelBuilder.Entity<RiskIssueState>().Property(e => e.Title).HasMaxLength(255);
        modelBuilder.Entity<RiskIssueState>().Property(e => e.Severity).HasMaxLength(20);
        modelBuilder.Entity<RiskIssueState>().Property(e => e.Status).HasMaxLength(50);
        modelBuilder.Entity<RiskIssueState>().Property(e => e.Notes).HasMaxLength(1000);
        modelBuilder.Entity<StatusReportMilestoneState>().Property(e => e.Name).HasMaxLength(255);
        modelBuilder.Entity<StatusReportMilestoneState>().Property(e => e.Status).HasMaxLength(50);
        modelBuilder.Entity<StatusReportRiskIssueState>().Property(e => e.Code).HasMaxLength(20);
        modelBuilder.Entity<StatusReportRiskIssueState>().Property(e => e.Type).HasMaxLength(20);
        modelBuilder.Entity<StatusReportRiskIssueState>().Property(e => e.Title).HasMaxLength(255);
        modelBuilder.Entity<StatusReportRiskIssueState>().Property(e => e.Severity).HasMaxLength(20);
        modelBuilder.Entity<StatusReportRiskIssueState>().Property(e => e.Status).HasMaxLength(50);
        modelBuilder.Entity<StatusReportRiskIssueState>().Property(e => e.Notes).HasMaxLength(1000);

        // Accomplishments / NextSteps: List<string> persisted as a JSON string
        // column (replaces the old Accomplishment/NextStep child tables). The
        // ValueComparer gives EF correct change tracking over list contents.
        var stringListConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions?)null),
            v => string.IsNullOrEmpty(v)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
        var stringListComparer = new ValueComparer<List<string>>(
            (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
            v => v == null ? 0 : v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item == null ? 0 : item.GetHashCode())),
            v => v == null ? new List<string>() : v.ToList());
        modelBuilder.Entity<StatusReportState>().Property(e => e.Accomplishments)
            .HasConversion(stringListConverter, stringListComparer)
            .HasColumnType("nvarchar(max)");
        modelBuilder.Entity<StatusReportState>().Property(e => e.NextSteps)
            .HasConversion(stringListConverter, stringListComparer)
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<ProjectState>().HasMany(t => t.StatusReportList).WithOne(l => l.Project).HasForeignKey(t => t.ProjectId);
        modelBuilder.Entity<ReportingWeekState>().HasMany(t => t.StatusReportList).WithOne(l => l.ReportingWeek).HasForeignKey(t => t.ReportingWeekId);
        modelBuilder.Entity<StatusReportState>().HasMany(t => t.StatusReportHealthIndicatorList).WithOne(l => l.StatusReport).HasForeignKey(t => t.StatusReportId);
        modelBuilder.Entity<ProjectState>().HasMany(t => t.ProjectMilestoneList).WithOne(l => l.Project).HasForeignKey(t => t.ProjectId);
        modelBuilder.Entity<MilestoneState>().HasMany(t => t.ProjectMilestoneList).WithOne(l => l.Milestone).HasForeignKey(t => t.MilestoneId);
        modelBuilder.Entity<ProjectState>().HasMany(t => t.RiskIssueList).WithOne(l => l.Project).HasForeignKey(t => t.ProjectId);
        modelBuilder.Entity<StatusReportState>().HasMany(t => t.StatusReportMilestoneList).WithOne(l => l.StatusReport).HasForeignKey(t => t.StatusReportId);
        modelBuilder.Entity<StatusReportState>().HasMany(t => t.StatusReportRiskIssueList).WithOne(l => l.StatusReport).HasForeignKey(t => t.StatusReportId);

        modelBuilder.Entity<ReportingWeekState>().HasIndex(e => new { e.WeekNumber, e.Year }).IsUnique();
        modelBuilder.Entity<ReportingWeekState>().HasIndex(e => e.StartDate).IsUnique();
        modelBuilder.ConfigureHTMLTemplateEntities();
        modelBuilder.ConfigureApiHubEntities();
        base.OnModelCreating(modelBuilder);
    }
}
