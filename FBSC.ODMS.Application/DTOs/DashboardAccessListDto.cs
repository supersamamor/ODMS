using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DashboardAccessListDto : BaseDto
{
	public string DashboardId { get; init; } = "";
	public string GranteeType { get; init; } = "";
	public string GranteeId { get; init; } = "";
	public string AccessLevel { get; init; } = "";
	public DateTime? GrantedAt { get; init; }
	public string GrantedAtFormatted { get { return this.GrantedAt == null ? "" : this.GrantedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	
	
}
