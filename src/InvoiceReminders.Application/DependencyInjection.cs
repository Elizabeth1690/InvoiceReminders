using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Application.Queries;
using InvoiceReminders.Application.Reminders;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceReminders.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IReminderMessageStrategy, FirstReminderMessageStrategy>();
        services.AddScoped<IReminderMessageStrategy, SecondReminderMessageStrategy>();
        services.AddScoped<IReminderMessageFactory, ReminderMessageFactory>();

        // Singleton: el candado debe ser compartido por todas las peticiones y el scheduler.
        services.AddSingleton<ClientRunGate>();

        services.AddScoped<ProcessInvoiceRemindersUseCase>();
        services.AddScoped<ProcessAllRemindersUseCase>();
        services.AddScoped<GetInvoicesSummaryUseCase>();
        services.AddScoped<GetClientsUseCase>();

        return services;
    }
}