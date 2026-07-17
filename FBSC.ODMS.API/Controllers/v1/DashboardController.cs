using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.Dashboard.Commands;
using FBSC.ODMS.Application.Features.ODMS.Dashboard.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardController(IWebhookSecretService secretService) : BaseApiController<DashboardController>
{
    [Authorize(Policy = Permission.Dashboard.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DashboardListDto>>> GetAsync([FromQuery] GetDashboardQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.Dashboard.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardState>> GetAsync(string id) =>
         await HandleRequest(new GetDashboardByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.Dashboard.Create)]
    [HttpPost]
    public async Task<ActionResult<DashboardState>> PostAsync([FromBody] DashboardViewModel request) =>
        await HandleCommand<AddDashboardCommand, DashboardState>(Mapper.Map<AddDashboardCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.Dashboard.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardState>> PutAsync(string id, [FromBody] DashboardViewModel request) =>
        await HandleCommand<EditDashboardCommand, DashboardState>(Mapper.Map<EditDashboardCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.Dashboard.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DashboardState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDashboardCommand, DashboardState>(new DeleteDashboardCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DashboardViewModel
{
    [Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Code { get;set; } = "";
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get;set; } = "";
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get;set; }
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Category { get;set; }
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DashboardThemeId { get;set; }
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string OwnerUserId { get;set; } = "";
	public bool IsPublic { get;set; } = false;
	public bool IsTemplate { get;set; } = false;
	public int? RefreshIntervalSeconds { get;set; } = 0;
	public DateTime? LastPublishedAt { get;set; } = DateTime.Now;
	public bool IsActive { get;set; } = false;
	
}
