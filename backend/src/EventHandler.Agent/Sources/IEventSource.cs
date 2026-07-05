using EventHandler.Contracts;

namespace EventHandler.Agent.Sources;

public interface IEventSource
{
    string Name { get; }

    /// <summary>Normalizes whatever this source produces into IncomingEventDto and calls
    /// sink.PublishAsync for each one. Two source flavors share one onboarding story
    /// (implement the interface, register it in Program.cs DI, add its config section —
    /// no other file changes): poll-style sources run their own loop and implement this
    /// interface; push-style sources implement <see cref="IHttpEventSource"/> instead and
    /// map endpoints onto the Agent's shared Kestrel host.</summary>
    Task StartAsync(IEventSink sink, CancellationToken ct);

    Task StopAsync(CancellationToken ct);
}
