using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHandler.Server.Infrastructure.Persistence;

public sealed class EventRepository : IEventRepository
{
    private readonly AppDbContext _dbContext;

    public EventRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken ct)
        => _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Event>> ListAllAsync(CancellationToken ct)
        => await _dbContext.Events.OrderByDescending(e => e.CreatedAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Event>> ListAssignedToAsync(Guid technicianId, CancellationToken ct)
        => await _dbContext.Events
            .Where(e => e.AssignedTechnicianId == technicianId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);

    public Task AddAsync(Event evt, CancellationToken ct)
    {
        // Synchronous Add (not AddAsync): the Id is generated in Server code, not by the DB,
        // so there's no store-generated-key round-trip to await.
        _dbContext.Events.Add(evt);
        return Task.CompletedTask;
    }
}
