using EventHandler.Contracts;

namespace EventHandler.Agent.Sources;

public interface IEventSource
{
    string Name { get; }

    /// <summary>Normalizes whatever this source produces into IncomingEventDto and calls
    /// sink.PublishAsync for each one. Onboarding a new source: implement this interface,
    /// register it in Program.cs DI, add its config section — no other file changes.</summary>
    Task StartAsync(IEventSink sink, CancellationToken ct);

    Task StopAsync(CancellationToken ct);
}
