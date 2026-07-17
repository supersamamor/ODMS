using FBSC.Common.Services.Shared.Interfaces;
using FBSC.Common.Services.Shared.Models.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FBSC.Common.Services.Shared.Services.SmtpMail;

/// <summary>
/// The email sending service.
/// </summary>
/// <remarks>
/// Creates an instance of <see cref="SmtpMailService"/>
/// with the specified options.
/// </remarks>
/// <param name="options"></param>
public class SmtpMailService(IOptions<SmtpSettings> options) : IMailService
{
    readonly SmtpSettings SmtpSettings = options.Value;

    /// <summary>
    /// Sends an email.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SendAsync(MailRequest request, CancellationToken cancellationToken = default)
    {
        var builder = new BodyBuilder
        {
            HtmlBody = request.Body
        };
        var email = new MimeMessage
        {
            Sender = MailboxAddress.Parse(SmtpSettings.Email),
            Subject = request.Subject,
            Body = builder.ToMessageBody()
        };
        email.To.Add(MailboxAddress.Parse(request.To));
        using var smtp = new SmtpClient();
        smtp.Connect(SmtpSettings.Host, SmtpSettings.Port, SecureSocketOptions.StartTls, cancellationToken);
        smtp.Authenticate(SmtpSettings.Email, SmtpSettings.Password, cancellationToken);
        await smtp.SendAsync(email, cancellationToken);
        smtp.Disconnect(true, cancellationToken);
    }
}