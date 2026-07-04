using EventHandler.Domain.Entities;
using EventHandler.Server.Application;

namespace EventHandler.Server.Infrastructure.Notifications;

/// <summary>Mode-B stub — VAPID keys come from config; no real Web Push delivery yet
/// (skeleton-plan.md §7).</summary>
public sealed class WebPushSender : IWebPushSender
{
    private readonly IConfiguration _configuration;

    public WebPushSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task SendAsync(PushSubscription subscription, string title, string body, string? deepLinkUrl, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
