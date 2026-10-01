namespace WeatherApiWrapperService.Services.WeatherService;

public record GetWeatherRequest(
    string Location,
    string ApiKey,
    string? Date1,
    string? Date2
);