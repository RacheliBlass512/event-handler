using System.Net.Http.Json;
using EventHandler.Contracts;

namespace EventHandler.Agent.ServerClient;

/// <summary>
/// Sends directly over HTTP — no outbox/retry (this session's simplification). Kept behind
/// IServerClient so a durable outbox can be dropped in later without touching sources or
/// ingestion (skeleton-plan.md §8, §12).
/// </summary>
public sealed class HttpServerClient : IServerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpServerClient> _logger;

    public HttpServerClient(HttpClient httpClient, ILogger<HttpServerClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IntakeResponseDto> SendAsync(IncomingEventDto evt, CancellationToken ct)
    {
        _logger.LogInformation(
            "POST api/intake/events for {SourceName}/{SourceEventId}", evt.SourceName, evt.SourceEventId);

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("api/intake/events", evt, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<IntakeResponseDto>(cancellationToken: ct);
            return result ?? new IntakeResponseDto(Guid.Empty, Accepted: false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex, "Intake POST failed for {SourceName}/{SourceEventId}", evt.SourceName, evt.SourceEventId);
            return new IntakeResponseDto(Guid.Empty, Accepted: false);
        }
    }
}
