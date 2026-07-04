using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;

namespace EventHandler.Domain.StateMachine;

/// <summary>
/// Owns only the event status transition table. Deliberately narrow — see
/// skeleton-plan.md §3 for the full State-Machine-vs-EventService boundary: this never
/// touches history, assignee, persistence, or notifications. That's EventService's job.
/// </summary>
public interface IEventStateMachine
{
    bool CanTransition(EventStatus from, EventStatus to);

    IReadOnlyCollection<EventStatus> AllowedNext(EventStatus from);

    /// <summary>Validates via the transition table and sets <paramref name="evt"/>.Status,
    /// or throws <see cref="InvalidStateTransitionException"/>.</summary>
    void Transition(Event evt, EventStatus to);
}
