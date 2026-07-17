using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DashboardQuery.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQuery.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardQueryController(IWebhookSecretService secretService) : BaseApiController<DashboardQueryController>
{
    [Authorize(Policy = Permission.DashboardQuery.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DashboardQueryListDto>>> GetAsync([FromQuery] GetDashboardQueryQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardQuery.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardQueryState>> GetAsync(string id) =>
         await HandleRequest(new GetDashboardQueryByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DashboardQuery.Create)]
    [HttpPost]
    public async Task<ActionResult<DashboardQueryState>> PostAsync([FromBody] DashboardQueryViewModel request) =>
        await HandleCommand<AddDashboardQueryCommand, DashboardQueryState>(Mapper.Map<AddDashboardQueryCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardQuery.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardQueryState>> PutAsync(string id, [FromBody] DashboardQueryViewModel request) =>
        await HandleCommand<EditDashboardQueryCommand, DashboardQueryState>(Mapper.Map<EditDashboardQueryCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DashboardQuery.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DashboardQueryState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDashboardQueryCommand, DashboardQueryState>(new DeleteDashboardQueryCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DashboardQueryViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataSourceId { get;set; } = "";
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get;set; } = "";
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get;set; }
	[Required]
	
	public string SqlQueryText { get;set; } = "";
	[StringLength(64, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? QueryHash { get;set; }
	public bool IsParameterized { get;set; } = false;
	public bool GeneratedByAI { get;set; } = false;
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ValidationStatus { get;set; }
	public DateTime? LastValidatedAt { get;set; } = DateTime.Now;
	public int? LastExecutionDurationMs { get;set; } = 0;
	public DateTime? LastExecutedAt { get;set; } = DateTime.Now;
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? LastExecutionErrorRemarks { get;set; }
	public bool IsActive { get;set; } = false;
	
}
