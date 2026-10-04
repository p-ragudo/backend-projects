namespace WeatherApiWrapperService.Services.WeatherService;

public class WeatherClient(HttpClient httpClient)
{
    public async Task<WeatherDataDto?> GetWeatherData(
        GetWeatherRequest request, 
        CancellationToken ct = default)
    {
        // query shape: baseUri/[location]/[date1](nullable)/[date2](nullable)?key=apiKey
        var query = request.Location;
        query += $"{(request.Date1 is not null ? $"/{request.Date1}" : "")}" +
            $"{(request.Date2 is not null ? $"/{request.Date2}" : "")}";

        var requestUri = httpClient.BaseAddress + query + $"?key={request.ApiKey}";

        using var response =  await httpClient.GetAsync(requestUri, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<WeatherDataDto>(cancellationToken: ct);
    }
}