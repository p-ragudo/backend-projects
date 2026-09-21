namespace basic_auth.Auth;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record UserResult(Guid? Id, string Email);