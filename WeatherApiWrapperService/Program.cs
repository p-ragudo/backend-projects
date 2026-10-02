using WeatherApiWrapperService.Services.WeatherService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var weatherApiUrl = builder.Configuration["WeatherApiUrl"]
    ?? throw new Exception("WeatherApiUrl is not set");

builder.Services.AddHttpClient<WeatherClient>(client =>
    client.BaseAddress = new Uri(weatherApiUrl))
    .AddStandardResilienceHandler();

builder.Services.AddScoped<WeatherService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();