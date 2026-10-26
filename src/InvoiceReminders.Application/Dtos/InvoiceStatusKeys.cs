using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Application.Dtos;

internal static class InvoiceStatusKeys
{
    public static string ToKey(InvoiceStatus status) => status switch
    {
        InvoiceStatus.PrimerRecordatorio => "primerrecordatorio",
        InvoiceStatus.SegundoRecordatorio => "segundorecordatorio",
        InvoiceStatus.Desactivado => "desactivado",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Estado desconocido.")
    };
}