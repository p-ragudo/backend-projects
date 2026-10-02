namespace WeatherApiWrapperService.Services.WeatherService;

// This service isn't necessary, but I wanted to at least have some use for the deserialization I just wrote
public class WeatherService
{
    public void PrintWeatherData(WeatherDataDto dto)
    {
        var weatherData = WeatherData.MapFromDto(dto);
        Console.WriteLine(weatherData);
    }
}