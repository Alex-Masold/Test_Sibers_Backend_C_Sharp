namespace TokenService.Settings;

public class JwtSettings
{
    public TimeSpan Expires { get; set; }
    public required string SecretKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}
