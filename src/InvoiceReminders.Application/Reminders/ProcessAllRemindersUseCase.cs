using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Application.Exceptions;
using InvoiceReminders.Domain.Clients;
using Microsoft.Extensions.Logging;

namespace InvoiceReminders.Application.Reminders;

public sealed class ProcessAllRemindersUseCase
{
    private readonly IClientRepository _clients;
    private readonly ProcessInvoiceRemindersUseCase _processClient;
    private readonly ILogger<ProcessAllRemindersUseCase> _logger;

    public ProcessAllRemindersUseCase(
        IClientRepository clients,
        ProcessInvoiceRemindersUseCase processClient,
        ILogger<ProcessAllRemindersUseCase> logger)
    {
        _clients = clients;
        _processClient = processClient;
        _logger = logger;
    }

    public async Task<ProcessAllRemindersResult> ExecuteAsync(CancellationToken ct)
    {
        var clients = await _clients.GetAllAsync(ct);
        var runs = new List<ClientReminderRun>(clients.Count);

        foreach (var client in clients)
        {
            runs.Add(await RunForClientAsync(client, ct));
        }

        return new ProcessAllRemindersResult(runs);
    }

    private async Task<ClientReminderRun> RunForClientAsync(Client client, CancellationToken ct)
    {
        try
        {
            var result = await _processClient.ExecuteAsync(client, ct);
            return new ClientReminderRun(client.Id, client.Name, result, null);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Falló el procesamiento del cliente {ClientId}", client.Id);

            var error = ex is ReminderRunInProgressException
                ? ex.Message
                : "Error inesperado procesando el cliente.";

            return new ClientReminderRun(client.Id, client.Name, null, error);
        }
    }
}