using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DataUploadBatchController(IWebhookSecretService secretService) : BaseApiController<DataUploadBatchController>
{
    [Authorize(Policy = Permission.DataUploadBatch.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DataUploadBatchListDto>>> GetAsync([FromQuery] GetDataUploadBatchQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataUploadBatch.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DataUploadBatchState>> GetAsync(string id) =>
         await HandleRequest(new GetDataUploadBatchByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DataUploadBatch.Create)]
    [HttpPost]
    public async Task<ActionResult<DataUploadBatchState>> PostAsync([FromBody] DataUploadBatchViewModel request) =>
        await HandleCommand<AddDataUploadBatchCommand, DataUploadBatchState>(Mapper.Map<AddDataUploadBatchCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataUploadBatch.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DataUploadBatchState>> PutAsync(string id, [FromBody] DataUploadBatchViewModel request) =>
        await HandleCommand<EditDataUploadBatchCommand, DataUploadBatchState>(Mapper.Map<EditDataUploadBatchCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DataUploadBatch.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DataUploadBatchState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDataUploadBatchCommand, DataUploadBatchState>(new DeleteDataUploadBatchCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DataUploadBatchViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataSourceId { get;set; } = "";
	[Required]
	[StringLength(260, ErrorMessage = "{0} length can't be more than {1}.")]
	public string FileName { get;set; } = "";
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string FileType { get;set; } = "";
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? UploadedBy { get;set; }
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? StagingTableName { get;set; }
	public int? RowCount { get;set; } = 0;
	public int? ColumnCount { get;set; } = 0;
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ImportStatus { get;set; } = "";
	public DateTime? ImportedAt { get;set; } = DateTime.Now;
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ErrorRemarks { get;set; }
	
}
