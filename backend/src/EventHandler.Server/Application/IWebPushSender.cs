using EventHandler.Domain.Entities;

namespace EventHandler.Server.Application;

/// <summary>Mode B (browser-closed) delivery. Implementation lives in Infrastructure —
/// this is the thin stub interface per this session's decision to skeleton, not build,
/// Web Push.</summary>
public interface IWebPushSender
{
    Task SendAsync(PushSubscription subscription, string title, string body, string? deepLinkUrl, CancellationToken ct);
}
