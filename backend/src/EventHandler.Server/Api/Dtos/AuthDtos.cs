using EventHandler.Domain.Enums;

namespace EventHandler.Server.Api.Dtos;

public sealed record LoginRequestDto(string Username, string Password);

public sealed record LoginResponseDto(string Token, UserRole Role, string DisplayName, DateTime ExpiresAt);
