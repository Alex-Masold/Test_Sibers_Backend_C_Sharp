using Domain.Models;

namespace Application.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    Role Role { get; }
    bool IsDirector { get; }
}

