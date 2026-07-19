using AutoMapper;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FBSC.Common.API.Controllers;


/// <summary>
/// A base class for an API controller. Defines default route and other common attributes.
/// Gets IMediator, IMapper, and ILogger from DI container.
/// </summary>
/// <typeparam name="T">Type of the controller extending this base class. Passed to ILogger.</typeparam>
[ApiConventionType(typeof(DefaultApiConventions))]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public abstract class BaseApiController<T> : ControllerBase
{
    private IMediator? _mediatorInstance;
    private IMapper? _mapperInstance;
    private ILogger<T>? _loggerInstance;
    private IConfiguration? _configuration;
    
    /// <summary>
    /// Instance of <see cref="IMediator"/>
    /// </summary>
    protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>()!;
    /// <summary>
    /// Instance of <see cref="IMapper"/>
    /// </summary>
    protected IMapper Mapper => _mapperInstance ??= HttpContext.RequestServices.GetService<IMapper>()!;
    /// <summary>
    /// Instance of <see cref="ILogger"/>
    /// </summary>
    protected ILogger<T> Logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>()!;
    protected IConfiguration Configuration => _configuration ??= HttpContext.RequestServices.GetService<IConfiguration>()!;
    // Define this at the class level so it is only allocated once in memory
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Gets the Client ID for the current request. 
    /// Automatically checks JWT claims first, then falls back to the X-Client-Id header for webhooks.
    /// </summary>
    protected string? ClientId
    {
        get
        {
            // 1. Try OAuth2/OIDC Claims (Standard Bearer Token)
            var clientId = User?.FindFirst(OpenIddictConstants.Claims.ClientId)?.Value
                        ?? User?.FindFirst("azp")?.Value;

            // 2. Fallback to Webhook Custom Header (HMAC Token-less requests)
            if (string.IsNullOrWhiteSpace(clientId) && HttpContext != null)
            {
                if (Request.Headers.TryGetValue("X-Client-Id", out var headerValue))
                {
                    clientId = headerValue.ToString();
                }
            }
            return clientId;
        }
    }
    /// <summary>
    /// Converts output of a function returning an <see cref="Option{A}"/> to an <see cref="ActionResult"/>&lt;<typeparamref name="A"/>&gt;.
    /// Returns <see cref="NotFoundResult"/> if Option is None.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <param name="option">A function returning an <see cref="Option{A}"/></param>
    /// <returns><see cref="ActionResult"/>&lt;<typeparamref name="A"/>&gt;</returns>
    protected ActionResult<A> ToActionResult<A>(Func<Option<A>> option) =>
        option().Match<ActionResult<A>>(some => some, () => NotFound());

    /// <summary>
    /// Converts output of an async function returning an <see cref="Option{A}"/> to an <see cref="ActionResult"/>&lt;<typeparamref name="A"/>&gt;.
    /// Returns <see cref="NotFoundResult"/> if Option is None.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <param name="option">An async function returning an <see cref="Option{A}"/></param>
    /// <returns><see cref="ActionResult"/>&lt;<typeparamref name="A"/>&gt;</returns>
    protected async Task<ActionResult<A>> ToActionResult<A>(Func<Task<Option<A>>> option) =>
        await option().Map(x => ToActionResult(() => x));

    /// <summary>
    /// Converts output of a function returning a <see cref="Validation"/>&lt;<see cref="Error"/>, <typeparamref name="A"/>&gt; 
    /// to an ActionResult&lt;<typeparamref name="A"/>&gt;. If status of Validation is FAIL, returns a
    /// <see cref="BadRequestObjectResult"/>. Validation errors are placed in <see cref="ValidationProblemDetails"/> object.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <param name="validation">A function returning a <see cref="Validation"/>&lt;<see cref="Error"/>, <typeparamref name="A"/>&gt;</param>
    /// <returns><see cref="ActionResult"/>&lt;<typeparamref name="A"/>&gt;</returns>
    protected ActionResult<A> ToActionResult<A>(Func<Validation<Error, A>> validation) =>
        validation().Match<ActionResult<A>>(
            succ => succ,
            errors => BadRequest(GetProblemDetails(errors)));

    /// <summary>
    /// Converts output of an async function returning a <see cref="Validation"/>&lt;<see cref="Error"/>, <typeparamref name="A"/>&gt; 
    /// to an ActionResult&lt;<typeparamref name="A"/>&gt;. If status of Validation is FAIL, returns a
    /// <see cref="BadRequestObjectResult"/>. Validation errors are placed in <see cref="ValidationProblemDetails"/> object.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <param name="validation">An async function returning a <see cref="Validation"/>&lt;<see cref="Error"/>, <typeparamref name="A"/>&gt;</param>
    /// <returns><see cref="ActionResult"/>&lt;<typeparamref name="A"/>&gt;</returns>
    protected async Task<ActionResult<A>> ToActionResult<A>(Func<Task<Validation<Error, A>>> validation) =>
        await validation().Map(x => ToActionResult(() => x));

    /// <summary>
    /// Handles the webhook request, validates HMAC, and routes through MediatR with functional validation.
    /// </summary>
    public async Task<ActionResult<TResponse>> HandleCommand<TRequest, TResponse>(TRequest request, string? secretKey)
        where TRequest : IRequest<Validation<Error, TResponse>>
    {
        try
        {
            var securityError = await ValidateHmacSignature(request, secretKey);
            if (securityError != null) return securityError;

            // 2. Functional Execution and Mapping
            // This safely unwraps the Validation<Error, TResponse> monad and returns either the Success state or ProblemDetails.
            return await ToActionResult(async () => await Mediator.Send(request));
        }
        catch (Exception ex)
        {
            // Catch unhandled exceptions (like DB timeouts) and format them to match the ProblemDetails schema
            Logger.LogError(ex, "Unexpected error processing webhook at {Path}", Request.Path);

            return BadRequest(new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = HttpContext.Request.Path
            });
        }
    }
    public async Task<ActionResult> HandleRequest<TRequest>(TRequest request, string? secretKey)
    {
        try
        {
            var securityError = await ValidateHmacSignature(request, secretKey);
            if (securityError != null) return securityError;
            return Ok(await Mediator.Send(request!));
        }
        catch (Exception ex)
        {
            // Catch unhandled exceptions (like DB timeouts) and format them to match the ProblemDetails schema
            Logger.LogError(ex, "Unexpected error processing webhook at {Path}", Request.Path);

            return BadRequest(new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = HttpContext.Request.Path
            });
        }
    }
    private async Task<UnauthorizedObjectResult?> ValidateHmacSignature<TRequest>(TRequest request, string? secretKey)
    {
        if (string.IsNullOrWhiteSpace(secretKey)) return null;

        if (!Request.Headers.TryGetValue("X-HMAC-Signature", out var hmacSignature))
        {
            return Unauthorized("Missing X-HMAC-Signature header.");
        }

        var sb = new StringBuilder();
        sb.Append(Request.Method);
        sb.Append('\n');
        sb.Append(Request.Path);
        sb.Append('\n');
        sb.Append(JsonSerializer.Serialize(request, s_jsonOptions));

        var message = sb.ToString();
        var expectedSignature = GenerateHmacSignature(message, secretKey);

        if (hmacSignature != expectedSignature)
        {
            Logger.LogWarning("Invalid HMAC signature detected for {Path}", Request.Path);
            return Unauthorized("Invalid HMAC signature.");
        }

        return null; // Null means validation passed
    }
    private ValidationProblemDetails GetProblemDetails(Seq<Error> errors)
    {
        errors.Iter(error => ModelState.AddModelError("error", error.ToString()));
        var problemDetails = new ValidationProblemDetails(ModelState)
        {
            Detail = "One or more validation errors occured.",
            Instance = HttpContext.Request.Path
        };
        var traceId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
        if (traceId != null)
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        return problemDetails;
    }
    private static string GenerateHmacSignature(string message, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        using var hmac = new HMACSHA256(keyBytes);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var hash = hmac.ComputeHash(messageBytes);
        return Convert.ToBase64String(hash);
    }
}
