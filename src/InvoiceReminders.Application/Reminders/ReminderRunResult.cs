namespace InvoiceReminders.Application.Reminders;

/// <param name="Processed">Cantidad de facturas pendientes cuyo procesamiento se intentó.</param>
/// <param name="Sent">Cantidad de correos enviados con el estado actualizado correctamente.</param>
/// <param name="FailedInvoiceIds">Identificadores de facturas cuyo procesamiento falló.</param>
/// <param name="SkippedInvoiceIds">Identificadores de facturas omitidas porque no se pudo actualizar su estado.</param>
public sealed record ReminderRunResult(
    int Processed,
    int Sent,
    IReadOnlyList<string> FailedInvoiceIds,
    IReadOnlyList<string> SkippedInvoiceIds);