using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultColumn.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultColumn.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardQueryResultColumnController(IWebhookSecretService secretService) : BaseApiController<DashboardQueryResultColumnController>
{
    [Authorize(Policy = Permission.DashboardQueryResultColumn.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DashboardQueryResultColumnListDto>>> GetAsync([FromQuery] GetDashboardQueryResultColumnQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardQueryResultColumn.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardQueryResultColumnState>> GetAsync(string id) =>
         await HandleRequest(new GetDashboardQueryResultColumnByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DashboardQueryResultColumn.Create)]
    [HttpPost]
    public async Task<ActionResult<DashboardQueryResultColumnState>> PostAsync([FromBody] DashboardQueryResultColumnViewModel request) =>
        await HandleCommand<AddDashboardQueryResultColumnCommand, DashboardQueryResultColumnState>(Mapper.Map<AddDashboardQueryResultColumnCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardQueryResultColumn.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardQueryResultColumnState>> PutAsync(string id, [FromBody] DashboardQueryResultColumnViewModel request) =>
        await HandleCommand<EditDashboardQueryResultColumnCommand, DashboardQueryResultColumnState>(Mapper.Map<EditDashboardQueryResultColumnCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DashboardQueryResultColumn.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DashboardQueryResultColumnState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDashboardQueryResultColumnCommand, DashboardQueryResultColumnState>(new DeleteDashboardQueryResultColumnCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DashboardQueryResultColumnViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get;set; } = "";
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ColumnName { get;set; } = "";
	[Required]
	public int OrdinalPosition { get;set; } = 0;
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? SqlDataType { get;set; }
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string InferredRole { get;set; } = "";
	public bool IsAggregatable { get;set; } = false;
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DefaultAggregation { get;set; }
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? FormatString { get;set; }
	[Required]
	public int Sequence { get;set; } = 0;
	
}
