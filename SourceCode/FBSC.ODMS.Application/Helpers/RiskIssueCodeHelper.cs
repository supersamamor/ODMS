using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Infrastructure.Data;

namespace FBSC.ODMS.Application.Helpers;

/// <summary>
/// Issues the auto-generated Risk/Issue code (R-000001 / I-000001). Per-type
/// counter, so Risks and Issues number independently. Call inside the same
/// transaction as the insert so the counter and row commit/roll back together.
/// </summary>
public static class RiskIssueCodeHelper
{
    public static async Task<string> GenerateAsync(ApplicationContext context, string type, CancellationToken cancellationToken = default)
    {
        var prefix = RiskIssueTypes.PrefixFor(type);
        var next = await SequenceGenerator.NextAsync(context, SequenceKeys.RiskIssueCode(prefix), cancellationToken);
        return CodeFormats.RiskIssue(prefix, next);
    }
}
