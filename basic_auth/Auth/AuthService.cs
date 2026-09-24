using Microsoft.EntityFrameworkCore;
using Isopoh.Cryptography.Argon2;

namespace basic_auth.Auth;

public class AuthService : IAuthService
{
    private readonly IDb _db;
    private readonly int _sessionTtlSeconds;
    private const string DummyArgon2Hash = "$argon2id$v=19$m=65536,t=3,p=1$c29tZXNhbHRzdHJpbmc$5t/g77k7zHwYqG...";

    public AuthService(IDb db, IConfiguration configuration)
    {
        _db = db;
        _sessionTtlSeconds = configuration.GetValue("Auth:SessionTtlSeconds", 3600);
    }

    public async Task<RegisterResult> RegisterAsync(RegisterRequest request, string? userAgent)
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

        var rawToken = Session.GenerateSessionToken();
        var expiresat = DateTime.UtcNow.AddSeconds(_sessionTtlSeconds);

        var session = new Session
        {
            TokenHash = Session.HashToken(rawToken),
            UserId = newUser.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresat,
            UserAgent = userAgent
        };

        _db.Users.Add(newUser);
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();

        return new RegisterResult(
            rawToken,
            newUser.Id,
            newUser.Email,
            expiresat
        );
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, string? userAgent) 
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

        var rawToken = Session.GenerateSessionToken();
        var expiresat = DateTime.UtcNow.AddSeconds(_sessionTtlSeconds);

        var session = new Session
        {
            TokenHash = Session.HashToken(rawToken),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresat,
            UserAgent = userAgent
        };

        await _db.Sessions
            .Where(s => s.UserId == user.Id)
            .ExecuteDeleteAsync();

        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();
        
        return new LoginResult(
            rawToken,
            user.Id,
            user.Email,
            expiresat
        );
    }

    public async Task LogoutAsync(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return;
        }

        var hashedToken = Session.HashToken(rawToken);

        await _db.Sessions
            .Where(s => s.TokenHash == hashedToken)
            .ExecuteDeleteAsync();
    }
}