namespace FBSC.ODMS.Core.Constants
{
    /// <summary>
    /// Option lists for the weekly Status Report module. Kept as constants (not
    /// lookup tables) by the same design decision as ProjectOptions.
    /// </summary>
    public static class HealthStatuses
    {
        public const string Red = "Red";
        public const string Amber = "Amber";
        public const string Green = "Green";

        public static readonly string[] List = [Green, Amber, Red];
    }

    /// <summary>The fixed per-dimension RAG rows on a status report.</summary>
    public static class HealthIndicatorAreas
    {
        public const string Scope = "Scope";
        public const string Schedule = "Schedule";
        public const string Budget = "Budget";
        public const string Quality = "Quality";
        public const string Risks = "Risks";
        public const string Issues = "Issues";

        public static readonly string[] List = [Scope, Schedule, Budget, Quality, Risks, Issues];
    }

    public static class StatusReportMilestoneStatuses
    {
        public const string NotStarted = "Not Started";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";

        public static readonly string[] List = [NotStarted, InProgress, Completed];
    }

    public static class RiskIssueSeverities
    {
        public const string High = "High";
        public const string Medium = "Medium";
        public const string Low = "Low";

        public static readonly string[] List = [High, Medium, Low];
    }

    public static class RiskIssueStatuses
    {
        public const string Open = "Open";
        public const string InProgress = "In Progress";
        public const string Closed = "Closed";

        public static readonly string[] List = [Open, InProgress, Closed];
    }

    public static class StatusReportStatuses
    {
        public const string PendingReview = "Pending Review";
        public const string Approved = "Approved";
        public const string ChangesRequested = "Changes Requested";

        public static readonly string[] List = [PendingReview, Approved, ChangesRequested];
    }
}
