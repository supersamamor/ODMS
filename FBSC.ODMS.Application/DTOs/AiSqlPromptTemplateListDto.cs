using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record AiSqlPromptTemplateListDto : BaseDto
{
	public string SystemType { get; init; } = "";
	public string PromptTemplate { get; init; } = "";
	public int? Sequence { get; init; }
	public string SequenceFormatted { get { return this.Sequence == null ? "" : this.Sequence!.Value.ToString("#,##0"); } }
	public bool IsActive { get; init; }
	public string IsActiveFormatted { get { return this.IsActive == true ? "Yes" : "No"; } }
	
	
}
