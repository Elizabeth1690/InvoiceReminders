namespace InvoiceReminders.Application.Exceptions;

public sealed class ClientNotFoundException : Exception
{
    public ClientNotFoundException(string clientId)
        : base($"No se encontró el cliente '{clientId}'.")
    {
    }
}