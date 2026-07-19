using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DataUpload.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUpload.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DataUploadController(IWebhookSecretService secretService) : BaseApiController<DataUploadController>
{
    [Authorize(Policy = Permission.DataUpload.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DataUploadListDto>>> GetAsync([FromQuery] GetDataUploadQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataUpload.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DataUploadState>> GetAsync(string id) =>
         await HandleRequest(new GetDataUploadByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DataUpload.Create)]
    [HttpPost]
    public async Task<ActionResult<DataUploadState>> PostAsync([FromBody] DataUploadViewModel request) =>
        await HandleCommand<AddDataUploadCommand, DataUploadState>(Mapper.Map<AddDataUploadCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataUpload.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DataUploadState>> PutAsync(string id, [FromBody] DataUploadViewModel request) =>
        await HandleCommand<EditDataUploadCommand, DataUploadState>(Mapper.Map<EditDataUploadCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DataUpload.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DataUploadState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDataUploadCommand, DataUploadState>(new DeleteDataUploadCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DataUploadViewModel
{
    [StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get;set; }
	[Required]
	public string? FilePath { get;set; } = "";
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string FileType { get;set; } = "";
	
}
