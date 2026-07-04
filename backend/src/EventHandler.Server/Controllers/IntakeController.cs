using EventHandler.Contracts;
using EventHandler.Server.Application;
using Microsoft.AspNetCore.Mvc;

namespace EventHandler.Server.Controllers;

/// <summary>
/// Agent -> Server intake boundary. Deliberately open (no [Authorize]) in this skeleton pass —
/// this session removed Agent-side source auth and the corresponding agent-auth guard here;
/// both are a documented TODO, not scaffolded in code yet (skeleton-plan.md §7, §12).
/// </summary>
[ApiController]
[Route("api/intake")]
public sealed class IntakeController : ControllerBase
{
    private readonly IEventService _eventService;

    public IntakeController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost("events")]
    public async Task<ActionResult<IntakeResponseDto>> Ingest(IncomingEventDto intake, CancellationToken ct)
    {
        var evt = await _eventService.CreateFromIntakeAsync(intake, ct);
        return Ok(new IntakeResponseDto(evt.Id, Accepted: true));
    }
}
