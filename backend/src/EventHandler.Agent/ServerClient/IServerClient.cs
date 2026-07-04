using EventHandler.Contracts;

namespace EventHandler.Agent.ServerClient;

public interface IServerClient
{
    Task<IntakeResponseDto> SendAsync(IncomingEventDto evt, CancellationToken ct);
}
