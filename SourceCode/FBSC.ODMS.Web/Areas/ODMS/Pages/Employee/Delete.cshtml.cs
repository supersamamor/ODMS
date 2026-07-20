using FBSC.ODMS.Application.Features.ODMS.Employee.Commands;
using FBSC.ODMS.Application.Features.ODMS.Employee.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Employee;

[Authorize(Policy = Permission.Employee.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public EmployeeViewModel Employee { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetEmployeeByIdQuery(id)), Employee);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteEmployeeCommand { Id = Employee.Id }), "Index");
    }
}
