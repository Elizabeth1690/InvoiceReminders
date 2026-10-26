
using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Application.Exceptions;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Domain.Invoices;
using Microsoft.Extensions.Logging;

namespace InvoiceReminders.Application.Reminders;

public sealed class ProcessInvoiceRemindersUseCase
{
    public const int BatchSize = 100;

    private readonly IClientRepository _clients;
    private readonly IInvoiceRepository _invoices;
    private readonly IEmailSender _emailSender;
    private readonly IReminderMessageFactory _messages;
    private readonly ClientRunGate _gate;
    private readonly ILogger<ProcessInvoiceRemindersUseCase> _logger;

    public ProcessInvoiceRemindersUseCase(
        IClientRepository clients,
        IInvoiceRepository invoices,
        IEmailSender emailSender,
        IReminderMessageFactory messages,
        ClientRunGate gate,
        ILogger<ProcessInvoiceRemindersUseCase> logger)
    {
        _clients = clients;
        _invoices = invoices;
        _emailSender = emailSender;
        _messages = messages;
        _gate = gate;
        _logger = logger;
    }

    public async Task<ReminderRunResult> ExecuteAsync(
        string clientId,
        CancellationToken ct)
    {
        var client = await _clients.GetByIdAsync(clientId, ct)
            ?? throw new ClientNotFoundException(clientId);

        return await ExecuteAsync(client, ct);
    }

    public async Task<ReminderRunResult> ExecuteAsync(
        Client client,
        CancellationToken ct)
    {
        using var lease = _gate.TryEnter(client.Id)
            ?? throw new ReminderRunInProgressException(client.Id);

        var processed = 0;
        var sent = 0;

        var failed = new List<string>();
        var skipped = new List<string>();

        string? cursor = null;

        while (true)
        {
            var batch = await _invoices.GetPendingRemindersByClientAsync(
                client.Id,
                cursor,
                BatchSize,
                ct);

            if (batch.Count == 0)
            {
                break;
            }

            foreach (var invoice in batch)
            {
                processed++;

                switch (await ProcessInvoiceAsync(client, invoice, ct))
                {
                    case InvoiceOutcome.Sent:
                        sent++;
                        break;

                    case InvoiceOutcome.Failed:
                        failed.Add(invoice.Id);
                        break;

                    case InvoiceOutcome.Skipped:
                        skipped.Add(invoice.Id);
                        break;
                }
            }

            cursor = batch[^1].Id;

            if (batch.Count < BatchSize)
            {
                break;
            }
        }

        return new ReminderRunResult(
            processed,
            sent,
            failed,
            skipped);
    }

    private async Task<InvoiceOutcome> ProcessInvoiceAsync(
        Client client,
        Invoice invoice,
        CancellationToken ct)
    {
        try
        {
            var currentStatus = invoice.Status;

            // Usar la regla del dominio evita duplicar
            // las transiciones de estado en esta capa.
            var nextInvoice = new Invoice(
                invoice.Id,
                invoice.ClientId,
                invoice.Number,
                invoice.Amount,
                invoice.DueDate,
                currentStatus);

            try
            {
                nextInvoice.AdvanceReminder();
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning(
                    "La factura {InvoiceId} tiene un estado no procesable: {Status}",
                    invoice.Id,
                    currentStatus);

                return InvoiceOutcome.Skipped;
            }

            var nextStatus = nextInvoice.Status;

            // Crear el mensaje con la factura en su estado original.
            var message = _messages.Create(client, invoice);

            // No intentar actualizar el estado si falla el correo.
            await _emailSender.SendAsync(message, ct);

            // Actualización condicional para evitar sobrescribir
            // un estado que haya cambiado desde la lectura.
            var persisted = await _invoices.TryUpdateStatusAsync(
                invoice.Id,
                currentStatus,
                nextStatus,
                ct);

            if (persisted)
            {
                return InvoiceOutcome.Sent;
            }

            _logger.LogWarning(
                "La factura {InvoiceId} cambió de estado durante el procesamiento. " +
                "El estado no fue actualizado por este proceso.",
                invoice.Id);

            return InvoiceOutcome.Skipped;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(
                ex,
                "Falló el procesamiento de la factura {InvoiceId}",
                invoice.Id);

            return InvoiceOutcome.Failed;
        }
    }

    private enum InvoiceOutcome
    {
        Sent,
        Failed,
        Skipped
    }
}
