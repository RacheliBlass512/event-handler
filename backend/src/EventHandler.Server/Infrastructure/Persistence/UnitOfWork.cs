using EventHandler.Domain.Abstractions;

namespace EventHandler.Server.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
