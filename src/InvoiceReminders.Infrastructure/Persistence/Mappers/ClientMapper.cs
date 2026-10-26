using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Infrastructure.Persistence.Documents;

namespace InvoiceReminders.Infrastructure.Persistence.Mappers;

internal static class ClientMapper
{
    public static Client ToDomain(ClientDocument document)
        => new(document.Id, document.Name, document.Email);
}