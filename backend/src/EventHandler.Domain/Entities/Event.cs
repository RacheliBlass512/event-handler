using EventHandler.Domain.Enums;

namespace EventHandler.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Denormalized provenance copied from the intake payload — no Source table/FK.</summary>
    public string SourceName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
    public EventStatus Status { get; set; }
    public Priority Priority { get; set; }
    public Guid? AssignedTechnicianId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<EventHistoryEntry> History { get; set; } = new List<EventHistoryEntry>();
}
