using FBSC.ODMS.Application.Features.ODMS.Project.Commands;
using FBSC.ODMS.Application.Features.ODMS.Project.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Project;

[Authorize(Policy = Permission.Project.Edit)]
public class EditModel : BasePageModel<EditModel>
{
    [BindProperty]
    public ProjectViewModel Project { get; set; } = new();
    [BindProperty]
    public string? RemoveSubDetailId { get; set; }
    [BindProperty]
    public string? AsyncAction { get; set; }
    public async Task<IActionResult> OnGet(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        return await PageFrom(async () => await Mediatr.Send(new GetProjectByIdQuery(id)), Project);
    }

    public async Task<IActionResult> OnPost()
    {		
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditProjectCommand>(Project)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		if (AsyncAction == "AddTeamMembers")
		{
			return AddTeamMembers();
		}
		if (AsyncAction == "RemoveTeamMembers")
		{
			return RemoveTeamMembers();
		}
		
		
        return Partial("_InputFieldsPartial", Project);
    }
	
	private PartialViewResult AddTeamMembers()
	{
		ModelState.Clear();
		if (Project!.TeamMembersList == null) { Project!.TeamMembersList = []; }
		Project!.TeamMembersList!.Add(new TeamMembersViewModel() { ProjectId = Project.Id });
		return Partial("_InputFieldsPartial", Project);
	}
	private PartialViewResult RemoveTeamMembers()
	{
		ModelState.Clear();
		Project.TeamMembersList = [..Project!.TeamMembersList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", Project);
	}
	
}
