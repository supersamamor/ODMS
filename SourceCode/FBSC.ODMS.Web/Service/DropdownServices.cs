using FBSC.Common.Data;
using FBSC.ODMS.Web.Areas.Admin.Queries.Users;
using MediatR;
using FBSC.ODMS.Web.Areas.Admin.Queries.Roles;
using FBSC.ODMS.Application.Features.ODMS.Report.Queries;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Queries;
using System.Globalization;
using FBSC.ODMS.Application.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBSC.ODMS.Web.Service
{
    public class DropdownServices(ApplicationContext context, IMediator mediaTr)
    {
        private readonly ApplicationContext _context = context;
        private readonly IMediator _mediaTr = mediaTr;

        public async Task<IEnumerable<SelectListItem>> GetRoleList()
        {
            var query = new GetRolesQuery()
            {
                PageSize = -1
            };
            return (await _mediaTr.Send(query)).Data.Select(l => new SelectListItem() { Value = l.Name, Text = l.Name });
        }
        public IEnumerable<SelectListItem> QueryTypeList()
        {
            IList<SelectListItem> items =
            [
                //new() { Text = Core.Constants.QueryType.QueryBuilder, Value = Core.Constants.QueryType.QueryBuilder, },
                new() { Text = Core.Constants.QueryType.TSql, Value = Core.Constants.QueryType.TSql, }
            ];
            return items;
        }
        public IEnumerable<SelectListItem> ReportChartTypeList()
        {
            IList<SelectListItem> items =
            [
                new() { Text = Core.Constants.ReportChartType.HorizontalBar, Value = Core.Constants.ReportChartType.HorizontalBar, },
                new() { Text = Core.Constants.ReportChartType.Bar, Value = Core.Constants.ReportChartType.Bar, },
                new() { Text = Core.Constants.ReportChartType.Pie, Value = Core.Constants.ReportChartType.Pie, },
                new() { Text = Core.Constants.ReportChartType.Doughnut, Value = Core.Constants.ReportChartType.Doughnut, },
                new() { Text = Core.Constants.ReportChartType.PolarArea, Value = Core.Constants.ReportChartType.PolarArea, },
                new() { Text = Core.Constants.ReportChartType.Line, Value = Core.Constants.ReportChartType.Line, },
                new() { Text = Core.Constants.ReportChartType.Radar, Value = Core.Constants.ReportChartType.Radar, },
                new() { Text = Core.Constants.ReportChartType.Bubble, Value = Core.Constants.ReportChartType.Bubble, },
                new() { Text = Core.Constants.ReportChartType.Scatter, Value = Core.Constants.ReportChartType.Scatter, },
                new() { Text = Core.Constants.ReportChartType.Table, Value = Core.Constants.ReportChartType.Table, },
                new() { Text = Core.Constants.ReportChartType.PDF, Value = Core.Constants.ReportChartType.PDF, },
                new() { Text = Core.Constants.ReportChartType.CustomHtml, Value = Core.Constants.ReportChartType.CustomHtml, },
            ];
            return items;
        }
        public IEnumerable<SelectListItem> AuthenticationTypeList()
        {
            IList<SelectListItem> items =
            [
                new() { Text = Core.Constants.AuthenticationTypes.SQL, Value = Core.Constants.AuthenticationTypes.SQL, },
                new() { Text = Core.Constants.AuthenticationTypes.WindowsIntegrated, Value = Core.Constants.AuthenticationTypes.WindowsIntegrated, },
                new() { Text = Core.Constants.AuthenticationTypes.AzureAD, Value = Core.Constants.AuthenticationTypes.AzureAD, },
            ];
            return items;
        }
        public IEnumerable<SelectListItem> DataSourceTypeList()
        {
            IList<SelectListItem> items =
            [
                new() { Text = "SQL Server", Value = Core.Constants.DataSourceTypes.SqlServer, },
                new() { Text = "File Upload", Value = Core.Constants.DataSourceTypes.FileUpload, },
            ];
            return items;
        }
        public IEnumerable<SelectListItem> DataTypeList()
        {
            IList<SelectListItem> items =
            [
                new() { Text = Core.Constants.DataTypes.CustomDropdown, Value = Core.Constants.DataTypes.CustomDropdown, },
                new() { Text = Core.Constants.DataTypes.Date, Value = Core.Constants.DataTypes.Date, },
                new() { Text = Core.Constants.DataTypes.DropdownFromTable, Value = Core.Constants.DataTypes.DropdownFromTable, },
                new() { Text = Core.Constants.DataTypes.Months, Value = Core.Constants.DataTypes.Months, },
                new() { Text = Core.Constants.DataTypes.Years, Value = Core.Constants.DataTypes.Years, },
            ];
            return items;
        }
        public IEnumerable<SelectListItem> GetDropdownFromCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return [];
            }
            return value.Split(',')
                         .Select(option => new SelectListItem() { Text = option.Trim(), Value = option.Trim() })
                         .ToList();
        }
        public IEnumerable<SelectListItem> GetYearsList(int yearsPrevious, int yearsAdvance)
        {
            List<SelectListItem> yearsList = [];
            int currentYear = DateTime.Now.Year;
            int startYear = currentYear - yearsPrevious;
            int endYear = currentYear + yearsAdvance;
            for (int year = startYear; year <= endYear; year++)
            {
                SelectListItem listItem = new()
                {
                    Text = year.ToString(),
                    Value = year.ToString(),
                };
                yearsList.Add(listItem);
            }
            return yearsList;
        }
        public IEnumerable<SelectListItem> GetMonthsList()
        {
            List<SelectListItem> monthsList = [];
            // Loop through the months and create SelectListItem objects for each month
            for (int month = 1; month <= 12; month++)
            {
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                SelectListItem listItem = new()
                {
                    Text = monthName,
                    Value = month.ToString() // Month number as the 'Value'
                };
                monthsList.Add(listItem);
            }
            return monthsList;
        }
        public async Task<IEnumerable<SelectListItem>> GetDropdownFromTableKeyValue(string tableKeyValue, string? filter)
        {
            var dropdownValues = await _mediaTr.Send(new GetDropdownValuesQuery(tableKeyValue, filter));
            List<SelectListItem> selectListItems = [];
            foreach (var item in dropdownValues)
            {
                string? key = item.TryGetValue("Key", out string? keyOutput) ? keyOutput : "";
                string? value = item.TryGetValue("Value", out string? valueOutput) ? valueOutput : "";
                selectListItems.Add(new()
                {
                    Text = value,
                    Value = key
                });
            }
            return selectListItems;
        }
        public async Task<IList<Dictionary<string, string>>> GetReportList()
        {
            return await _mediaTr.Send(new GetReportListQuery());
        }
        public IEnumerable<SelectListItem> WebhookEventList()
        {
            IList<SelectListItem> items =
            [
				// AI
				new() { Text = WebhookEvents.PromptGenerativeAI, Value = WebhookEvents.PromptGenerativeAI },
            ];
            return items;
        }
        public async Task<IEnumerable<SelectListItem>> DrillDownReportList()
        {
            var query = new GetReportQuery()
            {
                PageSize = -1
            };
            return (await _mediaTr.Send(query)).Data.Select(l => new SelectListItem() { Value = l.Id, Text = l.ReportName });
        }
        public async Task<IEnumerable<SelectListItem>> DataSourceList()
        {
            var query = new GetDataSourceQuery()
            {
                PageSize = -1
            };
            return (await _mediaTr.Send(query)).Data
                .Where(l => l.IsActive)
                .Select(l => new SelectListItem() { Value = l.Id, Text = l.Name });
        }

        public async Task<IEnumerable<SelectListItem>> GetUserList(string currentSelectedApprover, IList<string> allSelectedApprovers)
        {
            return (await _mediaTr.Send(new GetApproversQuery(currentSelectedApprover, allSelectedApprovers) { PageSize = -1 })).Data.Select(l => new SelectListItem { Value = l.Id, Text = l.Name });
        }
        public async Task<IEnumerable<SelectListItem>> GetRoleApproverList(string currentSelectedApprover, IList<string> allSelectedApprovers)
        {
            return (await _mediaTr.Send(new GetApproverRolesQuery(currentSelectedApprover, allSelectedApprovers) { PageSize = -1 })).Data.Select(l => new SelectListItem { Value = l.Id, Text = l.Name });
        }
        public SelectList GetProjectList(string? id)
        {
            return _context.GetSingle<ProjectState>(e => e.Id == id, new()).Result.Match(
                Some: e => new SelectList(new List<SelectListItem> { new() { Value = e.Id, Text = e.Id } }, "Value", "Text", e.Id),
                None: () => new SelectList(new List<SelectListItem>(), "Value", "Text")
            );
        }
        public SelectList GetProjectHistoryList(string? id)
        {
            return _context.GetSingle<ProjectHistoryState>(e => e.Id == id, new()).Result.Match(
                Some: e => new SelectList(new List<SelectListItem> { new() { Value = e.Id, Text = e.Id } }, "Value", "Text", e.Id),
                None: () => new SelectList(new List<SelectListItem>(), "Value", "Text")
            );
        }
        public SelectList GetEmployeeList(string? id)
        {
            return _context.GetSingle<EmployeeState>(e => e.Id == id, new()).Result.Match(
                Some: e => new SelectList(new List<SelectListItem> { new() { Value = e.Id, Text = e.Id } }, "Value", "Text", e.Id),
                None: () => new SelectList(new List<SelectListItem>(), "Value", "Text")
            );
        }
        public SelectList GetBusinessUnitList(string? id)
        {
            return _context.GetSingle<BusinessUnitState>(e => e.Id == id, new()).Result.Match(
                Some: e => new SelectList(new List<SelectListItem> { new() { Value = e.Id, Text = e.Name } }, "Value", "Text", e.Id),
                None: () => new SelectList(new List<SelectListItem>(), "Value", "Text")
            );
        }
    }
}