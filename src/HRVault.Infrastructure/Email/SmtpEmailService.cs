using System.Net;
using System.Net.Mail;
using HRVault.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace HRVault.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpOptions _options;

    public SmtpEmailService(
        IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(
                _options.FromEmail,
                _options.FromName),

            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(to);

        using var client = new SmtpClient(
            _options.Host,
            _options.Port)
        {
            EnableSsl = _options.UseSsl
        };

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            client.Credentials = new NetworkCredential(
                _options.Username,
                _options.Password);
        }

        cancellationToken.ThrowIfCancellationRequested();

        await client.SendMailAsync(
            message,
            cancellationToken);
    }
}