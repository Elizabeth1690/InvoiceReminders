using System.Collections.Concurrent;

namespace InvoiceReminders.Application.Reminders;

/// <summary>
/// Evita que dos procesos (por ejemplo el scheduler y una llamada manual) trabajen
/// sobre el mismo cliente a la vez dentro de una misma instancia de la aplicación.
/// Con varias instancias haría falta un candado distribuido.
/// </summary>
public sealed class ClientRunGate
{
    private readonly ConcurrentDictionary<string, byte> _running = new();

    /// <returns>Un "lease" que libera el candado al hacer Dispose, o null si el cliente ya está en proceso.</returns>
    public IDisposable? TryEnter(string clientId)
        => _running.TryAdd(clientId, 0) ? new Lease(this, clientId) : null;

    private void Release(string clientId) => _running.TryRemove(clientId, out _);

    private sealed class Lease : IDisposable
    {
        private readonly ClientRunGate _gate;
        private readonly string _clientId;
        private int _disposed;

        public Lease(ClientRunGate gate, string clientId)
        {
            _gate = gate;
            _clientId = clientId;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _gate.Release(_clientId);
            }
        }
    }
}