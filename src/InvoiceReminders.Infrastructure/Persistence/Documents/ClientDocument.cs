using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InvoiceReminders.Infrastructure.Persistence.Documents;

public sealed class ClientDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
}