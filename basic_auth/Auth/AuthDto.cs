namespace basic_auth.Auth;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record RegisterResult(
    string? SessionToken = null,
    Guid? Id = null,
    string? Email = null,
    DateTime? ExpiresAt = null
);

public record LoginResult(
    string SessionToken,
    Guid UserId,
    string Email,
    DateTime ExpiresAt
);