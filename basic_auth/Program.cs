using basic_auth.Auth;
using basic_auth.Data;
using basic_auth.StudentService;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<Db>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDb>(sp => sp.GetRequiredService<Db>());
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IStudentService, StudentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => "works");

app.UseBasicAuth();

app.MapControllers();

app.Run();