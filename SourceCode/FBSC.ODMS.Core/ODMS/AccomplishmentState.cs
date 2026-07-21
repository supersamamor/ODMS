using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record AccomplishmentState : BaseEntity
{
	public string? StatusReportId { get; init; }
	public string? Description { get; init; }
	
	public StatusReportState? StatusReport { get; init; }
	
	
}
