using EventHandler.Domain.Entities;

namespace EventHandler.Server.Application;

/// <summary>Implementation lives in Infrastructure.</summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
