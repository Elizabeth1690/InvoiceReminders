namespace InvoiceReminders.Application.Dtos;

public sealed record InvoiceDto(
    string Id,
    string ClientId,
    string ClientName,
    string Number,
    decimal Amount,
    DateOnly DueDate,
    string Status);