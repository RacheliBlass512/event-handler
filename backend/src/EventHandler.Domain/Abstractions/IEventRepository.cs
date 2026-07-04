using EventHandler.Domain.Entities;

namespace EventHandler.Domain.Abstractions;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Dispatcher view — all events.</summary>
    Task<IReadOnlyList<Event>> ListAllAsync(CancellationToken ct);

    /// <summary>Technician view — row-level filter enforced here, not just in the UI.</summary>
    Task<IReadOnlyList<Event>> ListAssignedToAsync(Guid technicianId, CancellationToken ct);

    Task AddAsync(Event evt, CancellationToken ct);
}
