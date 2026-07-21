using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Project;

/// <summary>
/// Shared Statement-of-Work handling for the Project Add/Edit pages. SOW is now
/// multi-file: at least one document is required unless "No SOW yet" is checked
/// or the project already carries attachments. Each newly-selected file is
/// written to the secure upload volume under a collision-safe name and recorded
/// as a <see cref="ProjectAttachmentViewModel"/>; the original name is kept for
/// display and download links.
/// </summary>
public abstract class ProjectFormPageModel<T> : BasePageModel<T> where T : class
{
    // PDF + DOCX + common images/xlsx: the formats the security layer
    // (FileUploadHelper) has registered content signatures + macro scanning for.
    // Legacy .doc (OLE compound) has no signature/macro-scan support and is
    // intentionally excluded.
    private static readonly string[] AllowedSowExtensions = [".pdf", ".docx", ".jpg", ".jpeg", ".png", ".xlsx"];

    protected void ValidateSOW(ProjectViewModel project)
    {
        var hasNewFiles = project.SOWForms != null && project.SOWForms.Any(f => f != null && f.Length > 0);
        var hasExistingFiles = project.ProjectAttachmentList != null && project.ProjectAttachmentList.Count > 0;

        if (hasNewFiles)
        {
            foreach (var file in project.SOWForms!.Where(f => f != null && f.Length > 0))
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedSowExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Project.SOWForms", $"'{file.FileName}' must be a PDF, DOCX, image, or XLSX file.");
                }
            }
        }
        else if (!project.NoSOW && !hasExistingFiles)
        {
            ModelState.AddModelError("Project.SOWForms", "At least one Statement of Work file is required unless 'No SOW yet' is checked.");
        }
    }

    /// <summary>
    /// Writes each newly-selected SOW file to the secure upload volume and appends
    /// it to the project's attachment list (existing attachments are preserved).
    /// </summary>
    /// <returns>The view model with any new attachments added, or null when an upload failed.</returns>
    protected async Task<ProjectViewModel?> SaveSOWAsync(ProjectViewModel project)
    {
        var newFiles = project.SOWForms?.Where(f => f != null && f.Length > 0).ToList() ?? [];
        if (newFiles.Count == 0)
        {
            return project;
        }

        var attachments = project.ProjectAttachmentList?.ToList() ?? [];
        foreach (var file in newFiles)
        {
            // Store under a collision-safe name; keep the original name for display.
            var storedFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploadedPath = await UploadFile<ProjectViewModel>(
                WebConstants.Project,
                WebConstants.ProjectSowField,
                project.Id,
                file,
                fileName: storedFileName,
                permittedExtensionsOverride: AllowedSowExtensions);
            if (string.IsNullOrEmpty(uploadedPath))
            {
                return null;
            }
            attachments.Add(new ProjectAttachmentViewModel
            {
                ProjectId = project.Id,
                StoredFileName = storedFileName,
                OriginalFileName = Path.GetFileName(file.FileName),
                FileSize = file.Length,
                ContentType = file.ContentType,
            });
        }

        return project with { ProjectAttachmentList = attachments };
    }
}
