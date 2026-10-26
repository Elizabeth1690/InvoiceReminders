using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Application.Dtos;
using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Application.Queries;

public sealed class GetInvoicesSummaryUseCase
{
    private readonly IClientRepository _clients;
    private readonly IInvoiceRepository _invoices;

    public GetInvoicesSummaryUseCase(IClientRepository clients, IInvoiceRepository invoices)
    {
        _clients = clients;
        _invoices = invoices;
    }

    public async Task<InvoicesSummaryDto> ExecuteAsync(string? clientId, CancellationToken ct)
    {
        var clients = await _clients.GetAllAsync(ct);
        var invoices = await _invoices.GetAllAsync(clientId, ct);

        var clientNames = clients.ToDictionary(c => c.Id, c => c.Name);

        var items = invoices
            .Select(i => new InvoiceDto(
                i.Id,
                i.ClientId,
                clientNames.GetValueOrDefault(i.ClientId, "Desconocido"),
                i.Number,
                i.Amount,
                i.DueDate,
                InvoiceStatusKeys.ToKey(i.Status)))
            .ToList();

        return new InvoicesSummaryDto(
            items.Count,
            invoices.Count(i => i.Status == InvoiceStatus.PrimerRecordatorio),
            invoices.Count(i => i.Status == InvoiceStatus.SegundoRecordatorio),
            invoices.Count(i => i.Status == InvoiceStatus.Desactivado),
            items);
    }
}