namespace FBSC.ODMS.Core.Constants
{
    public static class DataTypes
    {
        public const string Date = "Date";
        public const string Years = "Years";
        public const string Months = "Months";
        public const string CustomDropdown = "Custom Dropdown";
        public const string DropdownFromTable = "Dropdown from Table";
        /// <summary>
        /// Plain integer SQL parameter (e.g. the global work-week number).
        /// Not offered in the Report Setup filter UI (DropdownServices.DataTypeList
        /// enumerates its options explicitly) - used internally by
        /// ReportDataHelper.AppendGlobalFilters.
        /// </summary>
        public const string WholeNumber = "Whole Number";
    }
}
