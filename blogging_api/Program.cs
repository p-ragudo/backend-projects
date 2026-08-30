using blogging_api.Data;
using blogging_api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(5, 5, 62));

builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseMySql(
        connectionString,
        serverVersion
    )
);

builder.Services.AddScoped<BlogService>();
builder.Services.AddControllers();

var app = builder.Build();

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();