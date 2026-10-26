using InvoiceReminders.Domain.Clients;

namespace InvoiceReminders.Application.Abstractions;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(string clientId, CancellationToken ct);

    Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken ct);
}