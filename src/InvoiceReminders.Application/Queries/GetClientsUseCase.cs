using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Application.Dtos;

namespace InvoiceReminders.Application.Queries;

public sealed class GetClientsUseCase
{
    private readonly IClientRepository _clients;

    public GetClientsUseCase(IClientRepository clients)
    {
        _clients = clients;
    }

    public async Task<IReadOnlyList<ClientDto>> ExecuteAsync(CancellationToken ct)
    {
        var clients = await _clients.GetAllAsync(ct);

        return clients
            .Select(client => new ClientDto(
                client.Id,
                client.Name,
                client.Email))
            .ToList();
    }
}