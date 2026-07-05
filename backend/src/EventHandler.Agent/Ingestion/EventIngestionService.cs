using EventHandler.Agent.ServerClient;
using EventHandler.Agent.Sources;
using EventHandler.Contracts;

namespace EventHandler.Agent.Ingestion;

public sealed class EventIngestionService : IEventSink
{
    private readonly IServerClient _serverClient;
    private readonly ILogger<EventIngestionService> _logger;

    public EventIngestionService(IServerClient serverClient, ILogger<EventIngestionService> logger)
    {
        _serverClient = serverClient;
        _logger = logger;
    }

    public async Task<IntakeResponseDto> PublishAsync(IncomingEventDto evt, CancellationToken ct)
    {
        _logger.LogInformation("Ingesting event {SourceName}/{SourceEventId}", evt.SourceName, evt.SourceEventId);
        return await _serverClient.SendAsync(evt, ct);
    }
}
