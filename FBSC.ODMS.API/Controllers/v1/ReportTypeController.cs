using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.ReportType.Commands;
using FBSC.ODMS.Application.Features.ODMS.ReportType.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class ReportTypeController(IWebhookSecretService secretService) : BaseApiController<ReportTypeController>
{
    [Authorize(Policy = Permission.ReportType.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<ReportTypeListDto>>> GetAsync([FromQuery] GetReportTypeQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.ReportType.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ReportTypeState>> GetAsync(string id) =>
         await HandleRequest(new GetReportTypeByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.ReportType.Create)]
    [HttpPost]
    public async Task<ActionResult<ReportTypeState>> PostAsync([FromBody] ReportTypeViewModel request) =>
        await HandleCommand<AddReportTypeCommand, ReportTypeState>(Mapper.Map<AddReportTypeCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.ReportType.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<ReportTypeState>> PutAsync(string id, [FromBody] ReportTypeViewModel request) =>
        await HandleCommand<EditReportTypeCommand, ReportTypeState>(Mapper.Map<EditReportTypeCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.ReportType.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ReportTypeState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteReportTypeCommand, ReportTypeState>(new DeleteReportTypeCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record ReportTypeViewModel
{
    [Required]
	[StringLength(30, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Code { get;set; } = "";
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get;set; } = "";
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ChartRenderer { get;set; } = "";
	public int? MinColumnsRequired { get;set; } = 0;
	public int? MaxColumnsRequired { get;set; } = 0;
	public bool RequiresXAxis { get;set; } = false;
	public bool RequiresYAxis { get;set; } = false;
	public bool RequiresSeriesGrouping { get;set; } = false;
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? IconClass { get;set; }
	public bool IsActive { get;set; } = false;
	
}
