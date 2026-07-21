using AutoMapper;
using FBSC.Common.Core.Mapping;
using FBSC.ODMS.Application.Features.ODMS.Approval.Commands;
using FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Application.Features.ODMS.Employee.Commands;
using FBSC.ODMS.Application.Features.ODMS.Milestone.Commands;
using FBSC.ODMS.Application.Features.ODMS.Project.Commands;
using FBSC.ODMS.Application.Features.ODMS.ProjectHistory.Commands;
using FBSC.ODMS.Application.Features.ODMS.Report.Commands;
using FBSC.ODMS.Application.Features.ODMS.TeamMembers.Commands;
using FBSC.ODMS.Application.Features.ODMS.TeamMembersHistory.Commands;
using FBSC.ODMS.Core.ODMS;

namespace FBSC.ODMS.Application.Features.ODMS;

public class ODMSProfile : Profile
{
    public ODMSProfile()
    {
		CreateMap<AddReportCommand, ReportState>();
        CreateMap<EditReportCommand, ReportState>().IgnoreBaseEntityProperties();
		CreateMap<AddReportAnalyticsCommand, ReportAIIntegrationState>();
		CreateMap<AddReportWithSQLQueryFromAICommand, ReportState>();
		
        CreateMap<AddDataSourceCommand, DataSourceState>();
		CreateMap<EditDataSourceCommand, DataSourceState>()
			.IgnoreBaseEntityProperties()
			.ForMember(dest => dest.PasswordEncrypted, opt => opt.Ignore())
			.ForMember(dest => dest.GeneratedTableName, opt => opt.Ignore())
			.ForMember(dest => dest.ImportStatus, opt => opt.Ignore())
			.ForMember(dest => dest.ImportErrorMessage, opt => opt.Ignore())
			.ForMember(dest => dest.LastImportedDate, opt => opt.Ignore());

		CreateMap<EditApproverSetupCommand, ApproverSetupState>().IgnoreBaseEntityProperties();
		CreateMap<AddApproverSetupCommand, ApproverSetupState>().IgnoreBaseEntityProperties();
		CreateMap<ApproverAssignmentState, ApproverAssignmentState>().IgnoreBaseEntityProperties();


        CreateMap<AddBusinessUnitCommand, BusinessUnitState>();
        CreateMap<EditBusinessUnitCommand, BusinessUnitState>().IgnoreBaseEntityProperties();
        CreateMap<AddProjectCommand, ProjectState>();
        CreateMap<EditProjectCommand, ProjectState>().IgnoreBaseEntityProperties();
        CreateMap<AddTeamMembersCommand, TeamMembersState>();
        CreateMap<EditTeamMembersCommand, TeamMembersState>().IgnoreBaseEntityProperties();
        CreateMap<AddProjectHistoryCommand, ProjectHistoryState>();
        CreateMap<EditProjectHistoryCommand, ProjectHistoryState>().IgnoreBaseEntityProperties();
        CreateMap<AddTeamMembersHistoryCommand, TeamMembersHistoryState>();
        CreateMap<EditTeamMembersHistoryCommand, TeamMembersHistoryState>().IgnoreBaseEntityProperties();
        CreateMap<AddEmployeeCommand, EmployeeState>();
        CreateMap<EditEmployeeCommand, EmployeeState>().IgnoreBaseEntityProperties();

        CreateMap<AddMilestoneCommand, MilestoneState>();
        CreateMap<EditMilestoneCommand, MilestoneState>().IgnoreBaseEntityProperties();       
    }
}
