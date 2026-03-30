using Api.Requests.AuthRequests;
using Application.Contracts.AuthContracts;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    private CookieOptions cookieOptions = new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTime.UtcNow.AddDays(7),
    };

    private void SetRefreshTokenCookie(string token)
    {
        Response.Cookies.Append("refresh_token", token, cookieOptions);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthReadDto>> Login(
        [FromBody] AuthenticationRequest request,
        CancellationToken ct = default
    )
    {
        var dto = request.ToDto();
        var (accessToken, refreshToken) = await authService.LoginAsync(dto, ct);

        SetRefreshTokenCookie(refreshToken);

        return Ok(AuthReadDto.From(accessToken));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthReadDto>> Refresh(CancellationToken ct = default)
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var result = await authService.RefreshAsync(refreshToken, ct);

        Response.Cookies.Delete("refresh_token", cookieOptions);
        SetRefreshTokenCookie(result.refreshToken);

        return Ok(AuthReadDto.From(result.accessToken));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct = default)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(refreshToken))
            return Ok();

        await authService.LogoutAsync(refreshToken, ct);
        Response.Cookies.Delete("refresh_token");

        return Ok();
    }

    [HttpPost("logout/all")]
    [Authorize]
    public async Task<IActionResult> LogoutAll(CancellationToken ct = default)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(refreshToken))
            return Ok();

        await authService.LogoutAllAsync(refreshToken, ct);
        Response.Cookies.Delete("refresh_token", cookieOptions);

        return Ok();
    }
}
