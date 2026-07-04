using System.Security.Claims;
using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;
using EventHandler.Server.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHandler.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/push")]
public sealed class PushController : ControllerBase
{
    private readonly IPushSubscriptionRepository _pushSubscriptionRepository;

    public PushController(IPushSubscriptionRepository pushSubscriptionRepository)
    {
        _pushSubscriptionRepository = pushSubscriptionRepository;
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(PushSubscriptionDto request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _pushSubscriptionRepository.AddAsync(
            new PushSubscription { UserId = userId, Endpoint = request.Endpoint, P256dh = request.P256dh, Auth = request.Auth },
            ct);
        return NoContent();
    }

    [HttpDelete("subscribe")]
    public async Task<IActionResult> Unsubscribe([FromQuery] string endpoint, CancellationToken ct)
    {
        await _pushSubscriptionRepository.RemoveAsync(endpoint, ct);
        return NoContent();
    }
}
