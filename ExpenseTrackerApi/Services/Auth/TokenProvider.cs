using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTrackerApi.Services.Auth;

public class TokenProvider(IConfiguration config)
{
    public string Create(User user)
    {
        // multiple projects share the same secrets space, hence the project prefix
        var secretKey = config["ExpenseTrackerApi:Jwt:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured in app settings");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username", user.Username),
            ]),
            Expires = DateTime.UtcNow.AddMinutes(
                config.GetValue<int>("ExpenseTrackerApi:Jwt:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = config["ExpenseTrackerApi:Jwt:Issuer"],
            Audience = config["ExpenseTrackerApi:Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}