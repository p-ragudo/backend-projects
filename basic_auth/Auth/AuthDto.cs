namespace basic_auth.Auth;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record UserResult(
    bool IsSuccess,
    string? Token = null,
    Guid? Id = null,
    string? Email = null,
    string? ErrorMessage = null
);