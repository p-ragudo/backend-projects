namespace ExpenseTrackerApi.Services.Auth;

public class RefreshToken
{
    public Guid Id { get; set; }
    public required byte[] TokenHash { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime Expires { get; set; }
    public DateTime? Revoked { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
    public bool IsActive => Revoked is null && DateTime.UtcNow < Expires;
}