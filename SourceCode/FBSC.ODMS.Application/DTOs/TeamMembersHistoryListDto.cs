using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record TeamMembersHistoryListDto : BaseDto
{
    public string? MemberLevel { get; init; } = "";
    public string Role { get; init; } = "";
    public string? Name { get; init; } = "";


}
