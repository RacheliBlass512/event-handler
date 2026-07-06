using EventHandler.Domain.Enums;

namespace EventHandler.Server.Api.Dtos;

public sealed record EventDto(
    Guid Id,
    string Title,
    string Description,
    string SourceName,
    string SourceEventId,
    string Location,
    EventStatus Status,
    Priority Priority,
    Guid? AssignedTechnicianId,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record EventHistoryDto(
    EventChangeKind Kind,
    EventStatus? FromStatus,
    EventStatus? ToStatus,
    Guid? FromAssigneeId,
    Guid? ToAssigneeId,
    string ChangedByDisplayName,
    DateTime ChangedAt,
    string? Note);

public sealed record AssignRequestDto(Guid TechnicianId);

public sealed record TransferRequestDto(Guid ToTechnicianId);

public sealed record StatusChangeRequestDto(EventStatus To, string? Note);

public sealed record NoteDto(string Note);
