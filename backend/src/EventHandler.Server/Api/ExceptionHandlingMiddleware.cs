namespace EventHandler.Server.Api;

/// <summary>
/// Stub — passes requests through unchanged. Mapping domain exceptions (e.g.
/// InvalidStateTransitionException) to HTTP status codes is a policy decision deferred to the
/// implementation step, not scaffolded in this skeleton pass.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: catch domain/application exceptions here and translate to problem-details responses.
        await _next(context);
    }
}
