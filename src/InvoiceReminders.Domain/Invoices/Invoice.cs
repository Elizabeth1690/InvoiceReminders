namespace InvoiceReminders.Domain.Invoices;

public sealed class Invoice
{
    public Invoice(string id, string clientId, string number, decimal amount,
                   DateOnly dueDate, InvoiceStatus status)
    {
        Id = id;
        ClientId = clientId;
        Number = number;
        Amount = amount;
        DueDate = dueDate;
        Status = status;
    }

    public string Id { get; }
    public string ClientId { get; }
    public string Number { get; }
    public decimal Amount { get; }
    public DateOnly DueDate { get; }
    public InvoiceStatus Status { get; private set; }

    public bool RequiresReminder =>
        Status is InvoiceStatus.PrimerRecordatorio or InvoiceStatus.SegundoRecordatorio;

    public void AdvanceReminder()
    {
        Status = Status switch
        {
            InvoiceStatus.PrimerRecordatorio => InvoiceStatus.SegundoRecordatorio,
            InvoiceStatus.SegundoRecordatorio => InvoiceStatus.Desactivado,
            _ => throw new InvalidOperationException(
                $"La factura {Number} está desactivada y no admite más recordatorios.")
        };
    }
}