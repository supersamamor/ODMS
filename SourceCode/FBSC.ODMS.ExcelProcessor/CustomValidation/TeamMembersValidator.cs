using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class TeamMembersValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var projectId = rowValue[nameof(TeamMembersState.ProjectId)]?.ToString();
			var project = await context.Project.Where(l => l.Id == projectId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(project == null) {
				errorValidation += $"Reference to Project is not valid.; ";
			}
			else
			{
				rowValue[nameof(TeamMembersState.ProjectId)] = project?.Id;
			}
			var memberName = rowValue[nameof(TeamMembersState.MemberName)]?.ToString();
			if (!string.IsNullOrEmpty(memberName))
			{
				var memberNameMaxLength = 255;
				if (memberName.Length > memberNameMaxLength)
				{
					errorValidation += $"Full Name of the Member should be less than {memberNameMaxLength} characters.;";
				}
			}
			var role = rowValue[nameof(TeamMembersState.Role)]?.ToString();
			if (!string.IsNullOrEmpty(role))
			{
				var roleMaxLength = 100;
				if (role.Length > roleMaxLength)
				{
					errorValidation += $"Role of the Member in Project should be less than {roleMaxLength} characters.;";
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
