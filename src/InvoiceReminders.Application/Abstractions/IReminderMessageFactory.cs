using InvoiceReminders.Application.Reminders;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Domain.Invoices;
using System.Net.Mail;

namespace InvoiceReminders.Application.Abstractions;

public interface IReminderMessageFactory
{
    EmailMessage Create(Client client, Invoice invoice);
}