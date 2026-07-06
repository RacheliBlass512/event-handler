using EventHandler.Domain.Entities;

namespace EventHandler.Server.Application;

/// <summary>
/// Routing brain for real-time alerts: connected (per PresenceTracker) → SignalR (Mode A);
/// not connected → IWebPushSender (Mode B). See skeleton-plan.md §4a/§4b for the flows.
///
/// Delivery is best-effort: implementations must NOT throw to the caller — a transport failure is
/// logged, not propagated. Callers (e.g. after persisting an event) can await these directly without
/// wrapping them, since a notify failure must never fail the operation that already committed.
/// </summary>
public interface INotificationService
{
    /// <summary>A new event arrived — alert every dispatcher, routed per presence (Mode A/B).</summary>
    Task NotifyEventCreatedAsync(Event evt, CancellationToken ct);

    Task NotifyEventUpdatedAsync(Event evt, CancellationToken ct);

    Task NotifyEventAssignedAsync(Event evt, Guid technicianId, CancellationToken ct);

    /// <summary>Both the outgoing and incoming technician get notified.</summary>
    Task NotifyEventTransferredAsync(Event evt, Guid fromTechnicianId, Guid toTechnicianId, CancellationToken ct);
}
