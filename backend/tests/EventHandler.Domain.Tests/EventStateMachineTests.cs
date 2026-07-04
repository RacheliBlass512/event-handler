using EventHandler.Domain.Enums;

namespace EventHandler.Domain.Tests;

/// <summary>
/// Mandatory State Machine unit tests (skeleton-plan.md §10). Named and data-parameterized
/// against the exact transition table now, but [Skip]-attributed since EventStateMachine's
/// bodies are still stubs. Implementing the state machine is just: fill the method bodies
/// below and drop the Skip attributes.
/// </summary>
public class EventStateMachineTests
{
    private const string PendingImplementation = "TODO: fill in once EventStateMachine is implemented.";

    public static TheoryData<EventStatus, EventStatus> AllowedTransitions => new()
    {
        { EventStatus.New, EventStatus.Assigned },
        { EventStatus.New, EventStatus.Canceled },
        { EventStatus.Assigned, EventStatus.InProgress },
        { EventStatus.Assigned, EventStatus.Canceled },
        { EventStatus.InProgress, EventStatus.Resolved },
        { EventStatus.InProgress, EventStatus.Canceled },
        { EventStatus.Resolved, EventStatus.Closed },
        { EventStatus.Resolved, EventStatus.InProgress },
    };

    public static TheoryData<EventStatus, EventStatus> DisallowedTransitions => new()
    {
        { EventStatus.New, EventStatus.InProgress },
        { EventStatus.New, EventStatus.Resolved },
        { EventStatus.New, EventStatus.Closed },
        { EventStatus.Assigned, EventStatus.Resolved },
        { EventStatus.Assigned, EventStatus.Closed },
        { EventStatus.InProgress, EventStatus.Closed },
        { EventStatus.Resolved, EventStatus.Canceled },
        { EventStatus.Closed, EventStatus.New },
        { EventStatus.Canceled, EventStatus.New },
    };

    [Theory(Skip = PendingImplementation)]
    [MemberData(nameof(AllowedTransitions))]
    public void CanTransition_ReturnsTrue_ForAllowedTransition(EventStatus from, EventStatus to)
    {
    }

    [Theory(Skip = PendingImplementation)]
    [MemberData(nameof(DisallowedTransitions))]
    public void CanTransition_ReturnsFalse_ForDisallowedTransition(EventStatus from, EventStatus to)
    {
    }

    [Theory(Skip = PendingImplementation)]
    [MemberData(nameof(AllowedTransitions))]
    public void Transition_AppliesNewStatus_ForAllowedTransition(EventStatus from, EventStatus to)
    {
    }

    [Theory(Skip = PendingImplementation)]
    [MemberData(nameof(DisallowedTransitions))]
    public void Transition_ThrowsInvalidStateTransitionException_ForDisallowedTransition(EventStatus from, EventStatus to)
    {
    }

    [Fact(Skip = PendingImplementation)]
    public void AllowedNext_ReturnsExactExpectedSet_ForEachState()
    {
    }

    [Fact(Skip = PendingImplementation)]
    public void Closed_IsTerminal_AllowedNextIsEmpty()
    {
    }

    [Fact(Skip = PendingImplementation)]
    public void Canceled_IsTerminal_AllowedNextIsEmpty()
    {
    }

    [Fact(Skip = PendingImplementation)]
    public void Canceled_ReachableOnlyFromPreClosedStates()
    {
    }
}
