namespace InvoiceReminders.Application.Reminders;

public sealed record EmailMessage(
    string To,
    string RecipientName,
    string Subject,
    string Body);