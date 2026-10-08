using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApi.Services.Auth;

[ApiController]
[Route("auth")]
public class AuthController(AuthService _authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);

            return StatusCode(StatusCodes.Status201Created, new
            {
               token = result 
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new
            {
               message = "Invalid username or password",
               detail = ex.Message,
               stackTrace = ex.StackTrace
            });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(new { token = result});
        }
        catch (Exception ex)
        {
            return Unauthorized(new
            {
               message = "Invalid username or password",
               detail = ex.Message
            });
        }
    }
}