namespace FBSC.ODMS.Core.Constants
{
    /// <summary>
    /// SQL parameter names for the dashboard's global filter bar. Any report can
    /// opt in by referencing these as parameters in its query string, e.g.
    /// <c>WHERE (@GlobalProject = '' OR ProjectName LIKE '%' + @GlobalProject + '%')</c>.
    /// They are appended to every dashboard report execution (empty when unset),
    /// so referencing them is always safe and ignoring them costs nothing.
    /// </summary>
    public static class GlobalReportFilterFields
    {
        public const string Project = "GlobalProject";
        public const string WeekNumber = "GlobalWeekNumber";
        public const string WeekStartDate = "GlobalWeekStartDate";
        public const string WeekEndDate = "GlobalWeekEndDate";
        public const string BusinessUnit = "GlobalBusinessUnitId";
    }
}
