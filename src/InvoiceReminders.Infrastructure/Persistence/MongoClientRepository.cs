using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Infrastructure.Persistence.Documents;
using InvoiceReminders.Infrastructure.Persistence.Mappers;
using MongoDB.Driver;

namespace InvoiceReminders.Infrastructure.Persistence;

public sealed class MongoClientRepository : IClientRepository
{
    private readonly IMongoCollection<ClientDocument> _collection;

    public MongoClientRepository(IMongoDatabase database)
        => _collection = database.GetCollection<ClientDocument>(CollectionNames.Clients);

    public async Task<Client?> GetByIdAsync(string clientId, CancellationToken ct)
    {
        var document = await _collection.Find(x => x.Id == clientId).FirstOrDefaultAsync(ct);
        return document is null ? null : ClientMapper.ToDomain(document);
    }

    public async Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken ct)
    {
        var documents = await _collection
            .Find(FilterDefinition<ClientDocument>.Empty)
            .SortBy(x => x.Name)
            .ToListAsync(ct);

        return documents.Select(ClientMapper.ToDomain).ToList();
    }
}