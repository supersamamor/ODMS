using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardRefreshJobController(IWebhookSecretService secretService) : BaseApiController<DashboardRefreshJobController>
{
    [Authorize(Policy = Permission.DashboardRefreshJob.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DashboardRefreshJobListDto>>> GetAsync([FromQuery] GetDashboardRefreshJobQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardRefreshJob.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardRefreshJobState>> GetAsync(string id) =>
         await HandleRequest(new GetDashboardRefreshJobByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DashboardRefreshJob.Create)]
    [HttpPost]
    public async Task<ActionResult<DashboardRefreshJobState>> PostAsync([FromBody] DashboardRefreshJobViewModel request) =>
        await HandleCommand<AddDashboardRefreshJobCommand, DashboardRefreshJobState>(Mapper.Map<AddDashboardRefreshJobCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardRefreshJob.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardRefreshJobState>> PutAsync(string id, [FromBody] DashboardRefreshJobViewModel request) =>
        await HandleCommand<EditDashboardRefreshJobCommand, DashboardRefreshJobState>(Mapper.Map<EditDashboardRefreshJobCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DashboardRefreshJob.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DashboardRefreshJobState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDashboardRefreshJobCommand, DashboardRefreshJobState>(new DeleteDashboardRefreshJobCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DashboardRefreshJobViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get;set; } = "";
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string TriggerType { get;set; } = "";
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Status { get;set; } = "";
	public DateTime? QueuedAt { get;set; } = DateTime.Now;
	public DateTime? StartedAt { get;set; } = DateTime.Now;
	public DateTime? CompletedAt { get;set; } = DateTime.Now;
	public int? DurationMs { get;set; } = 0;
	public int? RowsCached { get;set; } = 0;
	public int? RetryCount { get;set; } = 0;
	public int? MaxRetries { get;set; } = 0;
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ErrorRemarks { get;set; }
	
}
