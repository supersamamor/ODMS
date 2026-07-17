using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DataSourceSchemaCacheController(IWebhookSecretService secretService) : BaseApiController<DataSourceSchemaCacheController>
{
    [Authorize(Policy = Permission.DataSourceSchemaCache.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DataSourceSchemaCacheListDto>>> GetAsync([FromQuery] GetDataSourceSchemaCacheQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataSourceSchemaCache.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DataSourceSchemaCacheState>> GetAsync(string id) =>
         await HandleRequest(new GetDataSourceSchemaCacheByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DataSourceSchemaCache.Create)]
    [HttpPost]
    public async Task<ActionResult<DataSourceSchemaCacheState>> PostAsync([FromBody] DataSourceSchemaCacheViewModel request) =>
        await HandleCommand<AddDataSourceSchemaCacheCommand, DataSourceSchemaCacheState>(Mapper.Map<AddDataSourceSchemaCacheCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DataSourceSchemaCache.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DataSourceSchemaCacheState>> PutAsync(string id, [FromBody] DataSourceSchemaCacheViewModel request) =>
        await HandleCommand<EditDataSourceSchemaCacheCommand, DataSourceSchemaCacheState>(Mapper.Map<EditDataSourceSchemaCacheCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DataSourceSchemaCache.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DataSourceSchemaCacheState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDataSourceSchemaCacheCommand, DataSourceSchemaCacheState>(new DeleteDataSourceSchemaCacheCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DataSourceSchemaCacheViewModel
{
    [Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataSourceId { get;set; } = "";
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string SchemaName { get;set; } = "";
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string TableName { get;set; } = "";
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ColumnName { get;set; } = "";
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? SqlDataType { get;set; }
	public int? OrdinalPosition { get;set; } = 0;
	public bool IsNullable { get;set; } = false;
	public DateTime? RefreshedAt { get;set; } = DateTime.Now;
	
}
