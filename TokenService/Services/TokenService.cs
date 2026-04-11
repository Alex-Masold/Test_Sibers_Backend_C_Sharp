using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TokenService.Settings;

namespace TokenService.Services;

public class JwtTokenService(IOptions<JwtSettings> jwtSettings, TimeProvider timeProvider)
    : ITokenService
{
    private static readonly JwtSecurityTokenHandler Handler = new();

    public string GenerateAccessToken(Employee employee)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new Claim(ClaimTypes.Role, employee.Role.ToString()),
        };
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey)
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var now = timeProvider.GetUtcNow();
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Value.Issuer,     
            audience: jwtSettings.Value.Audience,
            expires: now.Add(jwtSettings.Value.Expires).UtcDateTime,
            claims: claims,
            signingCredentials: credentials
        );

        return Handler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}
