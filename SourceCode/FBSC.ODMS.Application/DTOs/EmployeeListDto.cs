using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record EmployeeListDto : BaseDto
{
	public string? Department { get; init; }
	public string Email { get; init; } = "";
	public string EmployeeCode { get; init; } = "";
	public string Name { get; init; } = "";
	public string Position { get; init; } = "";
	public string? UserId { get; init; }
	
	
}
