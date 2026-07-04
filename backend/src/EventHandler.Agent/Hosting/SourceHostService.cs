using EventHandler.Agent.Sources;

namespace EventHandler.Agent.Hosting;

/// <summary>Resolves every registered IEventSource and starts them against the shared
/// ingestion sink (skeleton-plan.md §8).</summary>
public sealed class SourceHostService : BackgroundService
{
    private readonly IEnumerable<IEventSource> _sources;
    private readonly IEventSink _sink;
    private readonly ILogger<SourceHostService> _logger;

    public SourceHostService(IEnumerable<IEventSource> sources, IEventSink sink, ILogger<SourceHostService> logger)
    {
        _sources = sources;
        _sink = sink;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var source in _sources)
        {
            _logger.LogInformation("Starting event source '{SourceName}'", source.Name);
            await source.StartAsync(_sink, stoppingToken);
        }

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Expected on shutdown.
        }
        finally
        {
            foreach (var source in _sources)
            {
                await source.StopAsync(CancellationToken.None);
            }
        }
    }
}
