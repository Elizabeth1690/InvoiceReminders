using InvoiceReminders.Application.Dtos;
using InvoiceReminders.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceReminders.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController : ControllerBase
{
    private readonly GetInvoicesSummaryUseCase _getInvoicesSummary;

    public InvoicesController(
        GetInvoicesSummaryUseCase getInvoicesSummary)
    {
        _getInvoicesSummary = getInvoicesSummary;
    }

    [HttpGet]
    public async Task<ActionResult<InvoicesSummaryDto>> GetAll(
        [FromQuery] string? clientId,
        CancellationToken ct)
    {
        return Ok(
            await _getInvoicesSummary.ExecuteAsync(clientId, ct));
    }
}