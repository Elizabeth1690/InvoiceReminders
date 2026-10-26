using System.ComponentModel.DataAnnotations;

namespace InvoiceReminders.Infrastructure.Settings;

public sealed class MongoSettings
{
    public const string SectionName = "Mongo";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public string DatabaseName { get; set; } = string.Empty;
}