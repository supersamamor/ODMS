using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Areas.Admin.Queries.AuditTrail;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FBSC.ODMS.Core.Identity;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.AuditTrail;

[Authorize(Policy = Permission.AuditTrail.View)]
public class DetailsModel(UserManager<ApplicationUser> userManager) : BasePageModel<DetailsModel>
{
    public AuditLogViewModel AuditLog { get; set; } = new();

    public async Task<IActionResult> OnGet(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        return await Mediatr.Send(new GetAuditLogByIdQuery((int)id)).ToActionResult(
            someAsync: async e =>
            {
                Mapper.Map(e, AuditLog);
				if (!string.IsNullOrEmpty(e.UserId))
				{
					var user = await userManager.FindByIdAsync(e.UserId);
					AuditLog.User = Mapper.Map<AuditLogUserViewModel>(user);
				}      
				return Page();
            },
            none: null);
    }
}
