using EventHandler.Contracts;

namespace EventHandler.Agent.Sources;

public interface IEventSink
{
    Task PublishAsync(IncomingEventDto evt, CancellationToken ct);
}
