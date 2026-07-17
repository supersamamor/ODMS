using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record AiSqlPromptTemplateState : BaseEntity
{
	public string SystemType { get; init; } = "";
	public string PromptTemplate { get; init; } = "";
	public int? Sequence { get; init; }
	public bool IsActive { get; init; }
	
	
	
}
