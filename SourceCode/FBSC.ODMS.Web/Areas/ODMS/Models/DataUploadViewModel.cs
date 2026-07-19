using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DataUploadViewModel : BaseViewModel
{	
	[Display(Name = "Description")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get; init; }
	[Display(Name = "Uploaded File")]
	[Required]
	
	public IFormFile? FilePathForm { get; set; }public string? FilePath { get; init; } = "";
	public string? GeneratedFilePathPath
	{
		get
		{
			return this.FilePathForm?.FileName == null ? this.FilePath : "\\" + WebConstants.DataUpload + "\\" + this.Id + "\\" + nameof(this.FilePath) + "\\" + this.FilePathForm!.FileName;
		}
	}
	[Display(Name = "CSV, XLSX, or JSON")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string FileType { get; init; } = "";
	
	public DateTime LastModifiedDate { get; set; }
		
	
}
