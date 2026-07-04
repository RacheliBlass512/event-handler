using EventHandler.Domain.Entities;

namespace EventHandler.Domain.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct);

    /// <summary>Backs the dispatcher's technician-status dashboard.</summary>
    Task<IReadOnlyList<User>> ListTechniciansAsync(CancellationToken ct);
}
