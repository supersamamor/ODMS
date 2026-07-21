using System.Globalization;

namespace FBSC.ODMS.Core.Constants
{
    /// <summary>
    /// Keys and formats for the auto-generated codes issued by SequenceGenerator.
    /// Keys embed their scope so counters are independent (e.g. one ProjectCode
    /// counter per BusinessUnit, one Risk/Issue counter per type).
    /// </summary>
    public static class SequenceKeys
    {
        /// <summary>Per-BusinessUnit ProjectCode counter, e.g. "ProjectCode:{buId}".</summary>
        public static string ProjectCode(string businessUnitId) => $"ProjectCode:{businessUnitId}";

        /// <summary>Per-type Risk/Issue code counter, e.g. "RiskIssueCode:R".</summary>
        public static string RiskIssueCode(string typePrefix) => $"RiskIssueCode:{typePrefix}";
    }

    public static class CodeFormats
    {
        /// <summary>e.g. ("FLI", 1) => "FLI0000001" (business-unit code + 7-digit number).</summary>
        public static string Project(string businessUnitCode, long number) =>
            $"{businessUnitCode}{number.ToString("D7", CultureInfo.InvariantCulture)}";

        /// <summary>e.g. ("R", 1) => "R-000001" (type prefix + 6-digit number).</summary>
        public static string RiskIssue(string typePrefix, long number) =>
            $"{typePrefix}-{number.ToString("D6", CultureInfo.InvariantCulture)}";
    }

    public static class RiskIssueTypes
    {
        public const string Risk = "Risk";
        public const string Issue = "Issue";
        public const string RiskPrefix = "R";
        public const string IssuePrefix = "I";

        public static readonly string[] List = [Risk, Issue];

        public static string PrefixFor(string type) =>
            string.Equals(type, Issue, StringComparison.OrdinalIgnoreCase) ? IssuePrefix : RiskPrefix;
    }
}
