using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record ReportingWeekState : BaseEntity
{
	public int Year { get; init; }
	public int WeekNumber { get; init; }
	public DateTime StartDate { get; init; }
	public DateTime EndDate { get; init; }
	
	
	public IList<StatusReportState>? StatusReportList { get; set; }
	
}
