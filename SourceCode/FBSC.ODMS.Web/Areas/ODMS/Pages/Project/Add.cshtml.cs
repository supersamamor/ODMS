using FBSC.ODMS.Application.Features.ODMS.Project.Commands;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Project;

[Authorize(Policy = Permission.Project.Create)]
public class AddModel : ProjectFormPageModel<AddModel>
{
    [BindProperty]
    public ProjectViewModel Project { get; set; } = new();
    [BindProperty]
    public string? RemoveSubDetailId { get; set; }
    [BindProperty]
    public string? AsyncAction { get; set; }
    public IActionResult OnGet()
    {
		
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        ValidateSOW(Project);
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var sowResult = await SaveSOWAsync(Project);
        if (sowResult == null)
        {
            return Page();
        }
        Project = sowResult;

        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddProjectCommand>(Project)), "Details", true);
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
