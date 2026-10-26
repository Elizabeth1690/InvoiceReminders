namespace InvoiceReminders.Api.Scheduling;

public static class SchedulingServiceCollectionExtensions
{
    public static IServiceCollection AddReminderScheduler(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SchedulerSettings>()
            .Bind(configuration.GetSection(SchedulerSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHostedService<ReminderSchedulerService>();

        return services;
    }
}