using EventHandler.Contracts;
using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;
using EventHandler.Domain.StateMachine;

namespace EventHandler.Server.Application;

public sealed class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventStateMachine _stateMachine;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public EventService(
        IEventRepository eventRepository,
        IEventStateMachine stateMachine,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _eventRepository = eventRepository;
        _stateMachine = stateMachine;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Event> CreateFromIntakeAsync(IncomingEventDto intake, CancellationToken ct)
    {
        // The Agent sends Priority as a string (the enum's name); parse it here — the wire
        // contract stays dependency-free of Domain. Unknown/blank values fall back to Normal
        // rather than rejecting the intake (safe fail-open for this trust boundary).
        var priority = Enum.TryParse<Priority>(intake.Priority, ignoreCase: true, out var parsed)
            ? parsed
            : Priority.Normal;

        var evt = new Event
        {
            Id = Guid.NewGuid(),
            Title = intake.Title,
            Description = intake.Description,
            SourceName = intake.SourceName,
            SourceEventId = intake.SourceEventId,
            Location = intake.Location,
            Status = EventStatus.New,
            Priority = priority,
            AssignedTechnicianId = null,
            CreatedAt = intake.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
        };

        await _eventRepository.AddAsync(evt, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return evt;
    }

    public Task AssignAsync(Guid eventId, Guid technicianId, Guid dispatcherId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task TransferAsync(Guid eventId, Guid toTechnicianId, Guid dispatcherId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task ChangeStatusAsync(Guid eventId, EventStatus to, Guid userId, string? note, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync(Guid eventId, Guid userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddNoteAsync(Guid eventId, Guid userId, string note, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Event>> ListForUserAsync(Guid userId, UserRole role, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<EventHistoryEntry>> GetHistoryAsync(Guid eventId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Event>> ListAvailableAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
