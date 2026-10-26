using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Application.Reminders;

public sealed class FirstReminderMessageStrategy : IReminderMessageStrategy
{
    public bool CanHandle(Invoice invoice) =>
        invoice.Status == InvoiceStatus.PrimerRecordatorio;

    public EmailMessage Create(Client client, Invoice invoice) =>
        new(
            client.Email,
            client.Name,
            $"Primer recordatorio de pago: factura {invoice.Number}",
            $"Hola {client.Name}, la factura {invoice.Number}, por valor de {invoice.Amount:C}, tiene fecha de vencimiento {invoice.DueDate:dd/MM/yyyy}. Por favor, verifica su pago.");
}