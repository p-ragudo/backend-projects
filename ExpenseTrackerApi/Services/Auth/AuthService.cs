using System.Security.Cryptography;
using System.Text;
using ExpenseTrackerApi.Data;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Services.Auth;

public class AuthService(
    IDb _db, 
    TokenProvider _tokenProvider, 
    IConfiguration _configuration)
{
    private const string DummyArgon2Hash = "$argon2id$v=19$m=65536,t=3,p=1$c29tZXNhbHRzdHJpbmc$5t/g77k7zHwYqG...";

    public async Task<TokenResult> RegisterAsync(RegisterRequest request)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Email == request.Email);

        if (userExists)
        {
            return TokenResult.Failure(TokenErrorType.UserAlreadyExists);
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            PasswordHash = Argon2.Hash(request.Password)
        };

        _db.Users.Add(newUser);

        var generatedRefreshToken = _tokenProvider.GenerateRefreshToken();
        var generatedRefreshTokenHash = SHA256.HashData(Encoding.UTF8.GetBytes(generatedRefreshToken));
        var generatedAccessToken = _tokenProvider.CreateAccessToken(newUser);
        var expirationMinutes = _configuration.GetValue<int>("ExpenseTrackerApi:Jwt:RefreshTokenExpirationMinutes");

        var newRefreshToken = new RefreshToken()
        {
            TokenHash = generatedRefreshTokenHash,
            UserId = newUser.Id,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync();

        return TokenResult.Success(
            generatedRefreshToken,
            generatedAccessToken,
            newUser.Id,
            newUser.Email
        );
    }

    public async Task<TokenResult> LoginAsync(LoginRequest request) 
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
            return TokenResult.Failure(TokenErrorType.InvalidCredentials);
        }

        await RevokeAllActiveTokensAsync(user.Id);

        var generatedRefreshToken = _tokenProvider.GenerateRefreshToken();
        var generatedRefreshTokenHash = SHA256.HashData(Encoding.UTF8.GetBytes(generatedRefreshToken));
        var generatedAccessToken = _tokenProvider.CreateAccessToken(user);
        var expirationMinutes = _configuration.GetValue<int>("ExpenseTrackerApi:Jwt:RefreshTokenExpirationMinutes");

        var newRefreshToken = new RefreshToken()
        {
            TokenHash = generatedRefreshTokenHash,
            UserId = user.Id,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync();

        return TokenResult.Success(
            generatedRefreshToken,
            generatedAccessToken,
            user.Id,
            user.Email
        );
    }

    public async Task<TokenResult> RefreshAccessToken(RefreshTokenRequest request)
    {
        var requestTokenHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(request.RefreshToken));

        var existingRefreshToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == requestTokenHash);
        
        if (existingRefreshToken is null)
        {
            return TokenResult.Failure(TokenErrorType.InvalidToken);
        }

        if (!existingRefreshToken.IsActive)
        {
            if (existingRefreshToken.Revoked is not null)
            {
                await RevokeAllActiveTokensAsync(existingRefreshToken.UserId);
                await _db.SaveChangesAsync();
                return TokenResult.Failure(TokenErrorType.CompromisedToken);
            }

            return TokenResult.Failure(TokenErrorType.InvalidToken);
        }

        var existingUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == existingRefreshToken.UserId);
        
        if (existingUser is null)
        {
            return TokenResult.Failure(TokenErrorType.UserNotFound);
        }

        var generatedRefreshToken = _tokenProvider.GenerateRefreshToken();
        var generatedRefreshTokenHash = SHA256.HashData(Encoding.UTF8.GetBytes(generatedRefreshToken));
        var generatedAccessToken = _tokenProvider.CreateAccessToken(existingUser);
        var expirationMinutes = _configuration.GetValue<int>("ExpenseTrackerApi:Jwt:RefreshTokenExpirationMinutes");

        var newRefreshToken = new RefreshToken
        {
            TokenHash = generatedRefreshTokenHash,
            UserId = existingUser.Id,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        existingRefreshToken.Revoked = DateTime.UtcNow;
        existingRefreshToken.ReplacedByTokenId = newRefreshToken.Id;

        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync();

        return TokenResult.Success(
            generatedRefreshToken,
            generatedAccessToken,
            existingUser.Id,
            existingUser.Email
        );
    }

    private async Task RevokeAllActiveTokensAsync(Guid userId)
    {
        var activeTokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.Revoked == null)
            .ToListAsync();
        
        foreach (var token in activeTokens)
        {
            token.Revoked = DateTime.UtcNow;
        }
    }
}