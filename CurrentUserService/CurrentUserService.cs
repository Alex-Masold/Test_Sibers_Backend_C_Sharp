using System.Security.Claims;
using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace CurrentUserService.Service;

public class UserService : ICurrentUserService
{
    private readonly Lazy<int> _userId;
    private readonly Lazy<Role> _role;

    public UserService(IHttpContextAccessor accessor)
    {
        _userId = new Lazy<int>(() =>
        {
            var claim = accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is { Value: var v } && int.TryParse(v, out var id))
                return id;
            throw new AuthenticationException("User not authenticated");
        });

        _role = new Lazy<Role>(() =>
        {
            var claim = accessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
            if (claim is { Value: var v } && Enum.TryParse<Role>(v, out var role))
                return role;
            throw new AuthenticationException("User not authenticated");
        });
    }

    public int UserId => _userId.Value;
    public Role Role => _role.Value;

    public bool IsDirector => Role == Role.Director;
}
