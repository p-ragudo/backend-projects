namespace basic_auth.Auth;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest request, string? UserAgent);
    Task<LoginResult> LoginAsync(LoginRequest request, string? userAgent);
    Task LogoutAsync(string rawToken);
    Task<bool> VerifySession(string rawToken);
}