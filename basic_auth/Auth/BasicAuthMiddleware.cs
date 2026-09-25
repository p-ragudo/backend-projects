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
            path.StartsWith("/scalar"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Cookies.TryGetValue("session_token", out var sessionToken))
        {
            await WriteUnauthorizedResponse(context);
            return;
        }

        var validSession = await _authService.VerifySession(sessionToken);
        if (!validSession)
        {
            await WriteUnauthorizedResponse(context);
            return;
        }

        await _next(context);
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new 
        { 
            error = "Unauthorized", 
            message = "Missing or invalid session token." 
        });
    }
}