namespace InvoiceReminders.Application.Reminders;

public sealed record ClientReminderRun(
    string ClientId,
    string ClientName,
    ReminderRunResult? Result,
    string? Error);