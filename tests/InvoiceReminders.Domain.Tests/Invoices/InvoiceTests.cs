
using InvoiceReminders.Domain.Invoices;

namespace InvoiceReminders.Domain.Tests.Invoices;

public sealed class InvoiceTests
{
    [Theory]
    [InlineData(InvoiceStatus.PrimerRecordatorio)]
    [InlineData(InvoiceStatus.SegundoRecordatorio)]
    public void RequiresReminder_WhenStatusRequiresReminder_ReturnsTrue(
        InvoiceStatus status)
    {
        // Arrange
        var invoice = CreateInvoice(status);

        // Act
        var result = invoice.RequiresReminder;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RequiresReminder_WhenStatusIsDeactivated_ReturnsFalse()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.Desactivado);

        // Act
        var result = invoice.RequiresReminder;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AdvanceReminder_WhenStatusIsFirstReminder_AdvancesToSecondReminder()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.PrimerRecordatorio);

        // Act
        invoice.AdvanceReminder();

        // Assert
        Assert.Equal(InvoiceStatus.SegundoRecordatorio, invoice.Status);
    }

    [Fact]
    public void AdvanceReminder_WhenStatusIsSecondReminder_DeactivatesInvoice()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.SegundoRecordatorio);

        // Act
        invoice.AdvanceReminder();

        // Assert
        Assert.Equal(InvoiceStatus.Desactivado, invoice.Status);
        Assert.False(invoice.RequiresReminder);
    }

    [Fact]
    public void AdvanceReminder_WhenStatusIsDeactivated_ThrowsInvalidOperationException()
    {
        // Arrange
        var invoice = CreateInvoice(InvoiceStatus.Desactivado);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            invoice.AdvanceReminder);

        // Assert
        Assert.Contains(invoice.Number, exception.Message);
    }

    private static Invoice CreateInvoice(InvoiceStatus status)
    {
        return new Invoice(
            id: "invoice-001",
            clientId: "client-001",
            number: "FAC-001",
            amount: 150000m,
            dueDate: new DateOnly(2026, 10, 1),
            status: status);
    }
}
