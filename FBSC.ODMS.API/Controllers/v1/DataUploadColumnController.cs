using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DataUploadColumn.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUploadColumn.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DataUploadColumnController(IWebhookSecretService secretService) : BaseApiController<DataUploadColumnController>
{
    [Authorize(Policy = Permission.DataUploadColumn.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DataUploadColumnListDto>>> GetAsync([FromQuery] GetDataUploadColumnQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataUploadColumn.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DataUploadColumnState>> GetAsync(string id) =>
         await HandleRequest(new GetDataUploadColumnByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DataUploadColumn.Create)]
    [HttpPost]
    public async Task<ActionResult<DataUploadColumnState>> PostAsync([FromBody] DataUploadColumnViewModel request) =>
        await HandleCommand<AddDataUploadColumnCommand, DataUploadColumnState>(Mapper.Map<AddDataUploadColumnCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataUploadColumn.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DataUploadColumnState>> PutAsync(string id, [FromBody] DataUploadColumnViewModel request) =>
        await HandleCommand<EditDataUploadColumnCommand, DataUploadColumnState>(Mapper.Map<EditDataUploadColumnCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DataUploadColumn.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DataUploadColumnState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDataUploadColumnCommand, DataUploadColumnState>(new DeleteDataUploadColumnCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DataUploadColumnViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataUploadBatchId { get;set; } = "";
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ColumnName { get;set; } = "";
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DetectedDataType { get;set; }
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? MappedSqlDataType { get;set; }
	public int? OrdinalPosition { get;set; } = 0;
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? SampleValue { get;set; }
	
}
