using EventHandler.Domain.Enums;

namespace EventHandler.Server.Application;

public sealed record LoginResult(string Token, DateTime ExpiresAt, UserRole Role, string DisplayName);
