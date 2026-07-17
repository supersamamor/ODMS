using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Asp.Versioning.ApiExplorer;

namespace FBSC.Common.API.Swagger;

/// <summary>
/// A class for applying default configuration for generating Swagger documentation.
/// </summary>
public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration) : IConfigureOptions<SwaggerGenOptions>
{
    readonly IApiVersionDescriptionProvider _provider = provider;
    readonly string _appName = configuration.GetValue<string>("Application")!;

    /// <summary>
    /// Configures the Swagger documentation.
    /// </summary>
    /// <param name="options">Instance of <see cref="SwaggerGenOptions"/></param>
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo()
                {
                    Title = $"{_appName} {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                });
        }
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Input your access token here to access this API",
        });        
        options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer")
                {
                    Reference = new OpenApiReferenceWithDescription
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },                  
                },
                new List<string>()
            }
        });
    }
}
