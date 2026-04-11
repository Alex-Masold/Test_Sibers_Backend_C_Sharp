using Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace PasswordService;

public class  IdentityPasswordService : IPasswordService
{
    private static readonly PasswordHasher<object> Hasher = new();

    public string HashPassword(string password)
    {
        return Hasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string hashededPassword, string providedPassword)
    {
        var result = Hasher.VerifyHashedPassword(null!, hashededPassword, providedPassword);
        return result != PasswordVerificationResult.Failed;
    }
}
