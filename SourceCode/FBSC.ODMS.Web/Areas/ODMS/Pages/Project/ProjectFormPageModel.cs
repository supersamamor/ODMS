using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Project;

/// <summary>
/// Shared Statement-of-Work handling for the Project Add/Edit pages: the file
/// is required unless "No SOW yet" is checked, only PDF/DOC/DOCX are accepted,
/// and the uploaded file is stored via the standard secure-upload helper with
/// its path kept in SOWFileName.
/// </summary>
public abstract class ProjectFormPageModel<T> : BasePageModel<T> where T : class
{
    // PDF + DOCX only: these are the formats the security layer (FileUploadHelper)
    // has registered content signatures + macro scanning for. Legacy .doc (OLE
    // compound) has no signature/macro-scan support and is intentionally excluded.

    private static readonly string[] AllowedSowExtensions = [".pdf", ".docx", ".jpg", ".jpeg", ".png", ".xlsx"];


    protected void ValidateSOW(ProjectViewModel project)
    {
        if (project.SOWForm != null)
        {
            var extension = Path.GetExtension(project.SOWForm.FileName).ToLowerInvariant();
            if (!AllowedSowExtensions.Contains(extension))
            {
                ModelState.AddModelError("Project.SOWForm", "Statement of Work must be a PDF, DOCX, or image file.");
            }
        }
        else if (!project.NoSOW && string.IsNullOrEmpty(project.SOWFileName))
        {
            ModelState.AddModelError("Project.SOWForm", "Statement of Work is required unless 'No SOW yet' is checked.");
        }
    }

    /// <returns>The view model with SOWFileName set, or null when the upload failed.</returns>
    protected async Task<ProjectViewModel?> SaveSOWAsync(ProjectViewModel project)
    {
        if (project.SOWForm == null)
        {
            return project;
        }
        var uploadedFilePath = await UploadFile<ProjectViewModel>(WebConstants.Project, nameof(project.SOWFileName), project.Id, project.SOWForm, permittedExtensionsOverride: AllowedSowExtensions);
        if (uploadedFilePath == "")
        {
            return null;
        }
        return project with { SOWFileName = uploadedFilePath };
    }
}
