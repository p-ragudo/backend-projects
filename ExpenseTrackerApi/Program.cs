using System.Text;
using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.Services.Auth;
using ExpenseTrackerApi.Services.Expenses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<Db>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IDb>(sp => sp.GetRequiredService<Db>());

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ExpenseService>();

builder.Services.AddSingleton<TokenProvider>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["ExpenseTrackerApi:Jwt:SecretKey"])),
            ValidIssuer = builder.Configuration["ExpenseTrackerApi:Jwt:Issuer"],
            ValidAudience = builder.Configuration["ExpenseTrackerApi:Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("accessToken", out var accessToken))
                {
                    context.Token = accessToken;
                    Console.WriteLine($"[Auth] Token successfully extracted from cookie: {accessToken[..10]}...");
                }
                if (context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                {
                    Console.WriteLine($"[Auth] Refresh token successfully extracted from cookie: {refreshToken[..10]}...");
                }
                else
                {
                    Console.WriteLine("[Auth] 'accessToken or refreshToken' cookie not found in request!");
                }

                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();