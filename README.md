# ASP.NET Core Web App Solution Template
A multi-project Razor Pages solution template (Web + Web API) using modern .NET standards, clean architecture and example content. The solution includes Web, Web API, Core, Infrastructure and shared projects and demonstrates production-ready features.

## Features
### Common
- .NET 10, C# 14
- Nullable reference types
- Serilog with HTTP context enrichers and Seq sink
- Application monitoring via Application Insights
- Liveness/readiness health checks
- CQRS patterns (MediatR)
- FluentValidation
- Pagination (X.PagedList)
- Language-ext functional helpers (where applicable)
- OpenIddict for OIDC server and token validation
- Claims/role-based authorization
- Multi-tenant support
- Base domain types and immutable records
- Audit trail (database audit entries)
- Feature folders and helper utilities
- Report Setup and generation
- Approval workflow (modular/workflow approval, approval notifications)
- HTML template setup for email notifications and report generation
- Batch uploader and batch upload job management
- Dashboard and reporting widgets
- Datatables client-side integration

### Web (Razor Pages)
- AdminLTE-based UI with dark mode
- ASP.NET Core Identity and role/permission model
- Localization support
- Areas to organize functionality
- Datatables JS with server-side examples and custom DOM layouts
- HTML template management (for emails and report templates)
- Approval setup UI and approval notification flows
- Report Setup UI and report generation
- Batch upload jobs UI and scheduler integration
- Dashboard pages and widgets
- Configurable banner and environment-specific styling
- LibMan-managed client libraries

### Web API
- Swagger API documentation
- Base controllers and API conventions
- API versioning
- Standard error handling and thin controllers
- JWT and OIDC token support (claims-based authorization)
- API metadata endpoints

### Security
- Recommended security headers
- GDPR support and personal data marking
- Encrypted passwords in DB (where configured)
- Secure cookies and HTTPS enforcement
- Anti-forgery tokens on forms
- Login auditing (successful / failed attempts)
- Sensitive info suppression in logs

## Getting started

### Prerequisites
- .NET 10 SDK
- Visual Studio 2026 (or later) or a compatible IDE
- SQL Server (or configure in-memory DB for development)

### Configuration / Setup steps
1. Update the Web and Web API `appsettings.Development.json` values (Serilog, ApplicationInsights, SMTP, external auth, etc.) as needed.

2. Optional: generate and install a self-signed certificate for local HTTPS if required.

3. Optional: Set up external login providers (Google / Microsoft) per provider docs.

4. Create the Misc tools folder required for PDF generation:
   - The application reads `MiscToolsAndUtilitiesPath` from `appsettings.json` (default: `C:\MiscToolsAndUtilities`).
   - Create the folder on the C: drive (or the path you configured):
     - Example PowerShell:
       ```
       New-Item -Path 'C:\MiscToolsAndUtilities' -ItemType Directory -Force
       ```
   - Copy `Rotativa\wkhtmltopdf.exe` into that folder. This executable is required for PDF report generation using the HTML templates.
     - Example PowerShell (adjust source path to your repo/tools location):
       ```
       Copy-Item -Path '.\tools\Rotativa\wkhtmltopdf.exe' -Destination 'C:\MiscToolsAndUtilities\' -Force
       ```
   - Ensure the path in `appsettings.json` matches the created folder:
     ```
     "MiscToolsAndUtilitiesPath": "C:\\MiscToolsAndUtilities"
     ```
   - Ensure the application process / IIS App Pool identity has read & execute permissions for the folder and the `wkhtmltopdf.exe` file.

5. Open the solution in Visual Studio. After the packages are restored, update the configuration in appsettings.Development.json for both the Web API and Web projects.

    Web API:

    ```json
    {
      "Serilog": {
        "MinimumLevel": {
          "Default": "Information",
          "Override": {
            "Microsoft.AspNetCore": "Warning"
          }
        },
        "WriteTo": [
          { "Name": "Console" },
          {
            "Name": "Seq",
            "Args": {
              "serverUrl": "",
              "apiKey": ""
            }
          }
        ]
      },
      "ApplicationInsights": {
        "InstrumentationKey": ""
      },
      "UseInMemoryDatabase": false
    }
    ```

    Web:

    ```json
    {
      "DetailedErrors": true,
      "Serilog": {
        "MinimumLevel": {
          "Default": "Information",
          "Override": {
            "Microsoft.AspNetCore": "Warning"
          }
        },
        "WriteTo": [
          { "Name": "Console" },
          {
            "Name": "Seq",
            "Args": {
              "serverUrl": "",
              "apiKey": ""
            }
          }
        ]
      },
      "ApplicationInsights": {
        "InstrumentationKey": ""
      },
      "DefaultPassword": "",
      "DefaultClient": {
        "ClientId": "",
        "ClientSecret": ""
      },
      "SslThumbprint": "",
      "UseInMemoryDatabase": false,
      "NavbarColor": "orange",
      "SmtpSettings": {
        "Host": "",
        "Port": 587,
        "Email": "",
        "Password": ""
      },
      "Authentication": {
        "Microsoft": {
          "ClientId": "",
          "ClientSecret": ""
        },
        "Google": {
          "ClientId": "",
          "ClientSecret": ""
        }
      }
    }
    ```

