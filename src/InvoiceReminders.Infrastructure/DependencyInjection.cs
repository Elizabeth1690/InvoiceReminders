using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Infrastructure.Email;
using InvoiceReminders.Infrastructure.Persistence;
using InvoiceReminders.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InvoiceReminders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Valida la configuración al iniciar la aplicación para detectar errores antes de atender solicitudes.
        services.AddOptions<MongoSettings>()
            .Bind(configuration.GetSection(MongoSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SmtpSettings>()
            .Bind(configuration.GetSection(SmtpSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(sp.GetRequiredService<IOptions<MongoSettings>>().Value.ConnectionString));

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
            return sp.GetRequiredService<IMongoClient>().GetDatabase(settings.DatabaseName);
        });

        services.AddScoped<IInvoiceRepository, MongoInvoiceRepository>();
        services.AddScoped<IClientRepository, MongoClientRepository>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}