using EventHandler.Domain.Entities;

namespace EventHandler.Server.Api.Dtos;

public static class DtoMappingExtensions
{
    public static EventDto ToDto(this Event evt) => new(
        evt.Id, evt.Title, evt.Description, evt.SourceName, evt.SourceEventId, evt.Location,
        evt.Status, evt.Priority, evt.AssignedTechnicianId, evt.CreatedAt, evt.UpdatedAt);

    public static EventHistoryDto ToDto(this EventHistoryEntry entry) => new(
        entry.Kind, entry.FromStatus, entry.ToStatus, entry.FromAssigneeId, entry.ToAssigneeId,
        ChangedByDisplayName: string.Empty, // TODO: resolve via User lookup once EventService is implemented.
        entry.ChangedAt, entry.Note);
}
