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

    public Task<Event> CreateFromIntakeAsync(IncomingEventDto intake, CancellationToken ct)
    {
        throw new NotImplementedException();
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
