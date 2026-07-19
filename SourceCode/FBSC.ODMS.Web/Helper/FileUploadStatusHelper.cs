using FBSC.ODMS.Core.Constants;
namespace FBSC.ODMS.Web.Helper
{
    public static class FileUploadStatusHelper
    {
        public static string GetBadge(string status)
        {
            return status switch
            {
                FileUploadStatus.Pending => @"<span class=""badge bg-info"">" + status + @"</span>",
                FileUploadStatus.Done => @"<span class=""badge bg-success"">" + status + @"</span>",
                FileUploadStatus.Failed => @"<span class=""badge bg-danger"">" + status + @"</span>",
                _ => "",
            };
        }
    }
}
