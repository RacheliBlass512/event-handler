namespace EventHandler.Server.Application;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string username, string password, CancellationToken ct);
}
