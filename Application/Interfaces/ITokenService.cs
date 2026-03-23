using Domain.Models;

namespace Application.Interfaces;

public interface ITokenService
{
    public string GenerateAccessToken(Employee employee);
    public string GenerateRefreshToken();
}
