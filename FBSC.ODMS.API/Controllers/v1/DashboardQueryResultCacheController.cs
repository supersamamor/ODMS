using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultCache.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultCache.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardQueryResultCacheController(IWebhookSecretService secretService) : BaseApiController<DashboardQueryResultCacheController>
{
    [Authorize(Policy = Permission.DashboardQueryResultCache.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DashboardQueryResultCacheListDto>>> GetAsync([FromQuery] GetDashboardQueryResultCacheQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardQueryResultCache.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardQueryResultCacheState>> GetAsync(string id) =>
         await HandleRequest(new GetDashboardQueryResultCacheByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DashboardQueryResultCache.Create)]
    [HttpPost]
    public async Task<ActionResult<DashboardQueryResultCacheState>> PostAsync([FromBody] DashboardQueryResultCacheViewModel request) =>
        await HandleCommand<AddDashboardQueryResultCacheCommand, DashboardQueryResultCacheState>(Mapper.Map<AddDashboardQueryResultCacheCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardQueryResultCache.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardQueryResultCacheState>> PutAsync(string id, [FromBody] DashboardQueryResultCacheViewModel request) =>
        await HandleCommand<EditDashboardQueryResultCacheCommand, DashboardQueryResultCacheState>(Mapper.Map<EditDashboardQueryResultCacheCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DashboardQueryResultCache.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DashboardQueryResultCacheState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDashboardQueryResultCacheCommand, DashboardQueryResultCacheState>(new DeleteDashboardQueryResultCacheCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DashboardQueryResultCacheViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get;set; } = "";
	[Required]
	[StringLength(64, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ParameterSetHash { get;set; } = "";
	
	public string? ResultJson { get;set; }
	public int? RowCount { get;set; } = 0;
	public int? CacheSizeBytes { get;set; } = 0;
	public DateTime? CachedAt { get;set; } = DateTime.Now;
	public DateTime? ExpiresAt { get;set; } = DateTime.Now;
	public bool IsStale { get;set; } = false;
	
}
