namespace WeatherApiWrapperService.Services.WeatherService;

// This service isn't necessary, but I wanted to at least have some use for the deserialization I just wrote
public class WeatherService
{
    public void PrintWeatherData(WeatherDataDto dto)
    {
        var weatherData = WeatherData.MapFromDto(dto);
        
        Console.WriteLine($"Address: {weatherData.Address}");
        Console.WriteLine($"TimeZone: {weatherData.TimeZone}");

        foreach (var day in weatherData.Days)
        {
            Console.WriteLine("\n");
            Console.WriteLine($"""
                DateTime: {day.DateTime}
                TempMax: {day.TempMax}
                TempMin: {day.TempMin}
                Temp: {day.Temp}
                Humidity: {day.Humidity}
                Precipitation: {day.Precipitation}
                WindSpeed: {day.WindSpeed}
                Pressure: {day.Pressure}
                CloudCover: {day.CloudCover}
                Conditions: {day.Conditions}
            """);
        }
    }
}