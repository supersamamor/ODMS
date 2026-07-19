using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FBSC.ODMS.Web.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        return Redirect("ODMS/Dashboard/Index");
    }
}
