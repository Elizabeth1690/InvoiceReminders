using InvoiceReminders.Application.Reminders;
using System.Net.Mail;

namespace InvoiceReminders.Application.Abstractions;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken ct);
}