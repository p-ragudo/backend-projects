namespace basic_auth.Auth;

public class Session
{
    public string Id { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime Expiresat { get; set; }
    public string? UserAgent { get; set; }
}