using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Domain.Invoices;
using InvoiceReminders.Infrastructure.Persistence.Documents;
using InvoiceReminders.Infrastructure.Persistence.Mappers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace InvoiceReminders.Infrastructure.Persistence;

public sealed class MongoInvoiceRepository : IInvoiceRepository
{
    private static readonly string[] PendingStatuses =
    {
        InvoiceStatusMapper.ToDocumentValue(
            InvoiceStatus.PrimerRecordatorio),

        InvoiceStatusMapper.ToDocumentValue(
            InvoiceStatus.SegundoRecordatorio)
    };

    private readonly IMongoCollection<InvoiceDocument> _collection;
    private readonly ILogger<MongoInvoiceRepository> _logger;

    public MongoInvoiceRepository(
        IMongoDatabase database,
        ILogger<MongoInvoiceRepository> logger)
    {
        _collection = database.GetCollection<InvoiceDocument>(
            CollectionNames.Invoices);

        _logger = logger;
    }

    public async Task<IReadOnlyList<Invoice>> GetPendingRemindersByClientAsync(
        string clientId,
        string? afterId,
        int batchSize,
        CancellationToken ct)
    {
        var filters = Builders<InvoiceDocument>.Filter;

        var filter = filters.And(
            filters.Eq(x => x.ClientId, clientId),
            filters.In(x => x.Status, PendingStatuses));

        if (!string.IsNullOrEmpty(afterId))
        {
            filter &= filters.Gt(x => x.Id, afterId);
        }

        var documents = await _collection
            .Find(filter)
            .SortBy(x => x.Id)
            .Limit(batchSize)
            .ToListAsync(ct);

        return MapValid(documents);
    }

    public async Task<IReadOnlyList<Invoice>> GetAllAsync(
        string? clientId,
        CancellationToken ct)
    {
        var filter = string.IsNullOrWhiteSpace(clientId)
            ? FilterDefinition<InvoiceDocument>.Empty
            : Builders<InvoiceDocument>.Filter.Eq(
                x => x.ClientId,
                clientId);

        var documents = await _collection
            .Find(filter)
            .SortBy(x => x.DueDate)
            .ToListAsync(ct);

        return MapValid(documents);
    }

    public async Task<bool> TryUpdateStatusAsync(
        string invoiceId,
        InvoiceStatus expectedCurrentStatus,
        InvoiceStatus newStatus,
        CancellationToken ct)
    {
        var filters = Builders<InvoiceDocument>.Filter;

        var filter = filters.And(
            filters.Eq(x => x.Id, invoiceId),
            filters.Eq(
                x => x.Status,
                InvoiceStatusMapper.ToDocumentValue(
                    expectedCurrentStatus)));

        var update = Builders<InvoiceDocument>.Update
            .Set(
                x => x.Status,
                InvoiceStatusMapper.ToDocumentValue(newStatus));

        var result = await _collection.UpdateOneAsync(
            filter,
            update,
            cancellationToken: ct);

        return result.IsAcknowledged
            && result.ModifiedCount == 1;
    }

    private IReadOnlyList<Invoice> MapValid(
        IEnumerable<InvoiceDocument> documents)
    {
        var invoices = new List<Invoice>();

        foreach (var document in documents)
        {
            var invoice = InvoiceMapper.ToDomainOrNull(document);

            if (invoice is null)
            {
                _logger.LogWarning(
                    "Factura {InvoiceId} omitida: estado desconocido '{Status}'.",
                    document.Id,
                    document.Status);

                continue;
            }

            invoices.Add(invoice);
        }

        return invoices;
    }
}