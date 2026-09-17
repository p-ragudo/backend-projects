using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using todo_list_api.TodoService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<todo_list_api.DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<todo_list_api.DbContext>();

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/auth")
    .MapIdentityApi<IdentityUser>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => "todo_list_api");
app.MapControllers();

app.Run();