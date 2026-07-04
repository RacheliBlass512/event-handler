using EventHandler.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EventHandler.Server.Infrastructure.Auth;

/// <summary>
/// Thin wrapper over the framework's own hasher (skeleton-plan.md §7) — reuse only, no
/// custom hashing logic. Unlike the rest of Infrastructure, this is fully implemented since
/// it's pure delegation with no design decision left to defer.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<User> _inner = new();

    public string Hash(User user, string password) => _inner.HashPassword(user, password);

    public bool Verify(User user, string password)
        => _inner.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
}
