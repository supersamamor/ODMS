using FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Commands;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.AiSqlGenerationRequest;

[Authorize(Policy = Permission.AiSqlGenerationRequest.Create)]
public class AddModel(AIDashboardQueryGenerationServices aiDashboardQueryGenerationServices) : BasePageModel<AddModel>
{
    [BindProperty]
    public AiSqlGenerationRequestViewModel AiSqlGenerationRequest { get; set; } = new();
    [BindProperty]
    public string? RemoveSubDetailId { get; set; }
    [BindProperty]
    public string? AsyncAction { get; set; }
    public IActionResult OnGet()
    {

        return Page();
    }

    public async Task<IActionResult> OnPost(string handler)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (handler == "GenerateQueryFromAI")
        {
            return await OnPostGenerateQueryFromAiAsync();
        }

        // Server-assigned regardless of what was posted: an AI-generated query is always
        // PendingApproval until an explicit Approve action creates a DashboardQuery from it -
        // it can never reach the widget-usable state through this form.
        AiSqlGenerationRequest = AiSqlGenerationRequest with
        {
            ValidationStatus = QueryValidationStatus.PendingApproval,
            RequestedBy = User.Identity?.Name,
            GeneratedAt = DateTime.UtcNow,
            DashboardQueryId = null,
        };

        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddAiSqlGenerationRequestCommand>(AiSqlGenerationRequest)), "Details", true);
    }

    private async Task<IActionResult> OnPostGenerateQueryFromAiAsync()
    {
        if (string.IsNullOrEmpty(AiSqlGenerationRequest.DataSourceId) || string.IsNullOrEmpty(AiSqlGenerationRequest.NaturalLanguagePrompt))
        {
            NotyfService.Warning(Localizer["Data source and plain-language question are required to generate a query via AI."]);
            return Page();
        }

        try
        {
            var generation = await aiDashboardQueryGenerationServices.GenerateSqlAsync(
                AiSqlGenerationRequest.DataSourceId,
                AiSqlGenerationRequest.NaturalLanguagePrompt,
                cancellationToken: HttpContext.RequestAborted);

            AiSqlGenerationRequest = AiSqlGenerationRequest with
            {
                GeneratedSqlQueryText = generation.GeneratedSqlQueryText,
                ConfidenceScore = generation.ConfidenceScore,
                ValidationStatus = generation.ValidationStatus,
                ErrorRemarks = generation.ErrorRemarks,
                RequestedBy = User.Identity?.Name,
                GeneratedAt = DateTime.UtcNow,
            };

            NotyfService.Success(Localizer["SQL generated - review it before saving. It still requires explicit approval before it can be used on a widget."]);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while generating a dashboard query from AI.");
            NotyfService.Error(Localizer["An error occurred while generating query from AI."]);
        }

        ModelState.Clear();
        return Partial("_InputFieldsPartial", AiSqlGenerationRequest);
    }

    public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();

        return Partial("_InputFieldsPartial", AiSqlGenerationRequest);
    }

}
