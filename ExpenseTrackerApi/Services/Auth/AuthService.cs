using ExpenseTrackerApi.Data;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Services.Auth;

public class AuthService(IDb _db, TokenProvider _tokenProvider)
{
    private const string DummyArgon2Hash = "$argon2id$v=19$m=65536,t=3,p=1$c29tZXNhbHRzdHJpbmc$5t/g77k7zHwYqG...";

    public async Task<string> RegisterAsync(RegisterRequest request)
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

        var jwt = _tokenProvider.Create(newUser);

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        return jwt;
    }

    public async Task<string> LoginAsync(LoginRequest request) 
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

        return _tokenProvider.Create(user);
    }
}