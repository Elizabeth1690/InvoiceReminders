using InvoiceReminders.Domain.Invoices;
using InvoiceReminders.Infrastructure.Persistence.Mappers;

namespace InvoiceReminders.Infrastructure.Tests.Persistence;

public class InvoiceStatusMapperTests
{
    [Theory]
    [InlineData(InvoiceStatus.PrimerRecordatorio, "primerrecordatorio")]
    [InlineData(InvoiceStatus.SegundoRecordatorio, "segundorecordatorio")]
    [InlineData(InvoiceStatus.Desactivado, "desactivado")]
    public void ToDocumentValue_ReturnsExpectedValue(InvoiceStatus status, string expected) =>
        Assert.Equal(expected, InvoiceStatusMapper.ToDocumentValue(status));

    [Theory]
    [InlineData("primerrecordatorio", InvoiceStatus.PrimerRecordatorio)]
    [InlineData("segundorecordatorio", InvoiceStatus.SegundoRecordatorio)]
    [InlineData("desactivado", InvoiceStatus.Desactivado)]
    [InlineData("  PRIMERRECORDATORIO ", InvoiceStatus.PrimerRecordatorio)]
    public void TryFromDocumentValue_WithKnownValue_ReturnsStatus(string value, InvoiceStatus expected)
    {
        var parsed = InvoiceStatusMapper.TryFromDocumentValue(value, out var status);

        Assert.True(parsed);
        Assert.Equal(expected, status);
    }

    [Theory]
    [InlineData("otro")]
    [InlineData("")]
    [InlineData(null)]
    public void TryFromDocumentValue_WithUnknownValue_ReturnsFalse(string? value) =>
        Assert.False(InvoiceStatusMapper.TryFromDocumentValue(value, out _));
}