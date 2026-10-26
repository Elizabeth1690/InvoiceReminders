namespace InvoiceReminders.Application.Dtos;

public sealed record InvoicesSummaryDto(
    int Total,
    int PrimerRecordatorio,
    int SegundoRecordatorio,
    int Desactivado,
    IReadOnlyList<InvoiceDto> Invoices);