using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.Web.Areas.Admin.Models;

public record ResourcePermissionViewModel
{
    [Display(Name = "Resource")]
    public string ResourceName { get; init; } = "";

    [Display(Name = "View")]
    public bool View { get; init; } = false;
    public string? ViewPermission { get; init; }

    [Display(Name = "Create")]
    public bool Create { get; init; } = false;
    public string? CreatePermission { get; init; }

    [Display(Name = "Edit")]
    public bool Edit { get; init; } = false;
    public string? EditPermission { get; init; }

    [Display(Name = "Delete")]
    public bool Delete { get; init; } = false;
    public string? DeletePermission { get; init; }

    [Display(Name = "Upload")]
    public bool Upload { get; init; } = false;
    public string? UploadPermission { get; init; }

    [Display(Name = "History")]
    public bool History { get; init; } = false;
    public string? HistoryPermission { get; init; }

    [Display(Name = "Approve")]
    public bool Approve { get; init; } = false;
    public string? ApprovePermission { get; init; }
}