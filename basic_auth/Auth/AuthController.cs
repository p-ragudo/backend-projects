using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace basic_auth.Auth;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly DbContext _db;

    public AuthController(DbContext db)
    {
        _db = db;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {

    }
}