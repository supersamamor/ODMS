using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Core.ODMS;

/// <summary>
/// One uploaded Statement-of-Work (or supporting) file attached to a Project.
/// A project can have many attachments; each row points at a single file stored
/// on the secure upload volume. <see cref="StoredFileName"/> is the collision-safe
/// name written to disk, while <see cref="OriginalFileName"/> is the human-facing
/// name shown in download links.
/// </summary>
public record ProjectAttachmentState : BaseEntity
{
    public string ProjectId { get; init; } = "";
    public string StoredFileName { get; init; } = "";
    public string OriginalFileName { get; init; } = "";
    public long FileSize { get; init; }
    public string? ContentType { get; init; }

    public ProjectState? Project { get; init; }
}
