using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.ViewFile
{
    [Authorize]
    public class IndexModel(IConfiguration configuration) : BasePageModel<IndexModel>
    {
        private readonly IConfiguration _configuration = configuration;

        public IActionResult OnGet(string subFolder, string fileName)
        {
            try
            {
                var secureUploadFilePath = _configuration.GetValue<string>("UsersUpload:SecureUploadFilePath");
                // Construct the path to the requested file in your static folder.
                string filePath = Path.Combine(secureUploadFilePath!, subFolder, fileName!);

                // Serve the file.
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(fileStream, Helper.ContentTypeHelper.GetContentType(fileName!), fileName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error fetching the file. File Name {FileName}", fileName);
                return NotFound();
            }
        }        
    }
}
