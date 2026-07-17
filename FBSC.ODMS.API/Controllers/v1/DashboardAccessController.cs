using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DashboardAccess.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardAccess.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardAccessController(IWebhookSecretService secretService) : BaseApiController<DashboardAccessController>
{
    [Authorize(Policy = Permission.DashboardAccess.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DashboardAccessListDto>>> GetAsync([FromQuery] GetDashboardAccessQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardAccess.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardAccessState>> GetAsync(string id) =>
         await HandleRequest(new GetDashboardAccessByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DashboardAccess.Create)]
    [HttpPost]
    public async Task<ActionResult<DashboardAccessState>> PostAsync([FromBody] DashboardAccessViewModel request) =>
        await HandleCommand<AddDashboardAccessCommand, DashboardAccessState>(Mapper.Map<AddDashboardAccessCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardAccess.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardAccessState>> PutAsync(string id, [FromBody] DashboardAccessViewModel request) =>
        await HandleCommand<EditDashboardAccessCommand, DashboardAccessState>(Mapper.Map<EditDashboardAccessCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DashboardAccess.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DashboardAccessState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDashboardAccessCommand, DashboardAccessState>(new DeleteDashboardAccessCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DashboardAccessViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardId { get;set; } = "";
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string GranteeType { get;set; } = "";
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string GranteeId { get;set; } = "";
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string AccessLevel { get;set; } = "";
	public DateTime? GrantedAt { get;set; } = DateTime.Now;
	
}
