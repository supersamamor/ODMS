using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace FBSC.Common.Web.Utility.Helpers;

// TODO: Return Validation instead of mutating ModelState

/// <summary>
/// Helper and extension methods for the <see cref="IFormFile"/> class.
/// </summary>
public class FileUploadHelper
{
    // If you require a check on specific characters in the IsValidFileExtensionAndSignature
    // method, supply the characters in the _allowedChars field.
    private static readonly byte[] _allowedChars = [];
    private static readonly System.Collections.Generic.HashSet<string> _blacklistedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        // Executables
        ".exe", ".com", ".scr", ".bat", ".cmd", ".pif", ".msi", ".msp",

        // Scripts
        ".js", ".jse", ".vbs", ".vbe", ".ws", ".wsf", ".wsc", ".wsh",
        ".ps1", ".ps1xml", ".ps2", ".ps2xml", ".psc1", ".psc2",

        // DLL/system
        ".dll", ".sys", ".drv", ".ocx", ".cpl", ".scf",

        // Archives that can deliver payloads
        ".cab",

        // Macro-enabled Office
        ".docm", ".dotm", ".xlsm", ".xltm", ".xlam", ".pptm", ".potm", ".ppam", ".ppsm", ".sldm",

        // Other risky
        ".hta", ".inf", ".ins", ".isp", ".jar", ".jnlp", ".msc", ".msh", ".msh1", ".msh2",
        ".mshxml", ".msh1xml", ".msh2xml", ".reg", ".application", ".gadget", ".app",

