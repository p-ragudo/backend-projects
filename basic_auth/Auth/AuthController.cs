using Microsoft.AspNetCore.Mvc;

namespace basic_auth.Auth;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        try
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                userAgent = "Unknown";    
            }
            
            var result = await _authService.RegisterAsync(request, userAgent);

            Response.Cookies.Append("session_token", result.SessionToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = result.ExpiresAt,
                Path = "/"
            });

            return StatusCode(StatusCodes.Status201Created, new
            {
                result.UserId,
                result.Email
            });
        }
        catch (InvalidOperationException ex)
        {
            return Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict"
            );
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        try
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                userAgent = "Unknown";    
            }

            var result = await _authService.LoginAsync(request, userAgent);

            Response.Cookies.Append("session_token", result.SessionToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = result.ExpiresAt,
                Path = "/"
            });

            return Ok(new { result.UserId, result.Email });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized"
            );
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue("session_token", out var rawToken) 
            && !string.IsNullOrWhiteSpace(rawToken))
        {
            await _authService.LogoutAsync(rawToken);
        }

        Response.Cookies.Delete("session_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });

        return NoContent();
    }
}