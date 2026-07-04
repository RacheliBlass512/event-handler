using System.Security.Claims;
using EventHandler.Domain.Enums;
using EventHandler.Server.Api.Dtos;
using EventHandler.Server.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHandler.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/events")]
public sealed class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>Role-filtered: dispatcher sees all, technician sees only assigned
    /// (row-level enforcement lives in EventService, not here).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> List(CancellationToken ct)
    {
        var events = await _eventService.ListForUserAsync(GetUserId(), GetRole(), ct);
        return Ok(events.Select(e => e.ToDto()).ToList());
    }

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> ListAvailable(CancellationToken ct)
    {
        var events = await _eventService.ListAvailableAsync(ct);
        return Ok(events.Select(e => e.ToDto()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventDto>> GetById(Guid id, CancellationToken ct)
    {
        var events = await _eventService.ListForUserAsync(GetUserId(), GetRole(), ct);
        var evt = events.FirstOrDefault(e => e.Id == id);
        return evt is null ? NotFound() : Ok(evt.ToDto());
    }

    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<IReadOnlyList<EventHistoryDto>>> GetHistory(Guid id, CancellationToken ct)
    {
        var history = await _eventService.GetHistoryAsync(id, ct);
        return Ok(history.Select(h => h.ToDto()).ToList());
    }

    [Authorize(Roles = "Dispatcher")]
    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, AssignRequestDto request, CancellationToken ct)
    {
        await _eventService.AssignAsync(id, request.TechnicianId, GetUserId(), ct);
        return NoContent();
    }

    [Authorize(Roles = "Dispatcher")]
    [HttpPost("{id:guid}/transfer")]
    public async Task<IActionResult> Transfer(Guid id, TransferRequestDto request, CancellationToken ct)
    {
        await _eventService.TransferAsync(id, request.ToTechnicianId, GetUserId(), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, StatusChangeRequestDto request, CancellationToken ct)
    {
        await _eventService.ChangeStatusAsync(id, request.To, GetUserId(), request.Note, ct);
        return NoContent();
    }

    [Authorize(Roles = "Dispatcher")]
    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, CancellationToken ct)
    {
        await _eventService.CloseAsync(id, GetUserId(), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddNote(Guid id, NoteDto request, CancellationToken ct)
    {
        await _eventService.AddNoteAsync(id, GetUserId(), request.Note, ct);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : throw new InvalidOperationException("Missing user id claim.");
    }

    private UserRole GetRole()
    {
        var value = User.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse<UserRole>(value, out var role) ? role : throw new InvalidOperationException("Missing role claim.");
    }
}
