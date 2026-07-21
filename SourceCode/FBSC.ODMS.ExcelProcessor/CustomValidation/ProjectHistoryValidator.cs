using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class ProjectHistoryValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var projectId = rowValue[nameof(ProjectHistoryState.ProjectId)]?.ToString();
			var project = await context.Project.Where(l => l.Id == projectId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(project == null) {
				errorValidation += $"Reference to Project is not valid.; ";
			}
			else
			{
				rowValue[nameof(ProjectHistoryState.ProjectId)] = project?.Id;
			}
			var projectName = rowValue[nameof(ProjectHistoryState.ProjectName)]?.ToString();
			if (!string.IsNullOrEmpty(projectName))
			{
				var projectNameMaxLength = 255;
				if (projectName.Length > projectNameMaxLength)
				{
					errorValidation += $"Name of the Project should be less than {projectNameMaxLength} characters.;";
				}
			}
			var businessUnitId = rowValue[nameof(ProjectHistoryState.BusinessUnitId)]?.ToString();
			var businessUnit = await context.BusinessUnit.Where(l => l.Name == businessUnitId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(businessUnit == null) {
				errorValidation += $"Assigned Business Unit is not valid.; ";
			}
			else
			{
				rowValue[nameof(ProjectHistoryState.BusinessUnitId)] = businessUnit?.Id;
			}
			var priority = rowValue[nameof(ProjectHistoryState.Priority)]?.ToString();
			if (!string.IsNullOrEmpty(priority))
			{
				var priorityMaxLength = 50;
				if (priority.Length > priorityMaxLength)
				{
					errorValidation += $"Project Priority should be less than {priorityMaxLength} characters.;";
				}
			}
			var projectDescription = rowValue[nameof(ProjectHistoryState.ProjectDescription)]?.ToString();
			if (!string.IsNullOrEmpty(projectDescription))
			{
				var projectDescriptionMaxLength = 1000;
				if (projectDescription.Length > projectDescriptionMaxLength)
				{
					errorValidation += $"Detailed Description of Project should be less than {projectDescriptionMaxLength} characters.;";
				}
			}
			var projectManagerId = rowValue[nameof(ProjectHistoryState.ProjectManagerId)]?.ToString();
			var employee = await context.Employee.Where(l => l.Id == projectManagerId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(employee == null) {
				errorValidation += $"Assigned Project Manager is not valid.; ";
			}
			else
			{
				rowValue[nameof(ProjectHistoryState.ProjectManagerId)] = employee?.Id;
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
