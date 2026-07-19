using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Toastify;
using AspNetCoreHero.ToastNotification.Toastify.Models;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Pages.Shared.Components.ToastifySafe;

[ViewComponent(Name = "ToastifySafe")]
public class ToastifyViewComponent(IToastifyService service, ToastifyEntity options) : ViewComponent
{
    private readonly IToastifyService _service = service;
    private readonly ToastifyEntity _options = options;

    public IViewComponentResult Invoke()
    {
        var model = new ToastifyViewModel
        {
            Configuration = _options,
            Notifications = _service.ReadAllNotifications()
        };
        return View("Default", model);
    }
}