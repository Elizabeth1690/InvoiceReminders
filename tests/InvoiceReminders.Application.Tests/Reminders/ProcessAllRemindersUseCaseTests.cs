using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Application.Reminders;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Domain.Invoices;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InvoiceReminders.Application.Tests.Reminders;

public class ProcessAllRemindersUseCaseTests
{
    private readonly IClientRepository _clients =
        Substitute.For<IClientRepository>();

    private readonly IInvoiceRepository _invoices =
        Substitute.For<IInvoiceRepository>();

    private readonly IEmailSender _emailSender =
        Substitute.For<IEmailSender>();

    private readonly IReminderMessageFactory _messages =
        Substitute.For<IReminderMessageFactory>();

    private readonly ClientRunGate _gate = new();

    public ProcessAllRemindersUseCaseTests()
    {
        IReadOnlyList<Client> clients =
            new[]
            {
                new Client(
                    "c1",
                    "Acme",
                    "acme@test.com"),

                new Client(
                    "c2",
                    "Globex",
                    "globex@test.com")
            };

        _clients
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(clients);

        _messages
            .Create(
                Arg.Any<Client>(),
                Arg.Any<Invoice>())
            .Returns(
                new EmailMessage(
                    "x@test.com",
                    "X",
                    "asunto",
                    "cuerpo"));

        _invoices
            .TryUpdateStatusAsync(
                Arg.Any<string>(),
                Arg.Any<InvoiceStatus>(),
                Arg.Any<InvoiceStatus>(),
                Arg.Any<CancellationToken>())
            .Returns(true);

        GivenPending("c1", "a1");
        GivenPending("c2", "b1", "b2");
    }

    private ProcessAllRemindersUseCase CreateSut()
    {
        var processClient =
            new ProcessInvoiceRemindersUseCase(
                _clients,
                _invoices,
                _emailSender,
                _messages,
                _gate,
                NullLogger<ProcessInvoiceRemindersUseCase>
                    .Instance);

        return new ProcessAllRemindersUseCase(
            _clients,
            processClient,
            NullLogger<ProcessAllRemindersUseCase>
                .Instance);
    }

    private void GivenPending(
        string clientId,
        params string[] invoiceIds)
    {
        IReadOnlyList<Invoice> list =
            invoiceIds
                .Select(
                    id => new Invoice(
                        id,
                        clientId,
                        $"F-{id}",
                        100m,
                        new DateOnly(2026, 11, 1),
                        InvoiceStatus.PrimerRecordatorio))
                .ToList();

        _invoices
            .GetPendingRemindersByClientAsync(
                clientId,
                null,
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(list);
    }

    [Fact]
    public async Task Execute_ProcessesEveryClientAndAggregatesTotals()
    {
        var result =
            await CreateSut()
                .ExecuteAsync(CancellationToken.None);

        Assert.Equal(2, result.Clients.Count);
        Assert.Equal(3, result.Processed);
        Assert.Equal(3, result.Sent);
        Assert.Equal(0, result.ClientsWithErrors);
    }

    [Fact]
    public async Task Execute_WhenOneClientIsBusy_ReportsItsErrorAndContinuesWithTheOthers()
    {
        using var lease =
            _gate.TryEnter("c2");

        var result =
            await CreateSut()
                .ExecuteAsync(CancellationToken.None);

        Assert.Equal(1, result.ClientsWithErrors);
        Assert.Equal(1, result.Sent);
        Assert.Null(result.Clients[1].Result);
        Assert.NotNull(result.Clients[1].Error);
    }
}
