namespace basic_auth.Auth;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BasicAuthMiddleware> _logger;

    public BasicAuthMiddleware(
        RequestDelegate next,
        ILogger<BasicAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuthService _authService)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        if (
            path == "/" || 
            path.StartsWith("/auth/register") || 
            path.StartsWith("/auth/login") || 
            path.StartsWith("scalar"))
        {
            await _next(context);
        }

        if (!context.Request.Cookies.TryGetValue("session_token", out var sessionToken))
        {
            return;
        }

        var validSession = await _authService.VerifySession(sessionToken);
        if (!validSession)
        {
            return;
        }

        await _next(context);
    }
}