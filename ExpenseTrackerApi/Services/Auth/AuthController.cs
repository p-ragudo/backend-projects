using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ExpenseTrackerApi.Services.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(
    AuthService _authService, 
    IConfiguration _config) : ControllerBase
{
    private readonly int accessTokenExpirationMinutes = _config.GetValue<int>("ExpenseTrackerApi:Jwt:AccessTokenExpirationMinutes");
    private readonly int refreshTokenExpirationMinutes = _config.GetValue<int>("ExpenseTrackerApi:Jwt:RefreshTokenExpirationMinutes");

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);

            if (result.IsSuccess)
            {
                CookieAppendToken(Response.Cookies, "accessToken", result.AccessToken, accessTokenExpirationMinutes);
                CookieAppendToken(Response.Cookies, "refreshToken", result.RefreshToken, refreshTokenExpirationMinutes);

                return Ok();
            }
            else
            {
                return Unauthorized(new
                {
                    message = "Email or username already exists."
                });
            }
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An internal server error has occured."
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);

            if (result.IsSuccess)
            {
                CookieAppendToken(Response.Cookies, "accessToken", result.AccessToken, accessTokenExpirationMinutes);
                CookieAppendToken(Response.Cookies, "refreshToken", result.RefreshToken, refreshTokenExpirationMinutes);

                return Ok();
            }
            else
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An internal server error has occured."
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _authService.RefreshAccessToken(request, ct);

            if (result.IsSuccess)
            {
                CookieAppendToken(Response.Cookies, "accessToken", result.AccessToken, accessTokenExpirationMinutes);
                CookieAppendToken(Response.Cookies, "refreshToken", result.RefreshToken, refreshTokenExpirationMinutes);

                return Ok();
            }
            else
            {
                return Unauthorized(new
                {
                    message = "Invalid or expired refresh token."
                });
            }
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An internal server error has occured."
            });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirst("sub")?.Value;
        
        Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

        await _authService.LogoutAsync(userId, refreshToken);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        };

        Response.Cookies.Delete("accessToken", cookieOptions);
        Response.Cookies.Delete("refreshToken", cookieOptions);

        return Ok(new { message = "Logged out successfully"});
    }

    private static void CookieAppendToken(
        IResponseCookies cookies,
        string header,
        string token, 
        int expirationMinutes,
        string path = "/")
    {
        var baseOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Path = path
        };

        cookies.Append(header, token, baseOptions);
    }
}