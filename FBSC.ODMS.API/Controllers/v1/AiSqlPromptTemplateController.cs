using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Commands;
using FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class AiSqlPromptTemplateController(IWebhookSecretService secretService) : BaseApiController<AiSqlPromptTemplateController>
{
    [Authorize(Policy = Permission.AiSqlPromptTemplate.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<AiSqlPromptTemplateListDto>>> GetAsync([FromQuery] GetAiSqlPromptTemplateQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.AiSqlPromptTemplate.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<AiSqlPromptTemplateState>> GetAsync(string id) =>
         await HandleRequest(new GetAiSqlPromptTemplateByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.AiSqlPromptTemplate.Create)]
    [HttpPost]
    public async Task<ActionResult<AiSqlPromptTemplateState>> PostAsync([FromBody] AiSqlPromptTemplateViewModel request) =>
        await HandleCommand<AddAiSqlPromptTemplateCommand, AiSqlPromptTemplateState>(Mapper.Map<AddAiSqlPromptTemplateCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.AiSqlPromptTemplate.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<AiSqlPromptTemplateState>> PutAsync(string id, [FromBody] AiSqlPromptTemplateViewModel request) =>
        await HandleCommand<EditAiSqlPromptTemplateCommand, AiSqlPromptTemplateState>(Mapper.Map<EditAiSqlPromptTemplateCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.AiSqlPromptTemplate.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<AiSqlPromptTemplateState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteAiSqlPromptTemplateCommand, AiSqlPromptTemplateState>(new DeleteAiSqlPromptTemplateCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record AiSqlPromptTemplateViewModel
{
    [Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string SystemType { get;set; } = "";
	[Required]
	
	public string PromptTemplate { get;set; } = "";
	public int? Sequence { get;set; } = 0;
	public bool IsActive { get;set; } = false;
	
}
