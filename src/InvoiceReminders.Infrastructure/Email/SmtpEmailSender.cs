
using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Application.Reminders;
using InvoiceReminders.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace InvoiceReminders.Infrastructure.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(
            _settings.FromName,
            _settings.FromAddress));

        email.To.Add(new MailboxAddress(
    message.RecipientName,
    message.To));

        email.Subject = message.Subject;
        email.Body = new TextPart("plain")
        {
            Text = message.Body
        };

        using var smtpClient = new SmtpClient();

        var secureSocketOptions = _settings.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.None;

        await smtpClient.ConnectAsync(
            _settings.Host,
            _settings.Port,
            secureSocketOptions,
            ct);

        if (!string.IsNullOrWhiteSpace(_settings.User))
        {
            await smtpClient.AuthenticateAsync(
                _settings.User,
                _settings.Password,
                ct);
        }

        await smtpClient.SendAsync(email, ct);
        await smtpClient.DisconnectAsync(true, ct);
    }
}
