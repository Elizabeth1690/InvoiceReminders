using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Application.Reminders;

public sealed class SecondReminderMessageStrategy : IReminderMessageStrategy
{
    public bool CanHandle(Invoice invoice) =>
        invoice.Status == InvoiceStatus.SegundoRecordatorio;

    public EmailMessage Create(Client client, Invoice invoice) =>
        new(
            client.Email,
            client.Name,
            $"Segundo recordatorio de pago: factura {invoice.Number}",
            $"Hola {client.Name}, la factura {invoice.Number}, por valor de {invoice.Amount:C}, continúa pendiente. Si no se registra el pago, el servicio podrá ser desactivado.");
}