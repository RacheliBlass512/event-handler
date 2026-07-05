using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;
using EventHandler.Server.Infrastructure.Auth;
using EventHandler.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventHandler.Server.Infrastructure;

/// <summary>Seeds a dispatcher + a couple of technicians (skeleton-plan.md §5) so the app is
/// login-able out of the box. Dev-only seed credentials — see README.</summary>
public sealed class DbSeeder
{
    private const string DevPassword = "Passw0rd!";

    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public DbSeeder(AppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _dbContext.Users.AnyAsync(ct))
        {
            return;
        }

        var users = new[]
        {
            new User { Id = Guid.NewGuid(), Username = "dispatcher", Role = UserRole.Dispatcher, DisplayName = "Dana Dispatcher" },
            new User { Id = Guid.NewGuid(), Username = "tech1", Role = UserRole.Technician, DisplayName = "Tom Technician" },
            new User { Id = Guid.NewGuid(), Username = "tech2", Role = UserRole.Technician, DisplayName = "Tara Technician" },
        };

        foreach (var user in users)
        {
            user.PasswordHash = _passwordHasher.Hash(user, DevPassword);
        }

        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync(ct);
    }
}
