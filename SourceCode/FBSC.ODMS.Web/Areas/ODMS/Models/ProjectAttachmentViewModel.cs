using FBSC.ODMS.Web.Models;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

/// <summary>
/// A single already-uploaded Statement-of-Work file, round-tripped through the
/// Project form as hidden fields so it survives async partial reloads and is
/// preserved (not deleted) when the project is saved. Newly-selected files come
/// in through <see cref="ProjectViewModel.SOWForms"/> and are turned into these
/// after being written to the secure upload volume.
/// </summary>
public record ProjectAttachmentViewModel : BaseViewModel
{
    public string ProjectId { get; init; } = "";
    public string StoredFileName { get; init; } = "";
    public string OriginalFileName { get; init; } = "";
    public long FileSize { get; init; }
    public string? ContentType { get; init; }
}
