using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DashboardWidgetValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dashboardId = rowValue[nameof(DashboardWidgetState.DashboardId)]?.ToString();
			var dashboard = await context.Dashboard.Where(l => l.Code == dashboardId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dashboard == null) {
				errorValidation += $"Dashboard is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardWidgetState.DashboardId)] = dashboard?.Id;
			}
			if (!string.IsNullOrEmpty(dashboardId))
			{
				var dashboardIdMaxLength = 450;
				if (dashboardId.Length > dashboardIdMaxLength)
				{
					errorValidation += $"Dashboard should be less than {dashboardIdMaxLength} characters.;";
				}
			}
			var dashboardQueryId = rowValue[nameof(DashboardWidgetState.DashboardQueryId)]?.ToString();
			var dashboardQuery = await context.DashboardQuery.Where(l => l.QueryHash == dashboardQueryId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dashboardQuery == null) {
				errorValidation += $"Dashboard Query is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardWidgetState.DashboardQueryId)] = dashboardQuery?.Id;
			}
			if (!string.IsNullOrEmpty(dashboardQueryId))
			{
				var dashboardQueryIdMaxLength = 450;
				if (dashboardQueryId.Length > dashboardQueryIdMaxLength)
				{
					errorValidation += $"Dashboard Query should be less than {dashboardQueryIdMaxLength} characters.;";
				}
			}
			var reportTypeId = rowValue[nameof(DashboardWidgetState.ReportTypeId)]?.ToString();
			var reportType = await context.ReportType.Where(l => l.Code == reportTypeId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(reportType == null) {
				errorValidation += $"Report Type is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardWidgetState.ReportTypeId)] = reportType?.Id;
			}
			if (!string.IsNullOrEmpty(reportTypeId))
			{
				var reportTypeIdMaxLength = 450;
				if (reportTypeId.Length > reportTypeIdMaxLength)
				{
					errorValidation += $"Report Type should be less than {reportTypeIdMaxLength} characters.;";
				}
			}
			var title = rowValue[nameof(DashboardWidgetState.Title)]?.ToString();
			if (!string.IsNullOrEmpty(title))
			{
				var titleMaxLength = 150;
				if (title.Length > titleMaxLength)
				{
					errorValidation += $"Widget Title should be less than {titleMaxLength} characters.;";
				}
			}
			var xAxisColumnName = rowValue[nameof(DashboardWidgetState.XAxisColumnName)]?.ToString();
			if (!string.IsNullOrEmpty(xAxisColumnName))
			{
				var xAxisColumnNameMaxLength = 150;
				if (xAxisColumnName.Length > xAxisColumnNameMaxLength)
				{
					errorValidation += $"Maps To A DashboardQueryResultColumn.ColumnName should be less than {xAxisColumnNameMaxLength} characters.;";
				}
			}
			var yAxisColumnsJson = rowValue[nameof(DashboardWidgetState.YAxisColumnsJson)]?.ToString();
			if (!string.IsNullOrEmpty(yAxisColumnsJson))
			{
				var yAxisColumnsJsonMaxLength = 450;
				if (yAxisColumnsJson.Length > yAxisColumnsJsonMaxLength)
				{
					errorValidation += $"Json Array Of Measure Column Names should be less than {yAxisColumnsJsonMaxLength} characters.;";
				}
			}
			var seriesColumnName = rowValue[nameof(DashboardWidgetState.SeriesColumnName)]?.ToString();
			if (!string.IsNullOrEmpty(seriesColumnName))
			{
				var seriesColumnNameMaxLength = 150;
				if (seriesColumnName.Length > seriesColumnNameMaxLength)
				{
					errorValidation += $"Optional Grouping/Series Column should be less than {seriesColumnNameMaxLength} characters.;";
				}
			}
			var aggregationOverride = rowValue[nameof(DashboardWidgetState.AggregationOverride)]?.ToString();
			if (!string.IsNullOrEmpty(aggregationOverride))
			{
				var aggregationOverrideMaxLength = 20;
				if (aggregationOverride.Length > aggregationOverrideMaxLength)
				{
					errorValidation += $"Overrides The Result Column`s Default Aggregation should be less than {aggregationOverrideMaxLength} characters.;";
				}
			}
			var drillDownDashboardId = rowValue[nameof(DashboardWidgetState.DrillDownDashboardId)]?.ToString();
			var drillDownDashboard = await context.Dashboard.Where(l => l.Code == drillDownDashboardId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(drillDownDashboard == null) {
				errorValidation += $"Optional Dashboard Opened On Widget Click is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardWidgetState.DrillDownDashboardId)] = dashboard?.Id;
			}
			if (!string.IsNullOrEmpty(drillDownDashboardId))
			{
				var drillDownDashboardIdMaxLength = 450;
				if (drillDownDashboardId.Length > drillDownDashboardIdMaxLength)
				{
					errorValidation += $"Optional Dashboard Opened On Widget Click should be less than {drillDownDashboardIdMaxLength} characters.;";
				}
			}
			
			
			
			
			
			
			
			if (!string.IsNullOrEmpty(errorValidation))
			{
				throw new Exception(errorValidation);
			}
            return rowValue;
        }
			
		public static Dictionary<string, HashSet<int>> DuplicateValidation(List<ExcelRecord> records)
		{
			List<string> listOfKeys = [
																																																																				
			];
			return listOfKeys.Count > 0 ? DictionaryHelper.FindDuplicateRowNumbersPerKey(records, listOfKeys) : [];
		}
    }
}
