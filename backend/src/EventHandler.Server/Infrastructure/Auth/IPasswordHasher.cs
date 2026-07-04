using EventHandler.Domain.Entities;

namespace EventHandler.Server.Infrastructure.Auth;

public interface IPasswordHasher
{
    string Hash(User user, string password);
    bool Verify(User user, string password);
}
