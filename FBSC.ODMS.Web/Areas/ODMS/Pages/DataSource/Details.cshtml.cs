using FBSC.Common.Data;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Queries;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataSource;

[Authorize(Policy = Permission.DataSource.View)]
public class DetailsModel(ApplicationContext context, DataSourceConnectionFactory connectionFactory) : BasePageModel<DetailsModel>
{
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

    /// <summary>
    /// Verifies the stored credentials can actually open a connection. Not an entity
    /// mutation in the usual sense, so it needs its own explicit audit entry alongside the
    /// LastTestedAt/LastTestStatus update (which the auto-diffing AuditableDbContext already
    /// covers).
    /// </summary>
    [Authorize(Policy = Permission.DataSource.Approve)]
    public async Task<IActionResult> OnPostTestConnection(string id)
    {
        var dataSource = await context.DataSource.SingleAsync(d => d.Id == id);
        var (success, errorMessage) = await connectionFactory.TestConnectionAsync(dataSource);

        var now = DateTime.UtcNow;
        var updated = dataSource with
        {
            LastTestedAt = now,
            LastTestStatus = success ? "Success" : "Failed",
        };
        context.Entry(dataSource).CurrentValues.SetValues(updated);
        context.AuditLogs.Add(new Audit
        {
            UserId = User.Identity?.Name,
            TraceId = HttpContext.TraceIdentifier,
            Type = "TestConnection",
            TableName = nameof(Core.ODMS.DataSourceState).Replace("State", ""),
            PrimaryKey = dataSource.Id,
            DateTime = now,
            NewValues = System.Text.Json.JsonSerializer.Serialize(new { Status = success ? "Success" : "Failed", ErrorMessage = errorMessage }),
        });
        await context.SaveChangesAsync();

        if (success)
        {
            NotyfService.Success(Localizer["Connection test succeeded."]);
        }
        else
        {
            NotyfService.Error(Localizer["Connection test failed"] + $": {errorMessage}");
        }
        return RedirectToPage(new { id });
    }
}
