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
        // The Agent stamps the creation time on ingestion — sources don't supply it (and any
        // value they did send is overwritten here). Central so every source gets it uniformly.
        evt = evt with { CreatedAt = DateTime.UtcNow };
        _logger.LogInformation("Ingesting event {SourceName}/{SourceEventId}", evt.SourceName, evt.SourceEventId);
        return await _serverClient.SendAsync(evt, ct);
    }
}
