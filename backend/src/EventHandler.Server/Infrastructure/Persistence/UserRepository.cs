using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventHandler.Server.Infrastructure.Persistence;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        => _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct)
        => _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<IReadOnlyList<User>> ListTechniciansAsync(CancellationToken ct)
        => await _dbContext.Users.Where(u => u.Role == UserRole.Technician).ToListAsync(ct);

    public async Task<IReadOnlyList<User>> ListDispatchersAsync(CancellationToken ct)
        => await _dbContext.Users.Where(u => u.Role == UserRole.Dispatcher).ToListAsync(ct);
}
