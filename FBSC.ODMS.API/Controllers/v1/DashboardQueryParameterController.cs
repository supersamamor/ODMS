using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryParameter.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryParameter.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardQueryParameterController(IWebhookSecretService secretService) : BaseApiController<DashboardQueryParameterController>
{
    [Authorize(Policy = Permission.DashboardQueryParameter.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DashboardQueryParameterListDto>>> GetAsync([FromQuery] GetDashboardQueryParameterQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardQueryParameter.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardQueryParameterState>> GetAsync(string id) =>
         await HandleRequest(new GetDashboardQueryParameterByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DashboardQueryParameter.Create)]
    [HttpPost]
    public async Task<ActionResult<DashboardQueryParameterState>> PostAsync([FromBody] DashboardQueryParameterViewModel request) =>
        await HandleCommand<AddDashboardQueryParameterCommand, DashboardQueryParameterState>(Mapper.Map<AddDashboardQueryParameterCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardQueryParameter.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardQueryParameterState>> PutAsync(string id, [FromBody] DashboardQueryParameterViewModel request) =>
        await HandleCommand<EditDashboardQueryParameterCommand, DashboardQueryParameterState>(Mapper.Map<EditDashboardQueryParameterCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DashboardQueryParameter.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DashboardQueryParameterState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDashboardQueryParameterCommand, DashboardQueryParameterState>(new DeleteDashboardQueryParameterCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DashboardQueryParameterViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get;set; } = "";
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ParameterName { get;set; } = "";
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataType { get;set; } = "";
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ControlType { get;set; } = "";
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DefaultValue { get;set; }
	public bool IsRequired { get;set; } = false;
	
	public string? LookupSourceQuery { get;set; }
	[Required]
	public int Sequence { get;set; } = 0;
	
}
