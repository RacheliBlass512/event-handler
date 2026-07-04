using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;

namespace EventHandler.Server.Infrastructure.Persistence;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<User>> ListTechniciansAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
