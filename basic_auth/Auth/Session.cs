using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace basic_auth.Auth;

public class Session
{
    public string Id { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string? UserAgent { get; set; }

    public static string GenerateSessionToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
        return WebEncoders.Base64UrlEncode(randomBytes);
    }
}