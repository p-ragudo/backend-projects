namespace ExpenseTrackerApi.Services.Auth;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
    public DateTime? Revoked { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public bool IsActive => Revoked is null && DateTime.UtcNow < Expires;
}