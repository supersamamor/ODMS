using AspNetCoreHero.ToastNotification.Extensions;
using FBSC.Common.Web.Utility.Logging;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Web;
using FBSC.ODMS.Web.Areas.Identity;
using FBSC.ODMS.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using FBSC.ODMS.EmailSending;
using FBSC.ODMS.ExcelProcessor;
using FBSC.Common.Services.Shared;
using Serilog;
using FBSC.ODMS.Scheduler;
using Microsoft.Extensions.FileProviders;
using FBSC.ApiHub;
using FBSC.ODMS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration).ReadFrom
                          .Services(services).Enrich
                          .FromLogContext());

// Add services to the container.
var configuration = builder.Configuration;
var services = builder.Services;

services.ConfigureIdentityServices(configuration);
services.ConfigureDefaultServices(configuration);

if (configuration.GetValue<bool>("UseInMemoryDatabase"))
{
    services.AddDbContext<ApplicationContext>(options =>
        options.UseInMemoryDatabase("ApplicationContext"));
}
else
{
    services.AddDbContext<ApplicationContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("ApplicationContext"),
                             o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));
}
services.AddHealthChecks()
        .AddDbContextCheck<ApplicationContext>()
        .AddDbContextCheck<IdentityContext>();
services.AddEmailSendingAService(configuration);
services.AddExcelProcessor();
services.AddSharedServices(configuration);
services.AddApiHubServices(configuration);
services.AddDashboardEngineServices();
services.AddScheduler(configuration);
services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
});
var app = builder.Build();
var secureUploadFilePath = configuration.GetValue<string>("UsersUpload:SecureUploadFilePath");
if (secureUploadFilePath != null)
{
    bool secureUploadFilePathExists = System.IO.Directory.Exists(secureUploadFilePath);
    if (!secureUploadFilePathExists)
        System.IO.Directory.CreateDirectory(secureUploadFilePath);
}
// Static Files
var publishedUploadFilePath = configuration.GetValue<string>("UsersUpload:PublishedUploadFilePath");
if (publishedUploadFilePath != null)
{
    bool publishedUploadFilePathExists = System.IO.Directory.Exists(publishedUploadFilePath);
    if (!publishedUploadFilePathExists)
        System.IO.Directory.CreateDirectory(publishedUploadFilePath);

    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            const int durationInSeconds = 60 * 60 * 24 * 365;
            ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                "public,max-age=" + durationInSeconds;
        },
        FileProvider = new PhysicalFileProvider(publishedUploadFilePath),
        RequestPath = "/" + WebConstants.PublishedUploadFilePath
    });
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
var baseUrl = configuration.GetValue<string>("BaseUrl");
app.UseHttpsRedirection();
app.UseSecurityHeaders(policies =>
{
    policies.AddDefaultSecurityHeaders();
    if (configuration.GetValue<bool>("IsTest"))
    {
        // Relaxed CSP for Test Environment (allows localhost scripts for debugging)        
        policies.AddContentSecurityPolicy(builder =>
        {
            builder.AddUpgradeInsecureRequests();
            builder.AddBlockAllMixedContent();

            builder.AddDefaultSrc().Self().OverHttps();

            builder.AddScriptSrc()
                .Self()
                .UnsafeInline()
                .UnsafeEval()
                .OverHttps()
                .From("https://localhost:*")
                .From("http://localhost:*");

            builder.AddStyleSrc()
                .Self()
                .UnsafeInline()
                .OverHttps();

            builder.AddImgSrc().OverHttps().Data();
            builder.AddObjectSrc().None();
            builder.AddBaseUri().None();
            builder.AddFrameAncestors().Self();

            builder.AddFormAction().Self().OverHttps();

            builder.AddConnectSrc()
                .Self()
                .OverHttps()
                .From("http://localhost:*")   // allow BrowserLink (HTTP SignalR)
                .From("https://localhost:*")  // allow hot reload
                .From("ws://localhost:*")     // allow WebSocket (non-secure)
                .From("wss://localhost:*");   // allow WebSocket Secure

            builder.AddFontSrc().From([
                "https://fonts.gstatic.com",
                "https://stackpath.bootstrapcdn.com"
            ]).OverHttps();
        });
    }
    else
    {
        // Strict CSP for Production
        policies.AddContentSecurityPolicy(builder =>
        {
            builder.AddUpgradeInsecureRequests();
            builder.AddBlockAllMixedContent();

            builder.AddDefaultSrc().None().OverHttps();

            builder.AddScriptSrc()
                .Self()
                .StrictDynamic()
                .UnsafeInline()
                .WithNonce()
                .OverHttps();

            builder.AddStyleSrc()
                .Self()
                // .StrictDynamic() <-- Remove this entirely
                .UnsafeInline() // Acts as a fallback for older browsers that don't support nonces
                .WithNonce()    // The modern standard for allowing inline styles
                .OverHttps();

            builder.AddImgSrc().OverHttps().Data();
            builder.AddObjectSrc().None();
            builder.AddBaseUri().None();
            builder.AddFrameAncestors().Self();
            builder.AddFormAction().Self().OverHttps();
            builder.AddConnectSrc().Self().OverHttps();
            builder.AddFontSrc().From([
                "https://fonts.gstatic.com",
                "https://stackpath.bootstrapcdn.com"
            ]).OverHttps();
			
			builder.AddFrameSrc()
                .Self()
                .OverHttps();
        });
    }
});
app.UseWebOptimizer();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 365;
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            "public,max-age=" + durationInSeconds;
    }
});
app.UseCookiePolicy();
app.UseSerilogRequestLogging();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.EnrichDiagnosticContext();
app.MapControllers();
app.MapRazorPages();
app.MapHealthChecks("/health").AllowAnonymous();
app.UseNotyf();
var miscToolsAndUtilitiesPath = configuration.GetValue<string>("MiscToolsAndUtilitiesPath");
FBSC.HTMLTemplate.Initializer.Main(miscToolsAndUtilitiesPath!);
// Seed the database
if (configuration.GetValue<bool>("EnableDatabaseSeed"))
{
	Log.Information("Seeding database");
	var scope = app.Services.CreateScope();
	await DefaultEntity.Seed(scope.ServiceProvider);
	await DefaultRole.Seed(scope.ServiceProvider);
	await DefaultUser.Seed(scope.ServiceProvider);
	await DefaultClient.Seed(scope.ServiceProvider);
	await UserRole.Seed(scope.ServiceProvider);
	await DefaultDashboard.Seed(scope.ServiceProvider);
	await DefaultApiHub.Seed(scope.ServiceProvider);
	await DefaultInsightForgeDashboard.Seed(scope.ServiceProvider);
	Log.Information("Finished seeding database");
}
app.Run();
