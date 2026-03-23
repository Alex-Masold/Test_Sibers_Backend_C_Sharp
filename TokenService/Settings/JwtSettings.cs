namespace TokenService.Settings;

public class JwtSettings
{
    public TimeSpan Expires { get; set; }
    public required string SecretKey { get; set; }
}

