using EventHandler.Domain.Enums;

namespace EventHandler.Domain.StateMachine;

public sealed class InvalidStateTransitionException : Exception
{
    public EventStatus From { get; }
    public EventStatus To { get; }

    public InvalidStateTransitionException(EventStatus from, EventStatus to)
        : base($"Cannot transition event from '{from}' to '{to}'.")
    {
        From = from;
        To = to;
    }
}
