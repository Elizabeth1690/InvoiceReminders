using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InvoiceReminders.Infrastructure.Persistence.Documents;

public sealed class InvoiceDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("clientId")]
    public string ClientId { get; set; } = string.Empty;

    [BsonElement("number")]
    public string Number { get; set; } = string.Empty;

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("dueDate")]
    public DateTime DueDate { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;
}