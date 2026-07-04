using EventHandler.Contracts;
using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;

namespace EventHandler.Server.Application;

/// <summary>
/// The Event Handler — orchestrates the Domain State Machine, repositories, and
/// notifications. Sole author of <see cref="EventHistoryEntry"/> rows (status and assignee
/// changes alike); see skeleton-plan.md §3.
/// </summary>
public interface IEventService
{
    Task<Event> CreateFromIntakeAsync(IncomingEventDto intake, CancellationToken ct);

    /// <summary>Assigning a New event also drives New→Assigned through the State Machine and
    /// records one combined history row (skeleton-plan.md §4b).</summary>
    Task AssignAsync(Guid eventId, Guid technicianId, Guid dispatcherId, CancellationToken ct);

    /// <summary>Reassignment only — status is untouched, the State Machine is not involved
    /// (skeleton-plan.md §4b).</summary>
    Task TransferAsync(Guid eventId, Guid toTechnicianId, Guid dispatcherId, CancellationToken ct);

    Task ChangeStatusAsync(Guid eventId, EventStatus to, Guid userId, string? note, CancellationToken ct);

    Task CloseAsync(Guid eventId, Guid userId, CancellationToken ct);

    Task AddNoteAsync(Guid eventId, Guid userId, string note, CancellationToken ct);

    /// <summary>Row-level permission enforcement: technician sees only their own assigned
    /// events, dispatcher sees all.</summary>
    Task<IReadOnlyList<Event>> ListForUserAsync(Guid userId, UserRole role, CancellationToken ct);

    Task<IReadOnlyList<EventHistoryEntry>> GetHistoryAsync(Guid eventId, CancellationToken ct);

    /// <summary>Unassigned (New) events a technician can request to pick up.</summary>
    Task<IReadOnlyList<Event>> ListAvailableAsync(CancellationToken ct);
}
