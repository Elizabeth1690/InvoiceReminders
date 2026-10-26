using InvoiceReminders.Domain.Invoices;
using InvoiceReminders.Infrastructure.Persistence.Documents;

namespace InvoiceReminders.Infrastructure.Persistence.Mappers;

internal static class InvoiceMapper
{
    /// <returns>La factura de dominio, o null si el documento tiene un estado desconocido.</returns>
    public static Invoice? ToDomainOrNull(InvoiceDocument document)
    {
        if (!InvoiceStatusMapper.TryFromDocumentValue(document.Status, out var status))
        {
            return null;
        }

        return new Invoice(
            document.Id,
            document.ClientId,
            document.Number,
            document.Amount,
            DateOnly.FromDateTime(document.DueDate),
            status);
    }
}