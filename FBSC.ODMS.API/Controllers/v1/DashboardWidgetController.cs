using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DashboardWidget.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardWidget.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardWidgetController(IWebhookSecretService secretService) : BaseApiController<DashboardWidgetController>
{
    [Authorize(Policy = Permission.DashboardWidget.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DashboardWidgetListDto>>> GetAsync([FromQuery] GetDashboardWidgetQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardWidget.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardWidgetState>> GetAsync(string id) =>
         await HandleRequest(new GetDashboardWidgetByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DashboardWidget.Create)]
    [HttpPost]
    public async Task<ActionResult<DashboardWidgetState>> PostAsync([FromBody] DashboardWidgetViewModel request) =>
        await HandleCommand<AddDashboardWidgetCommand, DashboardWidgetState>(Mapper.Map<AddDashboardWidgetCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardWidget.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardWidgetState>> PutAsync(string id, [FromBody] DashboardWidgetViewModel request) =>
        await HandleCommand<EditDashboardWidgetCommand, DashboardWidgetState>(Mapper.Map<EditDashboardWidgetCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DashboardWidget.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DashboardWidgetState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDashboardWidgetCommand, DashboardWidgetState>(new DeleteDashboardWidgetCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DashboardWidgetViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardId { get;set; } = "";
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get;set; } = "";
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ReportTypeId { get;set; } = "";
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Title { get;set; } = "";
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? XAxisColumnName { get;set; }
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? YAxisColumnsJson { get;set; }
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? SeriesColumnName { get;set; }
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? AggregationOverride { get;set; }
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DrillDownDashboardId { get;set; }
	[Required]
	public int GridPositionX { get;set; } = 0;
	[Required]
	public int GridPositionY { get;set; } = 0;
	[Required]
	public int GridWidth { get;set; } = 0;
	[Required]
	public int GridHeight { get;set; } = 0;
	public int? RefreshIntervalOverrideSeconds { get;set; } = 0;
	public int? Sequence { get;set; } = 0;
	
}
