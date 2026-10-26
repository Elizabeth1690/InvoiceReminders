using InvoiceReminders.Application.Reminders;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceReminders.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class RemindersController : ControllerBase
{
    private readonly ProcessInvoiceRemindersUseCase _processClient;
    private readonly ProcessAllRemindersUseCase _processAll;

    public RemindersController(
        ProcessInvoiceRemindersUseCase processClient,
        ProcessAllRemindersUseCase processAll)
    {
        _processClient = processClient;
        _processAll = processAll;
    }

    [HttpPost("reminders/process")]
    public async Task<ActionResult<ProcessAllRemindersResult>> ProcessAll(CancellationToken ct)
        => Ok(await _processAll.ExecuteAsync(ct));

    [HttpPost("clients/{clientId}/reminders/process")]
    public async Task<ActionResult<ReminderRunResult>> ProcessClient(string clientId, CancellationToken ct)
        => Ok(await _processClient.ExecuteAsync(clientId, ct));
}