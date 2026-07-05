using EventHandler.Domain.Abstractions;
using EventHandler.Server.Infrastructure.Auth;

namespace EventHandler.Server.Application;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResult?> LoginAsync(string username, string password, CancellationToken ct)
    {
        var user = await _userRepository.GetByUsernameAsync(username, ct);
        if (user is null || !_passwordHasher.Verify(user, password))
        {
            return null;
        }

        var (token, expiresAt) = _jwtTokenService.GenerateToken(user);
        return new LoginResult(token, expiresAt, user.Role, user.DisplayName);
    }
}
