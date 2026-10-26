using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Infrastructure.Persistence.Mappers;

internal static class InvoiceStatusMapper
{
    public static string ToDocumentValue(InvoiceStatus status) => status switch
    {
        InvoiceStatus.PrimerRecordatorio => "primerrecordatorio",
        InvoiceStatus.SegundoRecordatorio => "segundorecordatorio",
        InvoiceStatus.Desactivado => "desactivado",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Estado desconocido.")
    };

    public static bool TryFromDocumentValue(string? value, out InvoiceStatus status)
    {
        switch (value?.Trim().ToLowerInvariant())
        {
            case "primerrecordatorio":
                status = InvoiceStatus.PrimerRecordatorio;
                return true;
            case "segundorecordatorio":
                status = InvoiceStatus.SegundoRecordatorio;
                return true;
            case "desactivado":
                status = InvoiceStatus.Desactivado;
                return true;
            default:
                status = default;
                return false;
        }
    }
}