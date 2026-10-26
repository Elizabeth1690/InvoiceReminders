namespace InvoiceReminders.Application.Dtos;

public sealed record ClientDto(
    string Id,
    string Name,
    string Email);