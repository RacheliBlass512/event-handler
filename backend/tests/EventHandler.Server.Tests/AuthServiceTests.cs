using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;
using EventHandler.Server.Application;
using EventHandler.Server.Infrastructure.Auth;

namespace EventHandler.Server.Tests;

public class AuthServiceTests
{
    private static readonly User Dispatcher = new()
    {
        Id = Guid.NewGuid(),
        Username = "dispatcher",
        PasswordHash = "correct-hash",
        Role = UserRole.Dispatcher,
        DisplayName = "Dana Dispatcher",
    };

    [Fact]
    public async Task LoginAsync_ReturnsToken_ForValidCredentials()
    {
        var sut = new AuthService(
            new FakeUserRepository(Dispatcher),
            new FakePasswordHasher(expectedPassword: "correct-password"),
            new FakeJwtTokenService());

        var result = await sut.LoginAsync("dispatcher", "correct-password", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("fake-token", result.Token);
        Assert.Equal(UserRole.Dispatcher, result.Role);
        Assert.Equal("Dana Dispatcher", result.DisplayName);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_ForWrongPassword()
    {
        var sut = new AuthService(
            new FakeUserRepository(Dispatcher),
            new FakePasswordHasher(expectedPassword: "correct-password"),
            new FakeJwtTokenService());

        var result = await sut.LoginAsync("dispatcher", "wrong-password", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_ForUnknownUsername()
    {
        var sut = new AuthService(
            new FakeUserRepository(user: null),
            new FakePasswordHasher(expectedPassword: "correct-password"),
            new FakeJwtTokenService());

        var result = await sut.LoginAsync("nobody", "correct-password", CancellationToken.None);

        Assert.Null(result);
    }

    private sealed class FakeUserRepository(User? user) : IUserRepository
    {
        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult(user);
        public Task<User?> GetByUsernameAsync(string username, CancellationToken ct) => Task.FromResult(user);
        public Task<IReadOnlyList<User>> ListTechniciansAsync(CancellationToken ct)
            => Task.FromResult<IReadOnlyList<User>>([]);
        public Task<IReadOnlyList<User>> ListDispatchersAsync(CancellationToken ct)
            => Task.FromResult<IReadOnlyList<User>>([]);
    }

    private sealed class FakePasswordHasher(string expectedPassword) : IPasswordHasher
    {
        public string Hash(User user, string password) => "hash";
        public bool Verify(User user, string password) => password == expectedPassword;
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public (string Token, DateTime ExpiresAt) GenerateToken(User user) => ("fake-token", DateTime.UtcNow.AddHours(1));
    }
}
