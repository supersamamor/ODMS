using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DataUploadState : BaseEntity
{
	public string? Description { get; init; }
	public string? FilePath { get; init; } = "";
	public string FileType { get; init; } = "";
	
	
	
}
