using EventHandler.Domain.Enums;

namespace EventHandler.Domain.Entities;

/// <summary>
/// One unified timeline entry for BOTH status and assignee changes (they are orthogonal —
/// either can change without the other). Status-only rows leave assignee fields null and
/// vice versa; a first assignment fills both (skeleton-plan.md §5).
/// </summary>
public class EventHistoryEntry
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public EventChangeKind Kind { get; set; }

    public EventStatus? FromStatus { get; set; }
    public EventStatus? ToStatus { get; set; }

    public Guid? FromAssigneeId { get; set; }
    public Guid? ToAssigneeId { get; set; }

    public Guid ChangedByUserId { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Note { get; set; }
}
