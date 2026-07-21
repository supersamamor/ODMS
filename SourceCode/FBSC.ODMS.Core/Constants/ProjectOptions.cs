namespace FBSC.ODMS.Core.Constants
{
    /// <summary>
    /// Option lists for the Project module's dropdown fields, per the final
    /// ODMS DatabaseStructure workbook. Kept as constants (not lookup tables)
    /// by design decision.
    /// </summary>
    public static class DeliveryTowers
    {
        public const string Infrastructure = "Infrastructure";
        public const string Application = "Application";
        public const string SmartCity = "Smart City";

        public static readonly string[] List = [Infrastructure, Application, SmartCity];
    }

    public static class DemandTypes
    {
        public const string AISolution = "AI Solution";
        public const string DataAndAnalytics = "Data & Analytics";
        public const string ApplicationDevelopment = "Application Development";
        public const string Infrastructure = "Infrastructure";
        public const string Integration = "Integration";
        public const string Others = "Others";

        public static readonly string[] List = [AISolution, DataAndAnalytics, ApplicationDevelopment, Infrastructure, Integration, Others];
    }

    public static class ProjectPriorities
    {
        public const string High = "High";
        public const string Medium = "Medium";
        public const string Low = "Low";

        public static readonly string[] List = [High, Medium, Low];
    }

    public static class ProjectActiveStatuses
    {
        public const string Active = "Active";
        public const string OnHold = "On Hold";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static readonly string[] List = [Active, OnHold, Completed, Cancelled];
    }

    public static class MemberLevels
    {
        public const string RankAndFile = "Rank & File";
        public const string Supervisor = "Supervisor";
        public const string Manager = "Manager";

        public static readonly string[] List = [RankAndFile, Supervisor, Manager];
    }
}
