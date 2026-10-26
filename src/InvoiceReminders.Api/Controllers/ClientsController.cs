using InvoiceReminders.Application.Dtos;
using InvoiceReminders.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceReminders.Api.Controllers;

[ApiController]
[Route("api/clients")]
public sealed class ClientsController : ControllerBase
{
    private readonly GetClientsUseCase _getClients;

    public ClientsController(GetClientsUseCase getClients)
    {
        _getClients = getClients;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClientDto>>> GetAll(
        CancellationToken ct)
    {
        return Ok(await _getClients.ExecuteAsync(ct));
    }
}