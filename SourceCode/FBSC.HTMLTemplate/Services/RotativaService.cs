using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using Microsoft.Extensions.Configuration;
using FBSC.HTMLTemplate.ViewModels;
using FBSC.Common.Utility.Helpers;
using FBSC.HTMLTemplate.Models;

namespace FBSC.HTMLTemplate.Services
{
    public class RotativaService(IConfiguration configuration)
	{   
		private readonly string _staticFolderPath = configuration.GetValue<string>("UsersUpload:SecureUploadFilePath")!;
		private Orientation _orientation = Orientation.Portrait;
		private Size _size = Size.A4;
		private int _marginTop = 15;
		private int _marginBottom = 15;
		private int _marginLeft = 10;
		private int _marginRight = 10;

		/// <summary>
		/// Generate PDF from HTML template with shortcodes and model data (case-insensitive)
		/// Supports nested sub-collections up to 2 levels with {#foreach CollectionName} ... {/foreach} syntax
		/// </summary>
		public async Task<RotativaDocumentModel> GeneratePDFFromTemplateAsync(string fileName, string staticFolder, string subFolderPath, HTMLTemplateViewModel documentForm,
			Microsoft.AspNetCore.Mvc.ActionContext pageContext,
			object model)
		{
			_orientation = Enum.Parse<Orientation>(documentForm.Orientation);
			_size = Enum.Parse<Size>(documentForm.PaperSize);
			_marginTop = documentForm.MarginTop;
			_marginBottom = documentForm.MarginBottom;
			_marginLeft = documentForm.MarginLeft;
			_marginRight = documentForm.MarginRight;
			// Process the template and replace shortcodes with model values (case-insensitive)
			string processedHtml = HTMLTemplateHelper.ProcessTemplate(documentForm.HTMLTemplate, model);
			string? processedFooterHtml = null;
			if (!string.IsNullOrEmpty(documentForm.HTMLFooterTemplate))
			{
				processedFooterHtml = HTMLTemplateHelper.ProcessTemplate(documentForm.HTMLFooterTemplate, model);
			}
			return await GeneratePDFFromHtmlAsync(fileName, staticFolder, subFolderPath, pageContext, processedHtml,  documentForm.CustomSwitch, processedFooterHtml, documentForm.CustomCss, documentForm.CssLibraries);
		}
		/// <summary>
		/// Generate PDF from HTML template with shortcodes and model data (case-insensitive)
		/// Supports nested sub-collections up to 2 levels with {#foreach CollectionName} ... {/foreach} syntax
		/// </summary>
		public async Task<RotativaDocumentModel> GeneratePDFFromTemplateAsync(string fileName, string staticFolder, string subFolderPath, HTMLTemplateState documentForm,
			Microsoft.AspNetCore.Mvc.ActionContext pageContext,
			object model)
		{
			_orientation = Enum.Parse<Orientation>(documentForm.Orientation);
			_size = Enum.Parse<Size>(documentForm.PaperSize);
			_marginTop = documentForm.MarginTop;
			_marginBottom = documentForm.MarginBottom;
			_marginLeft = documentForm.MarginLeft;
			_marginRight = documentForm.MarginRight;
			// Process the template and replace shortcodes with model values (case-insensitive)
			string processedHtml = HTMLTemplateHelper.ProcessTemplate(documentForm.HTMLTemplate, model);
			string? processedFooterHtml = null;
			if (!string.IsNullOrEmpty(documentForm.HTMLFooterTemplate))
			{
				processedFooterHtml = HTMLTemplateHelper.ProcessTemplate(documentForm.HTMLFooterTemplate, model);
			}
			return await GeneratePDFFromHtmlAsync(fileName, staticFolder, subFolderPath, pageContext, processedHtml, documentForm.CustomSwitch, processedFooterHtml, documentForm.CustomCss, documentForm.CssLibraries);
		}
		/// <summary>
		/// Generate PDF from HTML string using a wrapper view with optional CSS
		/// </summary>
		public async Task<RotativaDocumentModel> GeneratePDFFromHtmlAsync(string fileName, string staticFolder, string subFolderPath,
	 Microsoft.AspNetCore.Mvc.ActionContext pageContext,
	 string htmlContent,
	 string? customSwitch,
	 string? footerHtmlContent = null,
	 string? customCss = null,
	 string? cssLibraries = null)
		{
			FileHelper.CreateFolderIfNotExists(_staticFolderPath + "\\" + subFolderPath);
			var rotativaDocument = new RotativaDocumentModel(fileName, subFolderPath, staticFolder);
			var completeFilePathWithStaticFolder = _staticFolderPath + rotativaDocument.CompleteFilePath;
			if (File.Exists(completeFilePathWithStaticFolder))
			{
				File.Delete(completeFilePathWithStaticFolder);
			}

			// Create model that holds the HTML content and CSS
			var htmlModel = new HtmlContentModel
			{
				HtmlContent = htmlContent,
				CustomCss = customCss,
				CssLibraries = cssLibraries
			};

			string? footerFilePath = null;

			// If footer HTML content is provided, save it to a temporary file
			if (!string.IsNullOrEmpty(footerHtmlContent))
			{
				footerFilePath = Path.Combine(_staticFolderPath, subFolderPath, $"footer_{Guid.NewGuid()}.html");
				await File.WriteAllTextAsync(footerFilePath, footerHtmlContent);
			}

			try
			{
				var document = new ViewAsPdf("..\\Shared\\HtmlWrapper", htmlModel)
				{
					PageOrientation = _orientation,
					PageSize = _size,
					PageMargins = new Margins(_marginTop, _marginRight, _marginBottom, _marginLeft)
				};

				// Add footer if HTML content was provided
				customSwitch = customSwitch ?? "";
				if (!string.IsNullOrEmpty(footerFilePath))
				{
					document.CustomSwitches = (!string.IsNullOrEmpty(customSwitch) ? customSwitch + " " : "") + $"--footer-html \"{footerFilePath}\"";
				}

				var byteArray = await document.BuildFile(pageContext);
				await using var fileStream = new FileStream(completeFilePathWithStaticFolder, FileMode.Create, FileAccess.Write);
				await fileStream.WriteAsync(byteArray.AsMemory(0, byteArray.Length));

				return rotativaDocument;
			}
			finally
			{
				// Clean up temporary footer file
				if (!string.IsNullOrEmpty(footerFilePath) && File.Exists(footerFilePath))
				{
					try
					{
						File.Delete(footerFilePath);
					}
					catch
					{
						// Ignore cleanup errors
					}
				}
			}
		}
	}

	/// <summary>
	/// Model to hold HTML content and CSS for rendering
	/// </summary>
	public class HtmlContentModel
	{
		public string HtmlContent { get; set; } = "";
		public string? CustomCss { get; set; } = "";
		public string? CssLibraries { get; init; }
		public IList<string> CssLibraryList =>
			 CssLibraries?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
			 ?? [];
	}

	public class RotativaDocumentModel
	{
		public RotativaDocumentModel()
		{
		}

		public RotativaDocumentModel(string fileName, string subFolderPath, string staticFolder)
		{
			this.FileName = fileName;
			this.CompleteFilePath =  "\\" + subFolderPath + "\\" + this.FileName;
			this.FileUrl = "\\" + staticFolder + "\\" + subFolderPath + "\\" + this.FileName;
		}

		public string FileName { get; set; } = "";
		public string CompleteFilePath { get; private set; } = "";
		public string FileUrl { get; private set; } = "";
	}
}