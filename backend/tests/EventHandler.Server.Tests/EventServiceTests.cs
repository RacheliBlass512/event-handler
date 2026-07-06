using EventHandler.Contracts;
using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;
using EventHandler.Domain.StateMachine;
using EventHandler.Server.Application;

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

    private static (EventService Sut, FakeEventRepository Repo, FakeUnitOfWork Uow) BuildSut()
    {
        var repo = new FakeEventRepository();
        var uow = new FakeUnitOfWork();
        var sut = new EventService(repo, new ThrowingStateMachine(), uow, new ThrowingNotificationService());
        return (sut, repo, uow);
    }

    [Fact]
    public async Task CreateFromIntakeAsync_MapsFieldsAndPersists()
    {
        var (sut, repo, uow) = BuildSut();
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
    }

    [Theory]
    [InlineData("High", Priority.High)]
    [InlineData("high", Priority.High)]
    [InlineData("Critical", Priority.Critical)]
    public async Task CreateFromIntakeAsync_ParsesPriority(string wire, Priority expected)
    {
        var (sut, _, _) = BuildSut();

        var result = await sut.CreateFromIntakeAsync(Intake(wire), CancellationToken.None);

        Assert.Equal(expected, result.Priority);
    }

    [Theory]
    [InlineData("")]
    [InlineData("gibberish")]
    public async Task CreateFromIntakeAsync_UnknownPriority_FallsBackToNormal(string wire)
    {
        var (sut, _, _) = BuildSut();

        var result = await sut.CreateFromIntakeAsync(Intake(wire), CancellationToken.None);

        Assert.Equal(Priority.Normal, result.Priority);
    }

    private sealed class FakeEventRepository : IEventRepository
    {
        public Event? Added { get; private set; }

        public Task AddAsync(Event evt, CancellationToken ct)
        {
            Added = evt;
            return Task.CompletedTask;
        }

        public Task<Event?> GetByIdAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<IReadOnlyList<Event>> ListAllAsync(CancellationToken ct) => throw new NotSupportedException();
        public Task<IReadOnlyList<Event>> ListAssignedToAsync(Guid technicianId, CancellationToken ct) => throw new NotSupportedException();
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

    // Neither is exercised by CreateFromIntakeAsync — throw if the flow ever touches them.
    private sealed class ThrowingStateMachine : IEventStateMachine
    {
        public bool CanTransition(EventStatus from, EventStatus to) => throw new NotSupportedException();
        public IReadOnlyCollection<EventStatus> AllowedNext(EventStatus from) => throw new NotSupportedException();
        public void Transition(Event evt, EventStatus to) => throw new NotSupportedException();
    }

    private sealed class ThrowingNotificationService : INotificationService
    {
        public Task NotifyEventUpdatedAsync(Event evt, CancellationToken ct) => throw new NotSupportedException();
        public Task NotifyEventAssignedAsync(Event evt, Guid technicianId, CancellationToken ct) => throw new NotSupportedException();
        public Task NotifyEventTransferredAsync(Event evt, Guid fromTechnicianId, Guid toTechnicianId, CancellationToken ct) => throw new NotSupportedException();
    }
}
