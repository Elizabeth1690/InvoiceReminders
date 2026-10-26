using InvoiceReminders.Application.Reminders;
using Microsoft.Extensions.Options;

namespace InvoiceReminders.Api.Scheduling;

public sealed class ReminderSchedulerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SchedulerSettings _settings;
    private readonly ILogger<ReminderSchedulerService> _logger;

    public ReminderSchedulerService(
        IServiceScopeFactory scopeFactory,
        IOptions<SchedulerSettings> options,
        ILogger<ReminderSchedulerService> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("Scheduler de recordatorios deshabilitado.");
            return;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_settings.IntervalMinutes));

        try
        {
            do
            {
                await RunOnceAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            // Apagado normal de la aplicación.
        }
    }

    private async Task RunOnceAsync(CancellationToken ct)
    {
        try
        {
            // Los casos de uso son "scoped": cada ejecución necesita su propio scope.
            await using var scope = _scopeFactory.CreateAsyncScope();
            var useCase = scope.ServiceProvider.GetRequiredService<ProcessAllRemindersUseCase>();

            var result = await useCase.ExecuteAsync(ct);

            _logger.LogInformation(
                "Recordatorios: {Processed} procesadas, {Sent} enviadas, {Failed} fallidas, {Skipped} omitidas.",
                result.Processed, result.Sent, result.Failed, result.Skipped);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Falló la ejecución programada de recordatorios.");
        }
    }
}