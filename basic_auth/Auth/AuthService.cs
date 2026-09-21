using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;

namespace basic_auth.Auth;

public class AuthService
{
    private readonly IDb _db;

    public AuthService(IDb db)
    {
        _db = db;
    }

    public async Task<UserResult> RegisterAsync(RegisterRequest request)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Email == request.Email);

        if (userExists)
        {
            throw new InvalidOperationException("Email is already registered");
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = 
        }
    }
}