using Api.Requests.AuthRequests;
using Application.Contracts.AuthContracts;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RedisService.Settings;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    AuthService authService,
    IOptions<RefreshSettings> refreshSettings,
    IWebHostEnvironment environment,
    TimeProvider timeProvider
) : ControllerBase
{
    private const string RefreshTokenCookie = "refresh_token";

    private CookieOptions CreateCookieOptions() =>
        new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = environment.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Strict,
            Expires = timeProvider.GetUtcNow().Add(refreshSettings.Value.Expires),
        };

    private static readonly CookieOptions DeleteCookieOptions = new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.None,
    };

    [HttpPost("login")]
    public async Task<ActionResult<AuthReadDto>> Login(
        [FromBody] AuthenticationRequest request,
        CancellationToken ct = default
    )
    {
        var dto = request.ToDto();
        var result = await authService.LoginAsync(dto, ct);

        Response.Cookies.Append(RefreshTokenCookie, result.refreshToken, CreateCookieOptions());

        return Ok(AuthReadDto.From(result.accessToken));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthReadDto>> Refresh(CancellationToken ct = default)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var result = await authService.RefreshAsync(refreshToken, ct);

        Response.Cookies.Delete(RefreshTokenCookie, DeleteCookieOptions);
        Response.Cookies.Append(RefreshTokenCookie, result.refreshToken, CreateCookieOptions());

        return Ok(AuthReadDto.From(result.accessToken));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct = default)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];

        if (string.IsNullOrEmpty(refreshToken))
            return Ok();

        await authService.LogoutAsync(refreshToken, ct);
        Response.Cookies.Delete(RefreshTokenCookie, DeleteCookieOptions);

        return Ok();
    }

    [HttpPost("logout/all")]
    public async Task<IActionResult> LogoutAll(CancellationToken ct = default)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];

        if (string.IsNullOrEmpty(refreshToken))
            return Ok();

        await authService.LogoutAllAsync(refreshToken, ct);
        Response.Cookies.Delete(RefreshTokenCookie, DeleteCookieOptions);

        return Ok();
    }
}
