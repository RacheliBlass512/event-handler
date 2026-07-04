namespace EventHandler.Server.Api.Dtos;

public sealed record PushSubscriptionDto(string Endpoint, string P256dh, string Auth);
