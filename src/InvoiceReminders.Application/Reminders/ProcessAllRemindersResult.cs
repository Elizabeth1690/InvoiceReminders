namespace InvoiceReminders.Application.Reminders;

public sealed record ProcessAllRemindersResult(IReadOnlyList<ClientReminderRun> Clients)
{
    public int Processed => Clients.Sum(c => c.Result?.Processed ?? 0);
    public int Sent => Clients.Sum(c => c.Result?.Sent ?? 0);
    public int Failed => Clients.Sum(c => c.Result?.FailedInvoiceIds.Count ?? 0);
    public int Skipped => Clients.Sum(c => c.Result?.SkippedInvoiceIds.Count ?? 0);
    public int ClientsWithErrors => Clients.Count(c => c.Error is not null);
}