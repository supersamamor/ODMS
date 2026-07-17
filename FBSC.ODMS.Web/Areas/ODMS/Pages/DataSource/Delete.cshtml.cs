using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataSource;

[Authorize(Policy = Permission.DataSource.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public DataSourceViewModel DataSource { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetDataSourceByIdQuery(id)), DataSource);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteDataSourceCommand { Id = DataSource.Id }), "Index");
    }
}
