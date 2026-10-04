using System.Text.Json.Serialization;

namespace WeatherApiWrapperService.Services.WeatherService;

public record GetWeatherRequest(
    string Location,
    string ApiKey,
    string? Date1,
    string? Date2
);

public record WeatherDataDto(
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("timezone")] string TimeZone,
    [property: JsonPropertyName("days")] IReadOnlyList<WeatherDayDataDto> Days
);

public record WeatherDayDataDto(
    [property: JsonPropertyName("datetime")] string DateTime,
    [property: JsonPropertyName("tempmax")] double TempMax,
    [property: JsonPropertyName("tempmin")] double TempMin,
    [property: JsonPropertyName("temp")] double Temp,
    [property: JsonPropertyName("humidity")] double Humidity,
    [property: JsonPropertyName("precip")] double Precipitation,
    [property: JsonPropertyName("windspeed")] double WindSpeed,
    [property: JsonPropertyName("pressure")] double Pressure,
    [property: JsonPropertyName("cloudcover")] double CloudCover,
    [property: JsonPropertyName("conditions")] string Conditions
);