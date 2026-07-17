using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Commands;
using FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class AiSqlGenerationRequestController(IWebhookSecretService secretService) : BaseApiController<AiSqlGenerationRequestController>
{
    [Authorize(Policy = Permission.AiSqlGenerationRequest.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<AiSqlGenerationRequestListDto>>> GetAsync([FromQuery] GetAiSqlGenerationRequestQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.AiSqlGenerationRequest.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<AiSqlGenerationRequestState>> GetAsync(string id) =>
         await HandleRequest(new GetAiSqlGenerationRequestByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.AiSqlGenerationRequest.Create)]
    [HttpPost]
    public async Task<ActionResult<AiSqlGenerationRequestState>> PostAsync([FromBody] AiSqlGenerationRequestViewModel request) =>
        await HandleCommand<AddAiSqlGenerationRequestCommand, AiSqlGenerationRequestState>(Mapper.Map<AddAiSqlGenerationRequestCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.AiSqlGenerationRequest.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<AiSqlGenerationRequestState>> PutAsync(string id, [FromBody] AiSqlGenerationRequestViewModel request) =>
        await HandleCommand<EditAiSqlGenerationRequestCommand, AiSqlGenerationRequestState>(Mapper.Map<EditAiSqlGenerationRequestCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.AiSqlGenerationRequest.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<AiSqlGenerationRequestState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteAiSqlGenerationRequestCommand, AiSqlGenerationRequestState>(new DeleteAiSqlGenerationRequestCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record AiSqlGenerationRequestViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataSourceId { get;set; } = "";
	[Required]
	
	public string NaturalLanguagePrompt { get;set; } = "";
	
	public string? GeneratedSqlQueryText { get;set; }
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DashboardQueryId { get;set; }
	public int? ConfidenceScore { get;set; } = 0;
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ValidationStatus { get;set; }
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ErrorRemarks { get;set; }
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? RequestedBy { get;set; }
	public DateTime? GeneratedAt { get;set; } = DateTime.Now;
	
}
