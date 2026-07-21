using AutoMapper;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.Approval.Commands;
using FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Application.Features.ODMS.Employee.Commands;
using FBSC.ODMS.Application.Features.ODMS.Milestone.Commands;
using FBSC.ODMS.Application.Features.ODMS.Project.Commands;
using FBSC.ODMS.Application.Features.ODMS.Report.Commands;
using FBSC.ODMS.Application.Features.ODMS.TeamMembers.Commands;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Web.Areas.ODMS.Models;

namespace FBSC.ODMS.Web.Areas.ODMS.Mapping;

public class ODMSProfile : Profile
{
    public ODMSProfile()
    {
		CreateMap<ReportViewModel, AddReportCommand>()
            .ForMember(dest => dest.ReportRoleAssignmentList, opt => opt.MapFrom(src => src.ReportRoleAssignmentList!.Select(x => new ReportRoleAssignmentState { RoleName = x })));
        CreateMap<ReportViewModel, EditReportCommand>()
           .ForMember(dest => dest.ReportRoleAssignmentList, opt => opt.MapFrom(src => src.ReportRoleAssignmentList!.Select(x => new ReportRoleAssignmentState { RoleName = x })));
        CreateMap<ReportState, ReportViewModel>()
            .ForMember(dest => dest.ReportRoleAssignmentList, opt => opt.MapFrom(src => src.ReportRoleAssignmentList!.Select(x => x.RoleName)));
		CreateMap<ReportViewModel, AddReportWithSQLQueryFromAICommand>()
			.ForMember(dest => dest.ReportRoleAssignmentList, opt => opt.MapFrom(src => src.ReportRoleAssignmentList!.Select(x => new ReportRoleAssignmentState { RoleName = x })));
        CreateMap<ReportViewModel, ReportState>();      
        CreateMap<ReportQueryFilterState, ReportQueryFilterViewModel>().ForPath(e => e.ForeignKeyReport, o => o.MapFrom(s => s.Report!.ReportName));
        CreateMap<ReportQueryFilterViewModel, ReportQueryFilterState>();
        CreateMap<ReportResultModel, ReportResultViewModel>().ReverseMap();
        CreateMap<ReportQueryFilterModel, ReportQueryFilterViewModel>().ReverseMap();	
		
        CreateMap<DataSourceViewModel, AddDataSourceCommand>();
		CreateMap<DataSourceViewModel, EditDataSourceCommand>();
		CreateMap<DataSourceState, DataSourceViewModel>().ReverseMap();

		CreateMap<ApproverAssignmentState, ApproverAssignmentViewModel>().ReverseMap();
		CreateMap<ApproverSetupViewModel, EditApproverSetupCommand>();
		CreateMap<ApproverSetupViewModel, AddApproverSetupCommand>();
		CreateMap<ApproverSetupState, ApproverSetupViewModel>().ReverseMap();


        CreateMap<BusinessUnitViewModel, AddBusinessUnitCommand>();
        CreateMap<BusinessUnitViewModel, EditBusinessUnitCommand>();
        CreateMap<BusinessUnitState, BusinessUnitViewModel>().ReverseMap();
        CreateMap<BusinessUnitTechnologyBusinessPartnerState, BusinessUnitTechnologyBusinessPartnerViewModel>().ReverseMap();
        CreateMap<ProjectViewModel, AddProjectCommand>();
        CreateMap<ProjectViewModel, EditProjectCommand>();
        CreateMap<ProjectState, ProjectViewModel>().ForPath(e => e.ReferenceFieldProjectManagerId, o => o.MapFrom(s => s.Employee!.Id)).ForPath(e => e.ReferenceFieldBusinessUnitId, o => o.MapFrom(s => s.BusinessUnit!.Name));
        CreateMap<ProjectViewModel, ProjectState>();
        CreateMap<ProjectAttachmentState, ProjectAttachmentViewModel>().ReverseMap();
        CreateMap<TeamMembersViewModel, AddTeamMembersCommand>();
        CreateMap<TeamMembersViewModel, EditTeamMembersCommand>();
        CreateMap<TeamMembersState, TeamMembersViewModel>().ForPath(e => e.ProjectName, o => o.MapFrom(s => s.Project!.ProjectName));
        CreateMap<TeamMembersViewModel, TeamMembersState>();
        CreateMap<EmployeeViewModel, AddEmployeeCommand>();
        CreateMap<EmployeeViewModel, EditEmployeeCommand>();
        CreateMap<EmployeeState, EmployeeViewModel>().ReverseMap();


        CreateMap<StatusReportState, StatusReportViewModel>().ForPath(e => e.ReferenceFieldProjectId, o => o.MapFrom(s => s.Project!.ProjectCode)).ForPath(e => e.ReferenceFieldReportingWeekId, o => o.MapFrom(s => s.ReportingWeek!.Id));
        CreateMap<StatusReportViewModel, StatusReportState>();

        CreateMap<StatusReportHealthIndicatorViewModel, StatusReportHealthIndicatorState>().ReverseMap();
        CreateMap<MilestoneViewModel, AddMilestoneCommand>();
        CreateMap<MilestoneViewModel, EditMilestoneCommand>();
        CreateMap<MilestoneState, MilestoneViewModel>().ReverseMap();
        CreateMap<ProjectMilestoneViewModel, ProjectMilestoneState>().ReverseMap();
        CreateMap<RiskIssueViewModel, RiskIssueState>().ReverseMap();
        CreateMap<ReportingWeekState, ReportingWeekViewModel>().ReverseMap();
        CreateMap<StatusReportMilestoneViewModel, StatusReportMilestoneState>().ReverseMap();
        CreateMap<AccomplishmentState, AccomplishmentViewModel>().ForPath(e => e.ReferenceFieldStatusReportId, o => o.MapFrom(s => s.StatusReport!.Id));
        CreateMap<AccomplishmentViewModel, AccomplishmentState>(); 
        CreateMap<NextStepState, NextStepViewModel>().ForPath(e => e.ReferenceFieldStatusReportId, o => o.MapFrom(s => s.StatusReport!.Id));
        CreateMap<NextStepViewModel, NextStepState>();
    }
}
