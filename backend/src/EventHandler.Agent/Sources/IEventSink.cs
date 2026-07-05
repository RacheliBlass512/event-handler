using EventHandler.Contracts;

namespace EventHandler.Agent.Sources;

public interface IEventSink
{
    Task<IntakeResponseDto> PublishAsync(IncomingEventDto evt, CancellationToken ct);
}
