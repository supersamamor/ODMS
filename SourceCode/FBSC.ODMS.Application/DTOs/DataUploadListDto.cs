using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DataUploadListDto : BaseDto
{
	public string? Description { get; init; }
	public string? FilePath { get; init; } = "";
	public string FileType { get; init; } = "";
	
	
}
