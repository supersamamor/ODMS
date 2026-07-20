using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class TeamMembersHistoryValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var projectHistoryId = rowValue[nameof(TeamMembersHistoryState.ProjectHistoryId)]?.ToString();
			var projectHistory = await context.ProjectHistory.Where(l => l.Id == projectHistoryId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(projectHistory == null) {
				errorValidation += $"Project is not valid.; ";
			}
			else
			{
				rowValue[nameof(TeamMembersHistoryState.ProjectHistoryId)] = projectHistory?.Id;
			}
			var memberName = rowValue[nameof(TeamMembersHistoryState.MemberName)]?.ToString();
			if (!string.IsNullOrEmpty(memberName))
			{
				var memberNameMaxLength = 255;
				if (memberName.Length > memberNameMaxLength)
				{
					errorValidation += $"Full Name of the Member should be less than {memberNameMaxLength} characters.;";
				}
			}
			var role = rowValue[nameof(TeamMembersHistoryState.Role)]?.ToString();
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
