using EventHandler.Server.Api.Dtos;
using EventHandler.Server.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHandler.Server.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request.Username, request.Password, ct);
        return Ok(new LoginResponseDto(result.Token, result.Role, result.DisplayName, result.ExpiresAt));
    }
}