        // DB / executable-like / shortcuts
        ".ade", ".adp", ".bas", ".chm", ".crt", ".hlp", ".lnk", ".mad", ".maf",
        ".mag", ".mam", ".maq", ".mar", ".mas", ".mat", ".mau", ".mav", ".maw", ".mcf",
        ".mda", ".mdb", ".mde", ".mdt", ".mdw", ".mdz", ".ops", ".pcd", ".prg", ".url"
    };
    private static readonly Dictionary<string, List<byte[]>> _maliciousSignatures = new()
    {
        { "PE",  new List<byte[]> { "MZ"u8.ToArray(), "ZM"u8.ToArray() } }, // MZ/ZM
        { "VBS", new List<byte[]> { "wscript"u8.ToArray(), "createobject"u8.ToArray(), "shell.application"u8.ToArray() } },
        { "JS",  new List<byte[]> { "eval("u8.ToArray(), "activexobject"u8.ToArray(), "wscript.shell"u8.ToArray() } },
        { "PS",  new List<byte[]> { "powershell"u8.ToArray(), "invoke-expression"u8.ToArray(), "iex"u8.ToArray(), "downloadstring"u8.ToArray() } },
        { "BAT", new List<byte[]> { "@echo off"u8.ToArray(), "cmd.exe"u8.ToArray(), "powershell.exe"u8.ToArray() } },
    };

    private static bool HasDangerousFileName(string? fileName, string[]? permittedExtensions)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return true;

        var name = Path.GetFileName(fileName);
        // Reject Unicode Right-to-Left Override (RLO) or other bidi overrides
        if (name.Contains('\u202E') || name.Contains('\u202D') ||
            name.Contains('\u202A') || name.Contains('\u202B') ||
            name.Contains('\u202C')) return true;

        // Trailing dot/space is suspicious on Windows
        if (name.EndsWith('.') || name.EndsWith(' ')) return true;

        // Double-extension attack: reject if any inner extension is blacklisted
        var parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1)
        {
            // skip the base name (parts[0]); inspect all suffixes from index 1..n-1
            for (int i = 1; i < parts.Length - 0; i++)
            {
                var candidate = "." + parts[i].ToLowerInvariant();
                if (_blacklistedExtensions.Contains(candidate))
                    return true;
            }
        }

        // If a whitelist is provided, enforce last extension strictly
        var ext = Path.GetExtension(name);
        if (permittedExtensions != null && !permittedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static readonly System.Collections.Generic.HashSet<string> _textLikeExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt", ".csv", ".prn", ".log", ".json", ".xml"
    };
    // For more file signatures, see the File Signatures Database (https://www.filesignatures.net/)
    // and the official specifications for the file types you wish to add.    
    private static readonly Dictionary<string, (string[] MimeTypes, List<byte[]> Signatures)> _allowedFileTypesAndSignatures = new(StringComparer.OrdinalIgnoreCase)
    {
        // --- Images ---
        { ".gif", (
            new[] { "image/gif" },
            new List<byte[]> { "GIF8"u8.ToArray() }
        )},

        { ".png", (
            new[] { "image/png" },
            new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } }
        )},

        { ".jpeg", (
            new[] { "image/jpeg" },
            new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
            }
        )},

        { ".jpg", (
            new[] { "image/jpeg" },
            new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
            }
        )},

        // --- Documents ---
        { ".pdf", (
            new[] { "application/pdf" },
            new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } // %PDF
        )},

        { ".docx", (
            new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } // ZIP-based
        )},

        { ".xlsx", (
            new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } }
        )},

        { ".pptx", (
            new[] { "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } }
        )},

        // --- Archives ---
        { ".zip", (
            new[] { "application/zip", "application/x-zip-compressed" },
            new List<byte[]>
            {
                "WinZip"u8.ToArray(),
                "PKLITE"u8.ToArray(),
                "PKSpX"u8.ToArray(),
                new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                new byte[] { 0x50, 0x4B, 0x07, 0x08 },
            }
        )},

        // --- Text ---
        { ".txt", (
            new[] { "text/plain" },
            new List<byte[]>
            {
                Array.Empty<byte>(),                        // plain text (no BOM)
                new byte[] { 0xEF, 0xBB, 0xBF },           // UTF-8 BOM
                new byte[] { 0xFF, 0xFE },                 // UTF-16 LE BOM
                new byte[] { 0xFE, 0xFF },                 // UTF-16 BE BOM
            }
        )}
    };
    private static bool ContainsAnyMaliciousSignature(Stream data)
    {
        if (!data.CanSeek) return false;
        long originalPos = data.Position;
        try
        {
            data.Position = 0;
            // Limit to reasonable size to avoid huge allocations (e.g., first 8 MB)
            const int maxBytes = 8 * 1024 * 1024;
            using var ms = new MemoryStream();
            data.CopyTo(ms);
            var buffer = ms.ToArray();
            if (buffer.Length == 0) return false;

            var length = Math.Min(buffer.Length, maxBytes);
            var span = new Span<byte>(buffer, 0, length);

            // Lowercase ASCII inline (A-Z -> a-z)
            for (int i = 0; i < span.Length; i++)
            {
                byte b = span[i];
                if (b >= 0x41 && b <= 0x5A) span[i] = (byte)(b + 32);
            }

            foreach (var list in _maliciousSignatures.Values)
            {
                foreach (var pat in list)
                {
                    var patSpan = new ReadOnlySpan<byte>(pat);
                    // Ensure pattern is lowercased for ASCII comparison
                    byte[] lowerPat = new byte[pat.Length]; // Moved allocation out of the loop to avoid stack overflow
                    for (int i = 0; i < pat.Length; i++)
                    {
                        byte b = pat[i];
                        lowerPat[i] = (b >= 0x41 && b <= 0x5A) ? (byte)(b + 32) : b;
                    }
                    if (span.IndexOf(lowerPat) >= 0) return true;
                }
            }
            return false;
        }
        finally
        {
            data.Position = originalPos;
        }
    }
    private static bool ContainsOfficeMacros(Stream data)
    {
        if (!data.CanSeek) return false;
        long originalPos = data.Position;
        try
        {
            data.Position = 0;
            using var zip = new ZipArchive(data, ZipArchiveMode.Read, leaveOpen: true);
            // vbaProject.bin presence indicates macros
            return zip.Entries.Any(e => e.FullName.EndsWith("vbaProject.bin", StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            // Not a ZIP/OOXML; ignore
            return false;
        }
        finally
        {
            data.Position = originalPos;
        }
    }
    private static bool IsPandaDoc(ReadOnlySpan<byte> span)
    {
        // PandaDoc typically sets itself as the Producer or Creator in the PDF metadata
        static bool Contains(ReadOnlySpan<byte> hay, string needle)
            => hay.IndexOf(Encoding.ASCII.GetBytes(needle)) >= 0;

        return Contains(span, "/Producer (PandaDoc") ||
               Contains(span, "/Creator (PandaDoc") ||
               Contains(span, "PandaDoc.com");
    }

    private static bool PdfHasActiveContent(Stream data)
    {
        if (!data.CanSeek) return false;
        long originalPos = data.Position;
        try
        {
            data.Position = 0;
            using var ms = new MemoryStream();
            data.CopyTo(ms);
            var buf = ms.ToArray();
            if (buf.Length == 0) return false;

            int length = Math.Min(buf.Length, 4 * 1024 * 1024);
            var span = new ReadOnlySpan<byte>(buf, 0, length);

            // --- NEW EXCLUSION LOGIC ---
            if (IsPandaDoc(span)) return false;
            // ---------------------------

            static bool Contains(ReadOnlySpan<byte> hay, string needle)
                => hay.IndexOf(Encoding.ASCII.GetBytes(needle)) >= 0;

            return Contains(span, "/JavaScript") ||
                   Contains(span, "/AA") ||
                   Contains(span, "/Launch");
        }
        finally
        {
            data.Position = originalPos;
        }
    }
    // **WARNING!**
    // In the following file processing methods, the file`s content isn`t scanned.
    // In most production scenarios, an anti-virus/anti-malware scanner API is
    // used on the file before making the file available to users or other
    // systems. For more information, see the topic that accompanies this sample
    // app.

    /// <summary>
    /// Applies standard validations on the <see cref="IFormFile"/>
    /// then saves the file to the specified <paramref name="targetFilePath"/>.
    /// </summary>
    /// <typeparam name="T">Type of the model class containing the <see cref="IFormFile"/> property</typeparam>
    /// <param name="formFile"></param>
    /// <param name="permittedExtensions">List of permitted file extensions</param>
    /// <param name="sizeLimit">Max file size limit</param>
    /// <param name="targetFilePath">Path where file will be saved</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Filename used for storage</returns>
    public static async Task<Validation<Error, string>> ProcessFormFile<T>(IFormFile formFile,
        string[] permittedExtensions, long sizeLimit, string targetFilePath,
		bool validateActivePdf = true,
        CancellationToken cancellationToken = default) =>
        await ProcessFormFile<T, string>(formFile,
                                         permittedExtensions,
                                         sizeLimit,
                                         cancellationToken: cancellationToken,
                                         f: s =>
                                         {
                                             var trustedFileNameForFileStorage = Path.GetRandomFileName();
                                             var filePath = Path.Combine(targetFilePath, trustedFileNameForFileStorage);
                                             using var file = File.Create(filePath);
                                             s.WriteTo(file);
                                             return filePath;
                                         });

    /// <summary>
    /// Applies standard validations on the <see cref="IFormFile"/>
    /// then converts it to <see cref="Stream"/>.
    /// </summary>
    /// <typeparam name="T">Type of the model class containing the <see cref="IFormFile"/> property</typeparam>
    /// <typeparam name="TRet">Type that will be returned by the specified parameter <paramref name="f"/></typeparam>
    /// <param name="formFile"></param>
    /// <param name="permittedExtensions">List of permitted file extensions</param>
    /// <param name="sizeLimit">Max file size limit</param>
    /// <param name="f"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Validation<Error, TRet>> ProcessFormFile<T, TRet>(IFormFile formFile,
        string[]? permittedExtensions, long sizeLimit, Func<MemoryStream, TRet> f, bool validateActivePdf = true, CancellationToken cancellationToken = default)
    {
        if (!IsFileSafe(formFile))
        {
            return Error.New($"The file type isn`t safe : {formFile.ContentType}.");
        }
        var fieldDisplayName = string.Empty;

        // Use reflection to obtain the display name for the model
        // property associated with this IFormFile. If a display
        // name isn`t found, error messages simply won`t show
        // a display name.
        MemberInfo? property = typeof(T).GetProperty(formFile.Name[(formFile.Name.IndexOf('.') + 1)..]);

        if (property != null)
        {
            if (property.GetCustomAttribute<DisplayAttribute>() is DisplayAttribute displayAttribute)
            {
                fieldDisplayName = $"{displayAttribute.Name} ";
            }
        }

        // Don`t trust the file name sent by the client. To display
        // the file name, HTML-encode the value.
        var trustedFileNameForDisplay = WebUtility.HtmlEncode(formFile.FileName);

        // Check the file length. This check doesn`t catch files that only have 
        // a BOM as their content.
        if (formFile.Length == 0)
        {
            return Error.New($"{fieldDisplayName}({trustedFileNameForDisplay}) is empty.");
        }

        if (formFile.Length > sizeLimit)
        {
            var megabyteSizeLimit = sizeLimit / 1048576;
            return Error.New($"{fieldDisplayName}({trustedFileNameForDisplay}) exceeds {megabyteSizeLimit:N1} MB.");
        }

        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream, cancellationToken);

        // Check the content length in case the file`s only
        // content was a BOM and the content is actually
        // empty after removing the BOM.
        if (memoryStream.Length == 0)
        {
            return Error.New($"{fieldDisplayName}({trustedFileNameForDisplay}) is empty.");
        }

        if (!IsValidFileExtensionAndSignature(formFile.FileName, memoryStream, permittedExtensions, validateActivePdf))
        {
            return Error.New($"{fieldDisplayName}({trustedFileNameForDisplay}) file type isn`t permitted or the file`s signature doesn`t match the file`s extension.");
        }

        return f(memoryStream);
    }

    /// <summary>
    /// Apply standard validation on a file received from a multipart request.
    /// then converts it to <see cref="Stream"/>.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="contentDisposition"></param>
    /// <param name="permittedExtensions">List of permitted file extensions</param>
    /// <param name="sizeLimit">Max file size limit</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Validation<Error, Stream>> ProcessStreamedFile(MultipartSection section,
        ContentDispositionHeaderValue contentDisposition, string[] permittedExtensions, long sizeLimit,
		bool validateActivePdf = true,
        CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await section.Body.CopyToAsync(memoryStream, cancellationToken);

        // Check if the file is empty or exceeds the size limit.
        if (memoryStream.Length == 0)
        {
            return Error.New("The file is empty.");
        }

        if (memoryStream.Length > sizeLimit)
        {
            var megabyteSizeLimit = sizeLimit / 1048576;
            return Error.New($"The file exceeds {megabyteSizeLimit:N1} MB.");
        }

        if (!IsValidFileExtensionAndSignature(contentDisposition.FileName, memoryStream, permittedExtensions, validateActivePdf))
        {
            return Error.New("The file type isn`t permitted or the file`s signature doesn`t match the file`s extension.");
        }

        return memoryStream;
    }
    private static bool IsValidFileExtensionAndSignature(string? fileName, Stream? data, string[]? permittedExtensions, bool validateActivePdf)
	{
		if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
			return false;

		// Filename hardening & whitelist
		if (HasDangerousFileName(fileName, permittedExtensions))
			return false;

		var ext = Path.GetExtension(fileName).ToLowerInvariant();

		// Blocklisted extensions outright
		if (_blacklistedExtensions.Contains(ext))
			return false;

		data.Position = 0;
		using var reader = new BinaryReader(data, Encoding.UTF8, leaveOpen: true);

		// Plain-text types: ASCII enforcement + malicious keywords
		if (_textLikeExtensions.Contains(ext))
		{
			// Existing ASCII check (kept from your code)
			data.Position = 0;
			if (_allowedChars.Length == 0)
			{
				for (var i = 0; i < data.Length; i++)
					if (reader.ReadByte() > sbyte.MaxValue) return false;
			}
			else
			{
				data.Position = 0;
				for (var i = 0; i < data.Length; i++)
				{
					var b = reader.ReadByte();
					if (b > sbyte.MaxValue || !_allowedChars.Contains(b)) return false;
				}
			}

			// Malicious textual signatures (heuristic)
			data.Position = 0;
			if (ContainsAnyMaliciousSignature(data))
				return false;

			return true;
		}

		//Disable File Signature Validation For Now
		var fileSignatures = _allowedFileTypesAndSignatures
			.ToDictionary(
				kvp => kvp.Key,                // Keep the extension as key
				kvp => kvp.Value.Signatures,   // Keep only the List<byte[]> signatures
				StringComparer.OrdinalIgnoreCase
			);
		// Require known signature for non-text types      
		if (!fileSignatures.ContainsKey(ext))
			return false;

		//File signature check(exact match on header)
		data.Position = 0;
		var signatures = fileSignatures[ext];
		var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
		if (!signatures.Any(sig => headerBytes.AsSpan()[..sig.Length].SequenceEqual(sig)))
			return false;

		// Extra: OOXML macro detection (blocks renamed *.xlsm/*.docm/*.pptm)
		if (ext is ".xlsx" or ".docx" or ".pptx")
		{
			if (ContainsOfficeMacros(data))
				return false;
		}

		// Extra: PDF active content detection       
		if (validateActivePdf && ext is ".pdf")
		{
			if (PdfHasActiveContent(data))
				return false;
		}
		return true;
	}
   
    private static bool IsFileSafe(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!_allowedFileTypesAndSignatures.TryGetValue(ext, out (string[] MimeTypes, List<byte[]> Signatures) value))
            return false;

        var (allowedMimes, signatures) = value;

        // Check MIME type
        if (!allowedMimes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            return false;

        // Check signature
        using var reader = new BinaryReader(file.OpenReadStream());
        var fileHeader = reader.ReadBytes(8); // read first 8 bytes

        foreach (var sig in signatures)
        {
            if (sig.Length == 0 || fileHeader.Take(sig.Length).SequenceEqual(sig))
                return true;
        }

        return false;
    }
}