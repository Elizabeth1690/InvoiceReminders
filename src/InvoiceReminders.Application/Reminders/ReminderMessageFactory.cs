using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Application.Reminders;

public sealed class ReminderMessageFactory : IReminderMessageFactory
{
    private readonly IEnumerable<IReminderMessageStrategy> _strategies;

    public ReminderMessageFactory(IEnumerable<IReminderMessageStrategy> strategies)
    {
        _strategies = strategies;
    }

    public EmailMessage Create(Client client, Invoice invoice)
    {
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(invoice));

        return strategy?.Create(client, invoice)
            ?? throw new InvalidOperationException(
                $"No existe una estrategia de mensaje para el estado {invoice.Status}.");
    }
}