using EventHandler.Domain.Abstractions;

namespace EventHandler.Server.Application;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public Task<LoginResult> LoginAsync(string username, string password, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
