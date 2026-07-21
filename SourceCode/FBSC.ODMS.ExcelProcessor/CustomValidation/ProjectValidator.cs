using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class ProjectValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var projectName = rowValue[nameof(ProjectState.ProjectName)]?.ToString();
			if (!string.IsNullOrEmpty(projectName))
			{
				var projectNameMaxLength = 255;
				if (projectName.Length > projectNameMaxLength)
				{
					errorValidation += $"Name of the Project should be less than {projectNameMaxLength} characters.;";
				}
			}
			var businessUnitId = rowValue[nameof(ProjectState.BusinessUnitId)]?.ToString();
			var businessUnit = await context.BusinessUnit.Where(l => l.Name == businessUnitId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(businessUnit == null) {
				errorValidation += $"Assigned Business Unit is not valid.; ";
			}
			else
			{
				rowValue[nameof(ProjectState.BusinessUnitId)] = businessUnit?.Id;
			}
			var priority = rowValue[nameof(ProjectState.Priority)]?.ToString();
			if (!string.IsNullOrEmpty(priority))
			{
				var priorityMaxLength = 50;
				if (priority.Length > priorityMaxLength)
				{
					errorValidation += $"Project Priority should be less than {priorityMaxLength} characters.;";
				}
			}
			
			
			
			var projectDescription = rowValue[nameof(ProjectState.ProjectDescription)]?.ToString();
			if (!string.IsNullOrEmpty(projectDescription))
			{
				var projectDescriptionMaxLength = 1000;
				if (projectDescription.Length > projectDescriptionMaxLength)
				{
					errorValidation += $"Detailed Description of Project should be less than {projectDescriptionMaxLength} characters.;";
				}
			}
			var projectManagerId = rowValue[nameof(ProjectState.ProjectManagerId)]?.ToString();
			var employee = await context.Employee.Where(l => l.Id == projectManagerId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(employee == null) {
				errorValidation += $"Assigned Project Manager is not valid.; ";
			}
			else
			{
				rowValue[nameof(ProjectState.ProjectManagerId)] = employee?.Id;
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
