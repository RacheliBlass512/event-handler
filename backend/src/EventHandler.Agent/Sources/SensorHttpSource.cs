using EventHandler.Contracts;

namespace EventHandler.Agent.Sources;

/// <summary>
/// Push-style HTTP sensor intake: owns POST /events/sensor on the Agent's shared Kestrel host.
/// Model binding rejects malformed JSON with an automatic 400; <see cref="TryValidate"/> guards
/// required fields — this endpoint is where raw external input enters the system.
/// </summary>
public sealed class SensorHttpSource : IHttpEventSource
{
    public string Name => "SensorHttp";

    public void MapRoutes(IEndpointRouteBuilder app, IEventSink sink)
    {
        app.MapPost("/events/sensor", async (IncomingEventDto evt, CancellationToken ct) =>
        {
            if (!TryValidate(evt, out var error))
            {
                return Results.BadRequest(error);
            }

            var result = await sink.PublishAsync(evt, ct);
            return Results.Accepted(value: result);
        });
    }

    internal static bool TryValidate(IncomingEventDto evt, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(evt.SourceName)) error = "SourceName is required.";
        else if (string.IsNullOrWhiteSpace(evt.SourceEventId)) error = "SourceEventId is required.";
        else if (string.IsNullOrWhiteSpace(evt.Title)) error = "Title is required.";
        else if (string.IsNullOrWhiteSpace(evt.Description)) error = "Description is required.";
        else if (string.IsNullOrWhiteSpace(evt.Location)) error = "Location is required.";
        return error is null;
    }
}
