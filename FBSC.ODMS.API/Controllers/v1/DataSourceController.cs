using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DataSourceController(IWebhookSecretService secretService) : BaseApiController<DataSourceController>
{
    [Authorize(Policy = Permission.DataSource.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DataSourceListDto>>> GetAsync([FromQuery] GetDataSourceQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataSource.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DataSourceState>> GetAsync(string id) =>
         await HandleRequest(new GetDataSourceByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DataSource.Create)]
    [HttpPost]
    public async Task<ActionResult<DataSourceState>> PostAsync([FromBody] DataSourceViewModel request) =>
        await HandleCommand<AddDataSourceCommand, DataSourceState>(Mapper.Map<AddDataSourceCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataSource.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DataSourceState>> PutAsync(string id, [FromBody] DataSourceViewModel request) =>
        await HandleCommand<EditDataSourceCommand, DataSourceState>(Mapper.Map<EditDataSourceCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DataSource.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DataSourceState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDataSourceCommand, DataSourceState>(new DeleteDataSourceCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DataSourceViewModel
{
    [Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get;set; } = "";
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string SystemType { get;set; } = "";
	[Required]
	[StringLength(30, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ConnectionMode { get;set; } = "";
	[StringLength(200, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ServerAddress { get;set; }
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DatabaseName { get;set; }
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? AuthenticationType { get;set; }
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Username { get;set; }
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? PasswordEncrypted { get;set; }
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ConnectionStringEncrypted { get;set; }
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get;set; }
	public bool IsActive { get;set; } = false;
	public DateTime? LastTestedAt { get;set; } = DateTime.Now;
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? LastTestStatus { get;set; }
	
}
