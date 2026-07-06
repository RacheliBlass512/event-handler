using EventHandler.Contracts;
using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;
using EventHandler.Domain.StateMachine;
using EventHandler.Server.Api.Hubs;
using EventHandler.Server.Application;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;

namespace EventHandler.Server.Tests;

public class EventServiceTests
{
    private static IncomingEventDto Intake(string priority = "High") => new(
        SourceName: "sim-sensor",
        SourceEventId: "sim-001",
        Title: "Smoke detected",
        Description: "Smoke above threshold",
        Location: "Hangar 3",
        CreatedAt: new DateTime(2026, 7, 6, 10, 0, 0, DateTimeKind.Utc),
        Priority: priority);

    private static (EventService Sut, FakeEventRepository Repo, FakeUnitOfWork Uow, RecordingNotificationService Notifier) BuildSut(
        IReadOnlyList<Event>? seed = null)
    {
        var repo = new FakeEventRepository(seed);
        var uow = new FakeUnitOfWork();
        var notifier = new RecordingNotificationService();
        var sut = new EventService(repo, new ThrowingStateMachine(), uow, notifier);
        return (sut, repo, uow, notifier);
    }

    [Fact]
    public async Task CreateFromIntakeAsync_MapsFieldsAndPersists()
    {
        var (sut, repo, uow, notifier) = BuildSut();
        var intake = Intake();

        var result = await sut.CreateFromIntakeAsync(intake, CancellationToken.None);

        Assert.Equal(EventStatus.New, result.Status);
        Assert.Null(result.AssignedTechnicianId);
        Assert.Equal(intake.Title, result.Title);
        Assert.Equal(intake.Description, result.Description);
        Assert.Equal(intake.SourceName, result.SourceName);
        Assert.Equal(intake.SourceEventId, result.SourceEventId);
        Assert.Equal(intake.Location, result.Location);
        Assert.Equal(intake.CreatedAt, result.CreatedAt);
        Assert.NotEqual(Guid.Empty, result.Id);

        Assert.Same(result, repo.Added);
        Assert.True(uow.SaveCalled);
        // Persist then notify: the same event is handed to the real-time path.
        Assert.Same(result, notifier.CreatedEvent);
    }

    [Theory]
    [InlineData("High", Priority.High)]
    [InlineData("high", Priority.High)]
    [InlineData("Critical", Priority.Critical)]
    public async Task CreateFromIntakeAsync_ParsesPriority(string wire, Priority expected)
    {
        var (sut, _, _, _) = BuildSut();

        var result = await sut.CreateFromIntakeAsync(Intake(wire), CancellationToken.None);

        Assert.Equal(expected, result.Priority);
    }

    [Theory]
    [InlineData("")]
    [InlineData("gibberish")]
    public async Task CreateFromIntakeAsync_UnknownPriority_FallsBackToNormal(string wire)
    {
        var (sut, _, _, _) = BuildSut();

        var result = await sut.CreateFromIntakeAsync(Intake(wire), CancellationToken.None);

        Assert.Equal(Priority.Normal, result.Priority);
    }

    [Fact]
    public async Task ListForUserAsync_Dispatcher_SeesAllEvents()
    {
        var mine = EventFor(Guid.NewGuid());
        var other = EventFor(Guid.NewGuid());
        var (sut, _, _, _) = BuildSut(seed: new[] { mine, other });

        var result = await sut.ListForUserAsync(Guid.NewGuid(), UserRole.Dispatcher, CancellationToken.None);

        Assert.Equal(new[] { mine, other }, result);
    }

    [Fact]
    public async Task ListForUserAsync_Technician_SeesOnlyOwnAssignedEvents()
    {
        var tech = Guid.NewGuid();
        var mine = EventFor(tech);
        var other = EventFor(Guid.NewGuid());
        var (sut, _, _, _) = BuildSut(seed: new[] { mine, other });

        var result = await sut.ListForUserAsync(tech, UserRole.Technician, CancellationToken.None);

        Assert.Equal(new[] { mine }, result);
    }

    private static Event EventFor(Guid? assignedTechnicianId) => new()
    {
        Id = Guid.NewGuid(),
        Title = "e",
        Status = EventStatus.New,
        Priority = Priority.Normal,
        AssignedTechnicianId = assignedTechnicianId,
    };

    private sealed class FakeEventRepository : IEventRepository
    {
        private readonly IReadOnlyList<Event> _seed;
        public FakeEventRepository(IReadOnlyList<Event>? seed) => _seed = seed ?? Array.Empty<Event>();

        public Event? Added { get; private set; }

        public Task AddAsync(Event evt, CancellationToken ct)
        {
            Added = evt;
            return Task.CompletedTask;
        }

        public Task<Event?> GetByIdAsync(Guid id, CancellationToken ct)
            => Task.FromResult(_seed.FirstOrDefault(e => e.Id == id));

        public Task<IReadOnlyList<Event>> ListAllAsync(CancellationToken ct)
            => Task.FromResult(_seed);

        public Task<IReadOnlyList<Event>> ListAssignedToAsync(Guid technicianId, CancellationToken ct)
            => Task.FromResult<IReadOnlyList<Event>>(
                _seed.Where(e => e.AssignedTechnicianId == technicianId).ToList());
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public bool SaveCalled { get; private set; }

