using AutoMapper;
using FBSC.HTMLTemplate.Features.HTMLTemplate.Commands;
using FBSC.HTMLTemplate.Models;
using FBSC.HTMLTemplate.ViewModels;
using FBSC.Common.Core.Mapping;

namespace FBSC.HTMLTemplate
{
    public class HTMLTemplateMappingProfile : Profile
    {
        public HTMLTemplateMappingProfile()
        {
            CreateMap<HTMLTemplateViewModel, AddHTMLTemplateCommand>();
            CreateMap<HTMLTemplateViewModel, EditHTMLTemplateCommand>();
            CreateMap<HTMLTemplateState, HTMLTemplateViewModel>().ReverseMap();
            CreateMap<AddHTMLTemplateCommand, HTMLTemplateState>();
            CreateMap<EditHTMLTemplateCommand, HTMLTemplateState>().IgnoreBaseEntityProperties();
        }
    }
}
