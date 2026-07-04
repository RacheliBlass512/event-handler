namespace EventHandler.Agent.Sources;

/// <summary>
/// Stub source demonstrating the extension point (skeleton-plan.md §8) — an example of what
/// onboarding a push-style HTTP sensor would look like. Not registered in Program.cs by
/// default; register it there to bring it online.
/// </summary>
public sealed class SensorHttpSource : IEventSource
{
    public string Name => "SensorHttp";

    public Task StartAsync(IEventSink sink, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
