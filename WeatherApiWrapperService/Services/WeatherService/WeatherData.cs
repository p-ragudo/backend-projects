using System.Globalization;

namespace WeatherApiWrapperService.Services.WeatherService;

// Deserialization for application level use cases
public class WeatherData
{
    public string Address { get; set; } = string.Empty;
    public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
    public List<WeatherDayData> Days { get; set; } = [];

    public static WeatherData MapFromDto(WeatherDataDto dto)
    {
        var timeZoneInfo = TimeZoneInfo.TryFindSystemTimeZoneById(dto.TimeZone, out var tz)
            ? tz 
            : TimeZoneInfo.Utc;

        return new()
        {
            Address = dto.Address,
            TimeZone = timeZoneInfo,
            Days = dto.Days
                .Select(d => WeatherDayData.MapFromDto(d))
                .ToList()
        };
    }
}

public class WeatherDayData
{
    public DateTime DateTime { get; set; }
    public double TempMax { get; set; }
    public double TempMin { get; set; }
    public double Temp { get; set; }
    public double Humidity { get; set; }
    public double Precipitation { get; set; }
    public double WindSpeed { get; set; }
    public double Pressure { get; set; }
    public double CloudCover { get; set; }
    public string Conditions { get; set; } = string.Empty;

    public static WeatherDayData MapFromDto(WeatherDayDataDto dto)
    {
        return new()
        {
            DateTime = DateTime
                .TryParse(dto.DateTime, CultureInfo.InvariantCulture, out var dt)
                ? dt
                : default,
            TempMax = dto.TempMax,
            TempMin = dto.TempMin,
            Temp = dto.Temp,
            Humidity = dto.Humidity,
            Precipitation = dto.Precipitation,
            WindSpeed = dto.WindSpeed,
            Pressure = dto.Pressure,
            CloudCover = dto.CloudCover,
            Conditions = dto.Conditions
        };
    }
}