using InvoiceReminders.Application.Reminders;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Application.Tests.Reminders;

public sealed class ReminderMessageStrategyTests
{
    private readonly Client _client = new(
        "client-1",
        "Acme",
        "acme@test.com");

    [Fact]
    public void FirstReminder_Create_GeneratesExpectedMessage()
    {
        var invoice = new Invoice(
            "invoice-1",
            "client-1",
            "FAC-001",
            150000m,
            new DateOnly(2026, 10, 30),
            InvoiceStatus.PrimerRecordatorio);

        var strategy = new FirstReminderMessageStrategy();

        var message = strategy.Create(_client, invoice);

        Assert.Equal("acme@test.com", message.To);
        Assert.Equal("Acme", message.RecipientName);
        Assert.Equal(
            "Primer recordatorio de pago: factura FAC-001",
            message.Subject);
        Assert.Contains("FAC-001", message.Body);
        Assert.Contains("Acme", message.Body);
    }

    [Fact]
    public void SecondReminder_Create_GeneratesExpectedMessage()
    {
        var invoice = new Invoice(
            "invoice-1",
            "client-1",
            "FAC-001",
            150000m,
            new DateOnly(2026, 10, 30),
            InvoiceStatus.SegundoRecordatorio);

        var strategy = new SecondReminderMessageStrategy();

        var message = strategy.Create(_client, invoice);

        Assert.Equal("acme@test.com", message.To);
        Assert.Equal("Acme", message.RecipientName);
        Assert.Equal(
            "Segundo recordatorio de pago: factura FAC-001",
            message.Subject);
        Assert.Contains("FAC-001", message.Body);
        Assert.Contains("desactivado", message.Body);
    }
}