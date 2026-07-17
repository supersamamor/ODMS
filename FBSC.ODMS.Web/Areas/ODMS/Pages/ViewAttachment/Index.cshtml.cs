using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.ViewAttachment
{
    [Authorize]
    public class IndexModel(IConfiguration configuration) : BasePageModel<IndexModel>
    {
        private readonly IConfiguration _configuration = configuration;

        public IActionResult OnGet(string subFolder, string id, string fieldName, string fileName)
        {
            try
            {
                var secureUploadFilePath = _configuration.GetValue<string>("UsersUpload:SecureUploadFilePath");
                // Construct the path to the requested file in your static folder.
                string filePath = Path.Combine(secureUploadFilePath!, subFolder, id, fieldName,
                    fileName!);

                // Serve the file.
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(fileStream, Helper.ContentTypeHelper.GetContentType(fileName!), fileName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error fetching the attachment. File Name {FileName}", fileName);
                return NotFound();
            }
        }
        public IActionResult OnGetPreview(string completeFilePath)
        {
		    completeFilePath = _configuration.GetValue<string>("UsersUpload:SecureUploadFilePath") + completeFilePath;
            if (!System.IO.File.Exists(completeFilePath))
            {
                Logger.LogWarning("File not found: {FilePath}", completeFilePath);
                return NotFound();
            }
            string fileName = Path.GetFileName(completeFilePath);
            try
            {
                string contentType = Helper.ContentTypeHelper.GetContentType(fileName!);
                var fileStream = new FileStream(completeFilePath, FileMode.Open, FileAccess.Read);
                //force inline display
                Response!.Headers!.Append("Content-Disposition", $"Inline; filename=\"{fileName}\"");
                Response!.Headers!.Append("X-Content-Type-Options", "nosniff");
                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error previewing the attachment. File naame {FileName}", fileName);
                return NotFound();
            }
        }
    }
}
