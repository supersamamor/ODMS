using Asp.Versioning;
using FBSC.Common.API.Controllers;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.API.Services;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Queries;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.API.Controllers.v1;

[ApiVersion("1.0")]
public class DashboardThemeController(IWebhookSecretService secretService) : BaseApiController<DashboardThemeController>
{
    [Authorize(Policy = Permission.DashboardTheme.View)]
    [HttpGet]
    public async Task<ActionResult<PagedListResponse<DashboardThemeListDto>>> GetAsync([FromQuery] GetDashboardThemeQuery query) => 
         await HandleRequest(query, await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardTheme.View)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardThemeState>> GetAsync(string id) =>
         await HandleRequest(new GetDashboardThemeByIdQuery(id), await secretService.GetClientSecretAsync(ClientId));

    [Authorize(Policy = Permission.DashboardTheme.Create)]
    [HttpPost]
    public async Task<ActionResult<DashboardThemeState>> PostAsync([FromBody] DashboardThemeViewModel request) =>
        await HandleCommand<AddDashboardThemeCommand, DashboardThemeState>(Mapper.Map<AddDashboardThemeCommand>(request), await secretService.GetClientSecretAsync(ClientId));
  
    [Authorize(Policy = Permission.DashboardTheme.Edit)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardThemeState>> PutAsync(string id, [FromBody] DashboardThemeViewModel request) =>
        await HandleCommand<EditDashboardThemeCommand, DashboardThemeState>(Mapper.Map<EditDashboardThemeCommand>(request) with { Id = id }, await secretService.GetClientSecretAsync(ClientId));   

    [Authorize(Policy = Permission.DashboardTheme.Delete)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DashboardThemeState>> DeleteAsync(string id) =>
        await HandleCommand<DeleteDashboardThemeCommand, DashboardThemeState>(new DeleteDashboardThemeCommand { Id = id }, await secretService.GetClientSecretAsync(ClientId));
}

public record DashboardThemeViewModel
{
    [Required]
	[StringLength(30, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Code { get;set; } = "";
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get;set; } = "";
	public bool IsDarkMode { get;set; } = false;
	[Required]
	[StringLength(10, ErrorMessage = "{0} length can't be more than {1}.")]
	public string PrimaryColorHex { get;set; } = "";
	
	public string? ColorPaletteJson { get;set; }
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? GenerationAlgorithm { get;set; }
	public bool IsSystemDefault { get;set; } = false;
	
}
