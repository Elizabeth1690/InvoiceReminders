using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Application.Abstractions;

public interface IInvoiceRepository
{
    /// <summary>
    /// Devuelve un lote de facturas pendientes de recordatorio,
    /// ordenadas por Id, a partir del cursor indicado.
    /// </summary>
    Task<IReadOnlyList<Invoice>> GetPendingRemindersByClientAsync(
        string clientId,
        string? afterId,
        int batchSize,
        CancellationToken ct);

    Task<IReadOnlyList<Invoice>> GetAllAsync(
        string? clientId,
        CancellationToken ct);

    /// <summary>
    /// Actualiza el estado únicamente si la factura mantiene
    /// el estado esperado en la base de datos.
    ///
    /// Devuelve false si otro proceso ya modificó la factura.
    /// </summary>
    Task<bool> TryUpdateStatusAsync(
        string invoiceId,
        InvoiceStatus expectedCurrentStatus,
        InvoiceStatus newStatus,
        CancellationToken ct);
}