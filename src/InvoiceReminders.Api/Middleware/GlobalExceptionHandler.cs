using InvoiceReminders.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceReminders.Api.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            ClientNotFoundException => (StatusCodes.Status404NotFound, "Cliente no encontrado"),
            ReminderRunInProgressException => (StatusCodes.Status409Conflict, "Procesamiento en curso"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        var isServerError = status == StatusCodes.Status500InternalServerError;
        if (isServerError)
        {
            _logger.LogError(exception, "Error no controlado");
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = isServerError ? null : exception.Message
        }, cancellationToken);

        return true;
    }
}