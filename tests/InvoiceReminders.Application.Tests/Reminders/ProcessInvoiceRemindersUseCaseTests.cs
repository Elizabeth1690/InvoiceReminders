using InvoiceReminders.Application.Abstractions;
using InvoiceReminders.Application.Exceptions;
using InvoiceReminders.Application.Reminders;
using InvoiceReminders.Domain.Clients;
using InvoiceReminders.Domain.Invoices;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace InvoiceReminders.Application.Tests.Reminders;

public class ProcessInvoiceRemindersUseCaseTests
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

    private readonly Client _client =
        new("c1", "Acme", "acme@test.com");

    public ProcessInvoiceRemindersUseCaseTests()
    {
        _clients
            .GetByIdAsync(
                "c1",
                Arg.Any<CancellationToken>())
            .Returns(_client);

        _messages
            .Create(
                Arg.Any<Client>(),
                Arg.Any<Invoice>())
            .Returns(
                new EmailMessage(
                    "acme@test.com",
                    "Acme",
                    "asunto",
                    "cuerpo"));

        _invoices
            .TryUpdateStatusAsync(
                Arg.Any<string>(),
                Arg.Any<InvoiceStatus>(),
                Arg.Any<InvoiceStatus>(),
                Arg.Any<CancellationToken>())
            .Returns(true);
    }

    private ProcessInvoiceRemindersUseCase CreateSut() =>
        new(
            _clients,
            _invoices,
            _emailSender,
            _messages,
            _gate,
            NullLogger<ProcessInvoiceRemindersUseCase>.Instance);

    private static Invoice CreateInvoice(
        string id,
        InvoiceStatus status) =>
        new(
            id,
            "c1",
            $"F-{id}",
            100m,
            new DateOnly(2026, 11, 1),
            status);

    private void GivenPending(params Invoice[] invoices)
    {
        IReadOnlyList<Invoice> list = invoices;

        _invoices
            .GetPendingRemindersByClientAsync(
                "c1",
                null,
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(list);
    }

    [Fact]
    public async Task Execute_WhenEmailIsSent_PersistsNextStatus()
    {
        var invoice =
            CreateInvoice(
                "i1",
                InvoiceStatus.PrimerRecordatorio);

        GivenPending(invoice);

        var result =
            await CreateSut()
                .ExecuteAsync(
                    "c1",
                    CancellationToken.None);

        Assert.Equal(1, result.Sent);

        await _invoices
            .Received(1)
            .TryUpdateStatusAsync(
                "i1",
                InvoiceStatus.PrimerRecordatorio,
                InvoiceStatus.SegundoRecordatorio,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_WhenSecondReminderIsSent_PersistsDeactivatedStatus()
    {
        var invoice =
            CreateInvoice(
                "i1",
                InvoiceStatus.SegundoRecordatorio);

        GivenPending(invoice);

        var result =
            await CreateSut()
                .ExecuteAsync(
                    "c1",
                    CancellationToken.None);

        Assert.Equal(1, result.Sent);

        await _invoices
            .Received(1)
            .TryUpdateStatusAsync(
                "i1",
                InvoiceStatus.SegundoRecordatorio,
                InvoiceStatus.Desactivado,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_WhenEmailFails_DoesNotPersistStatus()
    {
        var invoice =
            CreateInvoice(
                "i1",
                InvoiceStatus.PrimerRecordatorio);

        GivenPending(invoice);

        _emailSender
            .SendAsync(
                Arg.Any<EmailMessage>(),
                Arg.Any<CancellationToken>())
            .ThrowsAsync(
                new InvalidOperationException(
                    "smtp caído"));

        var result =
            await CreateSut()
                .ExecuteAsync(
                    "c1",
                    CancellationToken.None);

        Assert.Equal(0, result.Sent);

        Assert.Equal(
            new[] { "i1" },
            result.FailedInvoiceIds);

        await _invoices
            .DidNotReceive()
            .TryUpdateStatusAsync(
                Arg.Any<string>(),
                Arg.Any<InvoiceStatus>(),
                Arg.Any<InvoiceStatus>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_WhenClientDoesNotExist_ThrowsClientNotFoundException()
    {
        _clients
            .GetByIdAsync(
                "nope",
                Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<Client?>(null));

        await Assert.ThrowsAsync<ClientNotFoundException>(
            () => CreateSut()
                .ExecuteAsync(
                    "nope",
                    CancellationToken.None));
    }

    [Fact]
    public async Task Execute_WhenThereAreNoPendingInvoices_ReturnsEmptyResult()
    {
        GivenPending();

        var result =
            await CreateSut()
                .ExecuteAsync(
                    "c1",
                    CancellationToken.None);

        Assert.Equal(0, result.Processed);
        Assert.Equal(0, result.Sent);
        Assert.Empty(result.FailedInvoiceIds);
        Assert.Empty(result.SkippedInvoiceIds);

        await _emailSender
            .DidNotReceive()
            .SendAsync(
                Arg.Any<EmailMessage>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_WhenOneInvoiceFails_ContinuesWithTheRest()
    {
        var first =
            CreateInvoice(
                "i1",
                InvoiceStatus.PrimerRecordatorio);

        var second =
            CreateInvoice(
                "i2",
                InvoiceStatus.SegundoRecordatorio);

        GivenPending(first, second);

        _emailSender
            .SendAsync(
                Arg.Any<EmailMessage>(),
                Arg.Any<CancellationToken>())
            .Returns(
                Task.FromException(
                    new InvalidOperationException("boom")),
                Task.CompletedTask);

        var result =
            await CreateSut()
                .ExecuteAsync(
                    "c1",
                    CancellationToken.None);

        Assert.Equal(2, result.Processed);
        Assert.Equal(1, result.Sent);

        Assert.Equal(
            new[] { "i1" },
            result.FailedInvoiceIds);

        await _invoices
            .DidNotReceive()
            .TryUpdateStatusAsync(
                "i1",
                Arg.Any<InvoiceStatus>(),
                Arg.Any<InvoiceStatus>(),
                Arg.Any<CancellationToken>());

        await _invoices
            .Received(1)
            .TryUpdateStatusAsync(
                "i2",
                InvoiceStatus.SegundoRecordatorio,
                InvoiceStatus.Desactivado,
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_WhenStatusChangedByAnotherProcess_ReportsInvoiceAsSkipped()
    {
        var invoice =
            CreateInvoice(
                "i1",
                InvoiceStatus.PrimerRecordatorio);

        GivenPending(invoice);

        _invoices
            .TryUpdateStatusAsync(
                Arg.Any<string>(),
                Arg.Any<InvoiceStatus>(),
                Arg.Any<InvoiceStatus>(),
                Arg.Any<CancellationToken>())
            .Returns(false);

        var result =
            await CreateSut()
                .ExecuteAsync(
                    "c1",
                    CancellationToken.None);

        Assert.Equal(0, result.Sent);

        Assert.Equal(
            new[] { "i1" },
            result.SkippedInvoiceIds);

        Assert.Empty(result.FailedInvoiceIds);
    }

    [Fact]
    public async Task Execute_WithMoreInvoicesThanBatchSize_ProcessesEveryBatch()
    {
        const int batchSize =
            ProcessInvoiceRemindersUseCase.BatchSize;

        var all =
            Enumerable
                .Range(1, batchSize + 50)
                .Select(
                    n => CreateInvoice(
                        $"i{n:D3}",
                        InvoiceStatus.PrimerRecordatorio))
                .ToList();

        IReadOnlyList<Invoice> firstBatch =
            all.Take(batchSize).ToList();

        IReadOnlyList<Invoice> secondBatch =
            all.Skip(batchSize).ToList();

        _invoices
            .GetPendingRemindersByClientAsync(
                "c1",
                null,
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(firstBatch);

        _invoices
            .GetPendingRemindersByClientAsync(
                "c1",
                firstBatch[^1].Id,
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .Returns(secondBatch);

        var result =
            await CreateSut()
                .ExecuteAsync(
                    "c1",
                    CancellationToken.None);

        Assert.Equal(
            batchSize + 50,
            result.Processed);

        Assert.Equal(
            batchSize + 50,
            result.Sent);
    }

    [Fact]
    public async Task Execute_WhenClientRunIsAlreadyInProgress_ThrowsReminderRunInProgressException()
    {
        using var lease =
            _gate.TryEnter("c1");

        await Assert.ThrowsAsync<ReminderRunInProgressException>(
            () => CreateSut()
                .ExecuteAsync(
                    "c1",
                    CancellationToken.None));
    }
}
