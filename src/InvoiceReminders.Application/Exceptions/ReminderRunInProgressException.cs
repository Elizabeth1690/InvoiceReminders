namespace InvoiceReminders.Application.Exceptions;

public sealed class ReminderRunInProgressException : Exception
{
    public ReminderRunInProgressException(string clientId)
        : base($"Ya hay un proceso de recordatorios en curso para el cliente '{clientId}'.")
    {
    }
}