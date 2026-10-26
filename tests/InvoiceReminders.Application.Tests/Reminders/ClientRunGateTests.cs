using InvoiceReminders.Application.Reminders;

namespace InvoiceReminders.Application.Tests.Reminders;

public class ClientRunGateTests
{
    [Fact]
    public void TryEnter_WhenClientIsAlreadyRunning_ReturnsNull()
    {
        var gate = new ClientRunGate();
        using var first = gate.TryEnter("c1");

        Assert.NotNull(first);
        Assert.Null(gate.TryEnter("c1"));
    }

    [Fact]
    public void TryEnter_AfterRelease_AllowsEnteringAgain()
    {
        var gate = new ClientRunGate();
        var first = gate.TryEnter("c1");
        Assert.NotNull(first);
        first.Dispose();

        using var second = gate.TryEnter("c1");

        Assert.NotNull(second);
    }

    [Fact]
    public void TryEnter_ForDifferentClients_DoesNotBlock()
    {
        var gate = new ClientRunGate();
        using var first = gate.TryEnter("c1");

        using var second = gate.TryEnter("c2");

        Assert.NotNull(second);
    }
}