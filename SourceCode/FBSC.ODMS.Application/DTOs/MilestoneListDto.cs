using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record MilestoneListDto : BaseDto
{
	public string Name { get; init; } = "";
	
	
}
