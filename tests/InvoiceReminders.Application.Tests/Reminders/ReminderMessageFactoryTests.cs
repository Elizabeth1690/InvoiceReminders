
using InvoiceReminders.Application.Reminders;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Application.Tests.Reminders;

public sealed class ReminderMessageFactoryTests
{
    private readonly Client _client = new(
        "client-1",
        "Acme",
        "acme@test.com");

    [Theory]
    [InlineData(
        InvoiceStatus.PrimerRecordatorio,
        "Primer recordatorio de pago: factura FAC-001")]
    [InlineData(
        InvoiceStatus.SegundoRecordatorio,
        "Segundo recordatorio de pago: factura FAC-001")]
    public void Create_WhenStrategyExists_ReturnsExpectedMessage(
        InvoiceStatus status,
        string expectedSubject)
    {
        // Arrange
        var invoice = new Invoice(
            "invoice-1",
            "client-1",
            "FAC-001",
            150000m,
            new DateOnly(2026, 10, 30),
            status);

        var factory = new ReminderMessageFactory(
        [
            new FirstReminderMessageStrategy(),
            new SecondReminderMessageStrategy()
        ]);

        // Act
        var message = factory.Create(_client, invoice);

        // Assert
        Assert.Equal(_client.Email, message.To);
        Assert.Equal(_client.Name, message.RecipientName);
        Assert.Equal(expectedSubject, message.Subject);
        Assert.Contains(invoice.Number, message.Body);
    }

    [Fact]
    public void Create_WhenNoStrategyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var invoice = new Invoice(
            "invoice-1",
            "client-1",
            "FAC-001",
            150000m,
            new DateOnly(2026, 10, 30),
            InvoiceStatus.Desactivado);

        var factory = new ReminderMessageFactory(
        [
            new FirstReminderMessageStrategy(),
            new SecondReminderMessageStrategy()
        ]);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => factory.Create(_client, invoice));

        // Assert
        Assert.Contains("Desactivado", exception.Message);
    }
}
