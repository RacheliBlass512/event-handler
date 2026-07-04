using EventHandler.Domain.Entities;

namespace EventHandler.Server.Application;

public sealed class NotificationService : INotificationService
{
    private readonly IPresenceTracker _presenceTracker;
    private readonly IWebPushSender _webPushSender;

    public NotificationService(IPresenceTracker presenceTracker, IWebPushSender webPushSender)
    {
        _presenceTracker = presenceTracker;
        _webPushSender = webPushSender;
    }

    public Task NotifyEventUpdatedAsync(Event evt, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task NotifyEventAssignedAsync(Event evt, Guid technicianId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task NotifyEventTransferredAsync(Event evt, Guid fromTechnicianId, Guid toTechnicianId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
