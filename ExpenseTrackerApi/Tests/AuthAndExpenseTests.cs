using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.Services.Auth;
using ExpenseTrackerApi.Services.Expenses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ExpenseTrackerApi.Tests;

public class AuthAndExpenseTests
{
    [Fact]
    public async Task Register_and_login_issue_auth_cookies()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var registered = await RegisterAsync(client);
        Assert.False(string.IsNullOrWhiteSpace(registered.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(registered.RefreshToken));

        using var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("test@example.com", "Correct-horse-42"));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(GetCookie(loginResponse, "accessToken")));
        Assert.False(string.IsNullOrWhiteSpace(GetCookie(loginResponse, "refreshToken")));

        using var invalidLogin = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("test@example.com", "wrong-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, invalidLogin.StatusCode);
    }

    [Fact]
    public async Task Refresh_tokens_are_stored_with_distinct_hashes()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        await RegisterAsync(client);
        using var login = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("test@example.com", "Correct-horse-42"));
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        var distinctHashes = await db.RefreshTokens
            .Select(token => token.TokenHash)
            .Distinct()
            .CountAsync();

        Assert.Equal(2, distinctHashes);
    }

    [Fact]
    public async Task Logout_revokes_refresh_token()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var tokens = await RegisterAsync(client);

        using var logout = new HttpRequestMessage(HttpMethod.Post, "/api/auth/logout");
        AddAuthCookies(logout, tokens.AccessToken, tokens.RefreshToken);

        using var logoutResponse = await client.SendAsync(logout);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);

        using var refreshRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/auth/refresh-token");
        AddAuthCookies(refreshRequest, tokens.AccessToken, tokens.RefreshToken);

        using var refreshResponse = await client.SendAsync(refreshRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }

    [Fact]
    public async Task Expired_access_token_can_be_refreshed_and_used_again()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var tokens = await RegisterAsync(client);
        var expiredAccessToken = CreateExpiredAccessToken();

        using var expiredRequest = new HttpRequestMessage(HttpMethod.Get, "/api/expenses");
        AddAuthCookies(expiredRequest, expiredAccessToken, tokens.RefreshToken);

        using var expiredResponse = await client.SendAsync(expiredRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, expiredResponse.StatusCode);

        using var refreshRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/auth/refresh-token");
        AddAuthCookies(refreshRequest, expiredAccessToken, tokens.RefreshToken);

        using var refreshResponse = await client.SendAsync(refreshRequest);

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var renewedAccessToken = GetCookie(refreshResponse, "accessToken");
        var renewedRefreshToken = GetCookie(refreshResponse, "refreshToken");

        using var retry = new HttpRequestMessage(HttpMethod.Get, "/api/expenses");
        AddAuthCookies(retry, renewedAccessToken, renewedRefreshToken);

        using var retryResponse = await client.SendAsync(retry);
        Assert.Equal(HttpStatusCode.OK, retryResponse.StatusCode);
    }

    [Fact]
    public async Task Expired_refresh_token_is_rejected()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var tokens = await RegisterAsync(client);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Db>();
            var storedToken = await db.RefreshTokens.SingleAsync();
            storedToken.Expires = DateTime.UtcNow.AddMinutes(-1);
            await db.SaveChangesAsync();
        }

        using var refreshRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/auth/refresh-token");
        AddAuthCookies(refreshRequest, tokens.AccessToken, tokens.RefreshToken);

        using var response = await client.SendAsync(refreshRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task All_expense_endpoints_require_authorization()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var id = Guid.NewGuid().ToString();

        using var get = await client.GetAsync("/api/expenses");
        using var create = await client.PostAsJsonAsync(
            "/api/expenses",
            new CreateExpenseRequest("Coffee", 4.50m, "Food"));
        using var update = await client.PutAsJsonAsync(
            "/api/expenses",
            new UpdateExpenseRequest(id, "Coffee", 5m, "Food"));
        using var delete = await client.DeleteAsync($"/api/expenses/{id}?id={id}");

        Assert.Equal(HttpStatusCode.Unauthorized, get.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, create.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, update.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, delete.StatusCode);
    }

    [Fact]
    public async Task Authorized_user_can_create_read_update_and_delete_expenses()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var tokens = await RegisterAsync(client);

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/expenses")
        {
            Content = JsonContent.Create(
                new CreateExpenseRequest("Coffee", 4.50m, "Food"))
        };
        AddAuthCookies(createRequest, tokens.AccessToken, tokens.RefreshToken);

        using var createResponse = await client.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, "/api/expenses");
        AddAuthCookies(getRequest, tokens.AccessToken, tokens.RefreshToken);

        using var getResponse = await client.SendAsync(getRequest);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        using var getJson = await JsonDocument.ParseAsync(
            await getResponse.Content.ReadAsStreamAsync());
        var expenses = getJson.RootElement.GetProperty("expenses");
        Assert.Single(expenses.EnumerateArray());

        var expenseId = expenses[0].GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(expenseId));

        using var updateRequest = new HttpRequestMessage(HttpMethod.Put, "/api/expenses")
        {
            Content = JsonContent.Create(
                new UpdateExpenseRequest(expenseId!, "Lunch", 12m, "Meals"))
        };
        AddAuthCookies(updateRequest, tokens.AccessToken, tokens.RefreshToken);

        using var updateResponse = await client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using var verifyRequest = new HttpRequestMessage(HttpMethod.Get, "/api/expenses");
        AddAuthCookies(verifyRequest, tokens.AccessToken, tokens.RefreshToken);

        using var verifyResponse = await client.SendAsync(verifyRequest);
        using var verifyJson = await JsonDocument.ParseAsync(
            await verifyResponse.Content.ReadAsStreamAsync());
        Assert.Equal(
            "Lunch",
            verifyJson.RootElement
                .GetProperty("expenses")[0]
                .GetProperty("name")
                .GetString());

        using var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/expenses/{expenseId}?id={expenseId}");
        AddAuthCookies(deleteRequest, tokens.AccessToken, tokens.RefreshToken);

        using var deleteResponse = await client.SendAsync(deleteRequest);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using var finalGet = new HttpRequestMessage(HttpMethod.Get, "/api/expenses");
        AddAuthCookies(finalGet, tokens.AccessToken, tokens.RefreshToken);

        using var finalResponse = await client.SendAsync(finalGet);
        using var finalJson = await JsonDocument.ParseAsync(
            await finalResponse.Content.ReadAsStreamAsync());
        Assert.Empty(finalJson.RootElement.GetProperty("expenses").EnumerateArray());
    }

    [Fact]
    public async Task Issued_tokens_have_configured_lifetimes()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        await factory.InitializeDatabaseAsync();

        var started = DateTime.UtcNow;
        var tokens = await RegisterAsync(client);
        var completed = DateTime.UtcNow;

        var accessExpiry = new JsonWebTokenHandler()
            .ReadJsonWebToken(tokens.AccessToken)
            .ValidTo;

        Assert.InRange(
            accessExpiry,
            started.AddMinutes(1).AddSeconds(-2),
            completed.AddMinutes(1).AddSeconds(2));

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        var refreshToken = await db.RefreshTokens.SingleAsync();

        Assert.InRange(
            refreshToken.Expires,
            started.AddMinutes(2).AddSeconds(-2),
            completed.AddMinutes(2).AddSeconds(2));
    }

    private static async Task<(string AccessToken, string RefreshToken)> RegisterAsync(
        HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest(
                "test@example.com",
                "test-user",
                "Correct-horse-42"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return (
            GetCookie(response, "accessToken"),
            GetCookie(response, "refreshToken"));
    }

    private static string GetCookie(HttpResponseMessage response, string name)
    {
        var cookie = response.Headers
            .GetValues("Set-Cookie")
            .FirstOrDefault(value => value.StartsWith(
                $"{name}=",
                StringComparison.Ordinal));

        Assert.NotNull(cookie);
        return Uri.UnescapeDataString(cookie!.Split(';', 2)[0][(name.Length + 1)..]);
    }

    private static void AddAuthCookies(
        HttpRequestMessage request,
        string accessToken,
        string refreshToken)
    {
        request.Headers.TryAddWithoutValidation(
            "Cookie",
            $"accessToken={accessToken}; refreshToken={refreshToken}");
    }

    private static string CreateExpiredAccessToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestApiFactory.Secret));
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(-1),
            Issuer = TestApiFactory.Issuer,
            Audience = TestApiFactory.Issuer,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}

internal sealed class TestApiFactory : WebApplicationFactory<ExpensesController>
{
    public const string Secret =
        "test-secret-key-with-enough-bytes-for-hmac-sha256";
    public const string Issuer = "expense-tracker-tests";

    private readonly SqliteConnection _connection =
        new("Data Source=:memory:");

    public TestApiFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ExpenseTrackerApi:Jwt:SecretKey"] = Secret,
                ["ExpenseTrackerApi:Jwt:Issuer"] = Issuer,
                ["ExpenseTrackerApi:Jwt:Audience"] = Issuer,
                ["ExpenseTrackerApi:Jwt:AccessTokenExpirationMinutes"] = "1",
                ["ExpenseTrackerApi:Jwt:RefreshTokenExpirationMinutes"] = "2"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<Db>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<IDbContextOptionsConfiguration<Db>>();
            services.RemoveAll<Db>();
            services.RemoveAll<IDb>();

            services.AddDbContext<Db>(options => options.UseSqlite(_connection));
            services.AddScoped<IDb>(provider => provider.GetRequiredService<Db>());
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Db>();
        await db.Database.EnsureCreatedAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }
}