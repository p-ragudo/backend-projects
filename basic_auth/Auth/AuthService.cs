using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Isopoh.Cryptography.Argon2;

namespace basic_auth.Auth;

public class AuthService
{
    private readonly IDb _db;
    private const string DummyArgon2Hash = "$argon2id$v=19$m=65536,t=3,p=1$c29tZXNhbHRzdHJpbmc$5t/g77k7zHwYqG...";

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
            PasswordHash = Argon2.Hash(request.Password)
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        return new UserResult(
            true
        );
    }

    public async Task<UserResult> LoginAsync(LoginRequest request) 
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        bool isValid = false;

        if (user != null)
        {
            isValid = Argon2.Verify(user.PasswordHash, request.Password);
        }
        else
        {
            Argon2.Verify(DummyArgon2Hash, "stringsrandom87654321");
        }

        if (!isValid || user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        

        return what do I return?
    }
}