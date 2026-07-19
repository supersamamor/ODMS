using AutoMapper;
using FBSC.ApiHub.Features.WebhookApi.Commands;
using FBSC.ApiHub.Features.WebhookEventAssignment.Commands;
using FBSC.ApiHub.Features.WebhookLogs.Commands;
using FBSC.ApiHub.Models;
using FBSC.Common.Core.Mapping;
using FBSC.ApiHub.ViewModels;
namespace FBSC.ApiHub
{
    public class WebhookMappingProfile : Profile
    {
        public WebhookMappingProfile()
        {
            CreateMap<AddWebhookApiCommand, WebhookApiState>();
            CreateMap<EditWebhookApiCommand, WebhookApiState>().IgnoreBaseEntityProperties();
            CreateMap<AddWebhookEventAssignmentCommand, WebhookEventAssignmentState>();
            CreateMap<EditWebhookEventAssignmentCommand, WebhookEventAssignmentState>().IgnoreBaseEntityProperties();
            CreateMap<AddWebhookLogsCommand, WebhookLogsState>();
            CreateMap<EditWebhookLogsCommand, WebhookLogsState>().IgnoreBaseEntityProperties();


            CreateMap<WebhookApiViewModel, AddWebhookApiCommand>();
            CreateMap<WebhookApiViewModel, EditWebhookApiCommand>();
            CreateMap<WebhookApiState, WebhookApiViewModel>().ReverseMap();
            CreateMap<WebhookEventAssignmentViewModel, AddWebhookEventAssignmentCommand>();
            CreateMap<WebhookEventAssignmentViewModel, EditWebhookEventAssignmentCommand>();
            CreateMap<WebhookEventAssignmentState, WebhookEventAssignmentViewModel>().ForPath(e => e.ReferenceFieldWebhookApiId, o => o.MapFrom(s => s.WebhookApi!.Name));
            CreateMap<WebhookEventAssignmentViewModel, WebhookEventAssignmentState>();
            CreateMap<WebhookLogsViewModel, AddWebhookLogsCommand>();
            CreateMap<WebhookLogsViewModel, EditWebhookLogsCommand>();
            CreateMap<WebhookLogsState, WebhookLogsViewModel>()
                .ForPath(e => e.ReferenceFieldWebhookEventAssignmentId, o => o.MapFrom(s => s.WebhookEventAssignment!.EventName))
                .ForPath(e => e.WebhookApiName, o => o.MapFrom(s => s.WebhookEventAssignment!.WebhookApi!.Name));
            CreateMap<WebhookLogsViewModel, WebhookLogsState>();
        }
    }
}

