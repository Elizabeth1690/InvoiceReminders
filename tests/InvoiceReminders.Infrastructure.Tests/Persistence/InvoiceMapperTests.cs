using InvoiceReminders.Domain.Invoices;
using InvoiceReminders.Infrastructure.Persistence.Documents;
using InvoiceReminders.Infrastructure.Persistence.Mappers;

namespace InvoiceReminders.Infrastructure.Tests.Persistence;

public class InvoiceMapperTests
{
    private static InvoiceDocument CreateDocument(string status) => new()
    {
        Id = "inv-1",
        ClientId = "acme",
        Number = "F-1",
        Amount = 1500m,
        DueDate = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc),
        Status = status
    };

    [Fact]
    public void ToDomainOrNull_WithValidDocument_MapsAllFields()
    {
        var invoice = InvoiceMapper.ToDomainOrNull(CreateDocument("segundorecordatorio"));

        Assert.NotNull(invoice);
        Assert.Equal("inv-1", invoice.Id);
        Assert.Equal("acme", invoice.ClientId);
        Assert.Equal(1500m, invoice.Amount);
        Assert.Equal(new DateOnly(2026, 11, 1), invoice.DueDate);
        Assert.Equal(InvoiceStatus.SegundoRecordatorio, invoice.Status);
    }

    [Fact]
    public void ToDomainOrNull_WithUnknownStatus_ReturnsNull() =>
        Assert.Null(InvoiceMapper.ToDomainOrNull(CreateDocument("xyz")));
}