        public Task SaveChangesAsync(CancellationToken ct)
        {
            SaveCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingStateMachine : IEventStateMachine
    {
        public bool CanTransition(EventStatus from, EventStatus to) => throw new NotSupportedException();
        public IReadOnlyCollection<EventStatus> AllowedNext(EventStatus from) => throw new NotSupportedException();
        public void Transition(Event evt, EventStatus to) => throw new NotSupportedException();
    }

    // Records the created-event notification so tests can assert save-then-notify wiring.
    // The other methods are off the create flow — throw if the flow ever touches them.
    private sealed class RecordingNotificationService : INotificationService
    {
        public Event? CreatedEvent { get; private set; }

        public Task NotifyEventCreatedAsync(Event evt, CancellationToken ct)
        {
            CreatedEvent = evt;
            return Task.CompletedTask;
        }

        public Task NotifyEventUpdatedAsync(Event evt, CancellationToken ct) => throw new NotSupportedException();
        public Task NotifyEventAssignedAsync(Event evt, Guid technicianId, CancellationToken ct) => throw new NotSupportedException();
        public Task NotifyEventTransferredAsync(Event evt, Guid fromTechnicianId, Guid toTechnicianId, CancellationToken ct) => throw new NotSupportedException();
    }
}

public class NotificationServiceTests
{
    [Fact]
    public async Task NotifyEventCreatedAsync_PushesToConnectedDispatchersOnly()
    {
        var online = Dispatcher();
        var offline = Dispatcher();
        var hub = new FakeHubContext();
        var sut = Build(hub, new FakePresenceTracker(online.Id), online, offline); // only 'online' is connected

        await sut.NotifyEventCreatedAsync(new Event { Id = Guid.NewGuid(), Title = "e" }, CancellationToken.None);

        // Mode A fires for the connected dispatcher; the offline one takes the Mode B path (no SignalR frame).
        Assert.Contains(($"user:{online.Id}", "EventCreated"), hub.Clients.Sends);
        Assert.DoesNotContain($"user:{offline.Id}", hub.Clients.Sends.Select(s => s.Group));
    }

    [Fact]
    public async Task NotifyEventCreatedAsync_AllDispatchersOffline_SendsNothingAndCompletes()
    {
        var hub = new FakeHubContext();
        var sut = Build(hub, new FakePresenceTracker(/* none connected */), Dispatcher(), Dispatcher());

        await sut.NotifyEventCreatedAsync(new Event { Id = Guid.NewGuid(), Title = "e" }, CancellationToken.None);

        // Everyone offline → no SignalR frames, and the Mode B guard means the call still completes.
        Assert.Empty(hub.Clients.Sends);
    }

    private static NotificationService Build(FakeHubContext hub, FakePresenceTracker presence, params User[] dispatchers)
        => new(
            hub,
            new FakeUserRepository(dispatchers),
            presence,
            new EmptyPushSubscriptionRepository(),   // offline branch finds no subscriptions → no web-push attempted
            new ThrowingWebPushSender(),             // ...so this is never reached
            NullLogger<NotificationService>.Instance);

    private static User Dispatcher() => new() { Id = Guid.NewGuid(), Role = UserRole.Dispatcher };

    private sealed class FakePresenceTracker : IPresenceTracker
    {
        private readonly HashSet<Guid> _connected;
        public FakePresenceTracker(params Guid[] connected) => _connected = new HashSet<Guid>(connected);

        public bool IsConnected(Guid userId) => _connected.Contains(userId);
        public void MarkConnected(Guid userId, string connectionId) { }
        public void MarkDisconnected(Guid userId, string connectionId) { }
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly IReadOnlyList<User> _dispatchers;
        public FakeUserRepository(params User[] dispatchers) => _dispatchers = dispatchers;

        public Task<IReadOnlyList<User>> ListDispatchersAsync(CancellationToken ct) => Task.FromResult(_dispatchers);

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<User?> GetByUsernameAsync(string username, CancellationToken ct) => throw new NotSupportedException();
        public Task<IReadOnlyList<User>> ListTechniciansAsync(CancellationToken ct) => throw new NotSupportedException();
    }

    private sealed class EmptyPushSubscriptionRepository : IPushSubscriptionRepository
    {
        public Task<IReadOnlyList<PushSubscription>> GetForUserAsync(Guid userId, CancellationToken ct)
            => Task.FromResult<IReadOnlyList<PushSubscription>>([]);
        public Task AddAsync(PushSubscription subscription, CancellationToken ct) => throw new NotSupportedException();
        public Task RemoveAsync(string endpoint, CancellationToken ct) => throw new NotSupportedException();
    }

    private sealed class ThrowingWebPushSender : IWebPushSender
    {
        // Mode B is log-only this iteration; a call here means the routing branched wrong.
        public Task SendAsync(PushSubscription subscription, string title, string body, string? deepLinkUrl, CancellationToken ct)
            => throw new NotSupportedException();
    }

    private sealed class FakeHubContext : IHubContext<EventsHub>
    {
        public FakeHubClients Clients { get; } = new();
        IHubClients IHubContext<EventsHub>.Clients => Clients;
        public IGroupManager Groups => throw new NotSupportedException();
    }

    private sealed class FakeHubClients : IHubClients
    {
        public List<(string Group, string Method)> Sends { get; } = new();

        public IClientProxy Group(string groupName) => new FakeClientProxy(groupName, Sends);

        public IClientProxy All => throw new NotSupportedException();
        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => throw new NotSupportedException();
        public IClientProxy Client(string connectionId) => throw new NotSupportedException();
        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => throw new NotSupportedException();
        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => throw new NotSupportedException();
        public IClientProxy Groups(IReadOnlyList<string> groupNames) => throw new NotSupportedException();
        public IClientProxy User(string userId) => throw new NotSupportedException();
        public IClientProxy Users(IReadOnlyList<string> userIds) => throw new NotSupportedException();
    }

    private sealed class FakeClientProxy : IClientProxy
    {
        private readonly string _group;
        private readonly List<(string, string)> _sink;
        public FakeClientProxy(string group, List<(string, string)> sink) => (_group, _sink) = (group, sink);

        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        {
            _sink.Add((_group, method));
            return Task.CompletedTask;
        }
    }
}
