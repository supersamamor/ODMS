using FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Commands;
using FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.BusinessUnit;

[Authorize(Policy = Permission.BusinessUnit.Edit)]
public class EditModel : BasePageModel<EditModel>
{
    [BindProperty]
    public BusinessUnitViewModel BusinessUnit { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetBusinessUnitByIdQuery(id)), BusinessUnit);
    }

    public async Task<IActionResult> OnPost()
    {		
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditBusinessUnitCommand>(BusinessUnit)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		if (AsyncAction == "AddTbp")
		{
			return AddTbp();
		}
		if (AsyncAction == "RemoveTbp")
		{
			return RemoveTbp();
		}

        return Partial("_InputFieldsPartial", BusinessUnit);
    }

	private PartialViewResult AddTbp()
	{
		ModelState.Clear();
		BusinessUnit.TechnologyBusinessPartnerList ??= [];
		BusinessUnit.TechnologyBusinessPartnerList.Add(new BusinessUnitTechnologyBusinessPartnerViewModel { BusinessUnitId = BusinessUnit.Id });
		return Partial("_InputFieldsPartial", BusinessUnit);
	}

	private PartialViewResult RemoveTbp()
	{
		ModelState.Clear();
		BusinessUnit.TechnologyBusinessPartnerList = [..(BusinessUnit.TechnologyBusinessPartnerList ?? []).Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", BusinessUnit);
	}

}
