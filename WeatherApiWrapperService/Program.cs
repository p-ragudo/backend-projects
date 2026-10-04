using WeatherApiWrapperService.Services.WeatherService;
using StackExchange.Redis;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var weatherApiUrl = builder.Configuration["WeatherApiUrl"]
    ?? throw new Exception("WeatherApiUrl is not set");

var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection")
    ?? throw new Exception("Redis connection string is not set");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddScoped(sp =>
{
   var muxer = sp.GetRequiredService<IConnectionMultiplexer>();
   return muxer.GetDatabase(); 
});

builder.Services.AddHttpClient<WeatherClient>(client =>
    client.BaseAddress = new Uri(weatherApiUrl))
    .AddStandardResilienceHandler();

builder.Services.AddScoped<WeatherService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();