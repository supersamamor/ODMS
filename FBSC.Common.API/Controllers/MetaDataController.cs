using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FBSC.Common.API.Controllers;

/// <summary>
/// An API controller for displaying API info such as version, etc.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="MetaDataController"/>
/// </remarks>
/// <param name="configuration">Instance of <see cref="IConfiguration"/></param>
public class MetaDataController(IConfiguration configuration) : BaseApiController<MetaDataController>
{
    /// <summary>
    /// The GET handler.
    /// </summary>
    /// <returns><see cref="ActionResult"/>&lt;<see cref="MetaData"/>&gt;</returns>
    [HttpGet]
    public ActionResult<MetaData> GetAsync()
    {
        var version = new Version();
        configuration.GetSection("Version").Bind(version);
        return Ok(new MetaData { Version = version });
    }
}

/// <summary>
/// A class representing the API metadata.
/// </summary>
public record MetaData
{
    /// <summary>
    /// The version info of the API.
    /// </summary>
    public Version Version { get; init; } = new();
}

/// <summary>
/// A class representing the version of the API.
/// </summary>
public record Version
{
    /// <summary>
    /// The release name associated with this version of the API.
    /// </summary>
    public string ReleaseName { get; init; } = "";
    /// <summary>
    /// The build number associated with this version of the API.
    /// </summary>
    public string BuildNumber { get; init; } = "";
}
