using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;

namespace EventHandler.Server.Infrastructure.Persistence;

public sealed class EventRepository : IEventRepository
{
    private readonly AppDbContext _dbContext;

    public EventRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Event>> ListAllAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Event>> ListAssignedToAsync(Guid technicianId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Event evt, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
