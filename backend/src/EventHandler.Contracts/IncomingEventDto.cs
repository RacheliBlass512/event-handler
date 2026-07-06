namespace EventHandler.Contracts;

/// <summary>
/// Canonical intake payload shared by Agent and Server. The Agent normalizes every source's
/// event into this shape before sending; no status field — the Server always assigns
/// <c>EventStatus.New</c> on intake.
/// </summary>
public sealed record IncomingEventDto(
    string SourceName,
    string SourceEventId,
    string Title,
    string Description,
    string Location,
    DateTime CreatedAt,
    string Priority);
