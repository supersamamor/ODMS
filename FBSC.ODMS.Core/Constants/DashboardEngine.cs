namespace FBSC.ODMS.Core.Constants
{
    /// <summary>
    /// Semantic role inferred for a discovered/result-set column, used to drive
    /// auto-suggested field mappings without any per-schema code.
    /// </summary>
    public static class SemanticType
    {
        public const string Measure = "Measure";
        public const string Dimension = "Dimension";
        public const string Date = "Date";
        public const string Identifier = "Identifier";
    }

    /// <summary>
    /// Chart-role a mapped column can play on a widget. Mirrors the axis/series
    /// fields already present on DashboardWidgetState.
    /// </summary>
    public static class ChartRole
    {
        public const string Label = "Label";
        public const string XAxis = "XAxis";
        public const string YAxis = "YAxis";
        public const string Series = "Series";
        public const string Value = "Value";
        public const string Tooltip = "Tooltip";
        public const string Color = "Color";
    }

    public static class DataSourceConnectionMode
    {
        public const string ConnectionString = "ConnectionString";
        public const string ServerCredentials = "ServerCredentials";
    }

    public static class DataSourceSystemType
    {
        public const string SqlServer = "SqlServer";
    }

    public static class DataSourceAuthenticationType
    {
        public const string SqlServer = "SqlServer";
        public const string Windows = "Windows";
    }

    public static class QueryValidationStatus
    {
        public const string Draft = "Draft";
        public const string PendingApproval = "PendingApproval";
        public const string Valid = "Valid";
        public const string Invalid = "Invalid";
    }

    public static class RefreshTriggerType
    {
        public const string Scheduled = "Scheduled";
        public const string Manual = "Manual";
        public const string OnDemand = "OnDemand";
    }

    public static class RefreshJobStatus
    {
        public const string Queued = "Queued";
        public const string Running = "Running";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
    }

    public static class AggregationType
    {
        public const string None = "None";
        public const string Sum = "Sum";
        public const string Avg = "Avg";
        public const string Count = "Count";
        public const string Min = "Min";
        public const string Max = "Max";
    }

    public static class DashboardGranteeType
    {
        public const string User = "User";
        public const string Role = "Role";
    }

    public static class DashboardAccessLevel
    {
        public const string View = "View";
        public const string Edit = "Edit";
        public const string Owner = "Owner";
    }
}
