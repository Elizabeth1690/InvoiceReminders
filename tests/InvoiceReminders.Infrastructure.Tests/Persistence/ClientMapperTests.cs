using InvoiceReminders.Infrastructure.Persistence.Documents;
using InvoiceReminders.Infrastructure.Persistence.Mappers;

namespace InvoiceReminders.Infrastructure.Tests.Persistence;

public class ClientMapperTests
{
    [Fact]
    public void ToDomain_MapsAllFields()
    {
        var document = new ClientDocument { Id = "acme", Name = "Acme S.A.S.", Email = "contacto@acme.test" };

        var client = ClientMapper.ToDomain(document);

        Assert.Equal("acme", client.Id);
        Assert.Equal("Acme S.A.S.", client.Name);
        Assert.Equal("contacto@acme.test", client.Email);
    }
}