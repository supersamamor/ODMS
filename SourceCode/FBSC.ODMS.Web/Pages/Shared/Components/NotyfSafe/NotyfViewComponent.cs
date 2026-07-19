using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Helpers;
using AspNetCoreHero.ToastNotification.Notyf;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Pages.Shared.Components.NotyfSafe;

[ViewComponent(Name = "NotyfSafe")]
public class NotyfViewComponent(INotyfService service, NotyfEntity options) : ViewComponent
{
    public NotyfEntity Options { get; } = options;

    public IViewComponentResult Invoke()
    {
        var model = new NotyfViewModel
        {
            Configuration = Options.ToJson(),
            Notifications = service.ReadAllNotifications()
        };
        return View("Default", model);
    }
}

