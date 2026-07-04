using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;

namespace EventHandler.Domain.StateMachine;

/// <summary>
/// Skeleton stub. Bodies are unimplemented; the transition table below (skeleton-plan.md §2)
/// documents the intended shape and is what EventHandler.Domain.Tests pins down once this
/// is implemented.
///
/// Transition table:
///   New        -> Assigned, Canceled
///   Assigned   -> InProgress, Canceled
///   InProgress -> Resolved, Canceled
///   Resolved   -> Closed, InProgress (reopen)
///   Closed     -> (terminal)
///   Canceled   -> (terminal)
/// </summary>
public sealed class EventStateMachine : IEventStateMachine
{
    public bool CanTransition(EventStatus from, EventStatus to)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<EventStatus> AllowedNext(EventStatus from)
    {
        throw new NotImplementedException();
    }

    public void Transition(Event evt, EventStatus to)
    {
        throw new NotImplementedException();
    }
}
