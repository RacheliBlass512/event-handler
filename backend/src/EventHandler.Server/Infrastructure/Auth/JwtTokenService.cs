using EventHandler.Domain.Entities;
using EventHandler.Server.Application;

namespace EventHandler.Server.Infrastructure.Auth;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        throw new NotImplementedException();
    }
}
