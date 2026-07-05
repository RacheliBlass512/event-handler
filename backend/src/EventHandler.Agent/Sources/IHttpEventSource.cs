namespace EventHandler.Agent.Sources;

/// <summary>
/// Push-style counterpart of <see cref="IEventSource"/>: instead of running its own loop, the
/// source maps its endpoints onto the Agent's shared Kestrel host and fully owns its HTTP
/// surface (route, payload shape, validation). Onboarding a push source: implement this
/// interface, register it in Program.cs DI, add its config section — no other file changes.
/// </summary>
public interface IHttpEventSource
{
    string Name { get; }

    void MapRoutes(IEndpointRouteBuilder app, IEventSink sink);
}
