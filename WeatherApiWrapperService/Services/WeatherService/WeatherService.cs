using System.Text.Json;
using StackExchange.Redis;

namespace WeatherApiWrapperService.Services.WeatherService;

public class WeatherService(WeatherClient weatherClient, IDatabase redisDb)
{

    public async Task<WeatherDataDto?> GetWeatherData(GetWeatherRequest request)
    {
        var cacheKey = $"weather:{request.Location.ToLower()}";
        var cacheResult = redisDb.StringGet(request.Location);

        if (!cacheResult.IsNullOrEmpty)
        {
            return JsonSerializer.Deserialize<WeatherDataDto>(cacheResult.ToString());
        }

        var result = await weatherClient.GetWeatherData(request);

        if (result is not null)
        {
            await redisDb.StringSetAsync(cacheKey, JsonSerializer.Serialize(result));   
        }
        
        return result;
    }
}