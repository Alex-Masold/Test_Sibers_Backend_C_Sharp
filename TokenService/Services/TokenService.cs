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

public class JwtTokenService(IOptions<JwtSettings> jwtSettings) : ITokenService
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

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.Add(jwtSettings.Value.Expires),
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
