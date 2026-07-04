namespace EventHandler.Domain.Enums;

/// <summary>
/// Discriminates rows in the unified <see cref="Entities.EventHistoryEntry"/> timeline.
/// Assigned vs Transferred is derivable from FromAssigneeId == null, but kept distinct for
/// clearer UI/notification text (skeleton-plan.md §5).
/// </summary>
public enum EventChangeKind
{
    StatusChanged,
    Assigned,
    Transferred,
    NoteAdded
}