6. Open the __Package Manager Console__ and apply the EF Core migrations:

    ```
    Add-Migration -Context ApplicationContext InitialDatabaseStructure
    Update-Database -Context IdentityContext
    Update-Database -Context ApplicationContext
    ```

7. Out of the box, the application assumes that the following URLs are configured
    - Web: https://localhost:5001
    - Web API: https://localhost:44379

    To configure Visual Studio to use the above URLs, edit __launchSettings.json__ for the Web API and Web projects.

    Web API
    ```
    {
      "profiles": {
        "MyApp.API": {
          "commandName": "Project",
          "launchBrowser": true,
          "launchUrl": "swagger",
          "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
          },
          "applicationUrl": "https://localhost:44379;http://localhost:44378"
        }
      }
    }
    ```

    Web
    ```
    {
      "profiles": {
        "MyApp.Web": {
          "commandName": "Project",
          "launchBrowser": true,
          "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
          },
          "applicationUrl": "https://localhost:5001;http://localhost:5000"
        }
      }
    }
    ```

    Or you can configure the automatically generated ports in the project `appsettings.json` files.

### Run
- Build and run the solution from Visual Studio (select the Web and Web API projects as required).
- Default admin credentials are seeded on first run; update `appsettings.Development.json` for the default password.

## Default web credentials

User: system@admin  
Password: &lt;set in appsettings.json&gt;

## Generating access tokens

The Web project implements an OpenID Connect server and token authentication using the OpenIddict library. It supports 
authorization code flow, device authorization flow, client credentials and password grant.

Download the Postman collection below for samples of how to generate access tokens using the supported flows. You can
use the generated access token to authenticate API requests.

## API
This is the Web API project. It is an ASP.NET Core application with an example Controller for a RESTful HTTP 
service. It uses claims-based authorization to secure the endpoints.

## Application
This project contains business logic codes that is meant to be shared, e.g. between Web and Web API projects.

## Core
This project contains the domain models. If using a functional programming approach, domain models should be immutable
records. Business logic codes should be placed in static classes. It is done this way to separate data and logic in
accordance with the functional programming approach.

## Infrastructure
This project contains code for communicating with external sources such as databases or external services.

## Web
This is the Web project. It implements Razor Pages, ASP.NET Core Identity and uses Areas to organize functionality. It
also implements an OpenID server and token authentication using the OpenIddict library. It uses claims-based authorization
to secure the pages.

## New/Notable pages and routes
- Report Setup: `/WebAppTemplate/ReportSetup` — configure and run saved reports
- Approval Setup / Approval module: `/WebAppTemplate/ApproverSetup` — manage approvers, sequences and email templates
- HTML Template management: `/HTMLTemplate/Setup` — manage templates used for emails and generated reports
- Batch Upload Jobs: `/Admin/BatchUploadJobs` — upload batches and view job history
- Dashboard pages under `/WebAppTemplate` area

## Developer notes
- The solution is now standardized on .NET 10 and C# 14. Review project TFMs if you fork/cloned older templates.
- Scheduler jobs (Quartz) are included for background tasks (batch processing and approval notification jobs).
- HTML template processing and email sending are extensible — template variables are resolved against loaded domain models at send/generate time.
- Approval workflows support multiple approval types (Any/All/In Sequence) and email notification flows with placeholders (e.g. `{ApproverName}`, `{ApprovalUrl}`) that are replaced at runtime.
- Audit trail captures important changes and user operations; ensure database retention policies match your compliance requirements.

## Useful links
- Update local URLs: edit the projects' __launchSettings.json__ profiles
- Run EF migrations: use the __Package Manager Console__ shown above
- Scheduler configuration: see the Scheduler project for deployed job definitions and `WebAppTemplate_jobs-qa.xml` copy settings

## Contributing / Extending
- Follow the solution's folder conventions (Areas for UI, Core for domain models, Infrastructure for data access).
- When adding new email/report templates, add them to the HTML Template management so they are available for both email and report generation.