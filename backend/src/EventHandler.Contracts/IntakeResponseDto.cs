namespace EventHandler.Contracts;

public sealed record IntakeResponseDto(Guid ServerEventId, bool Accepted);
