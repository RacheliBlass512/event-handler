using EventHandler.Server.Infrastructure.Auth;
using EventHandler.Server.Infrastructure.Persistence;

namespace EventHandler.Server.Infrastructure;

/// <summary>Seeds a dispatcher + a couple of technicians (skeleton-plan.md §5). Stub — no
/// migration exists yet for this to seed against.</summary>
public sealed class DbSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public DbSeeder(AppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public Task SeedAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
