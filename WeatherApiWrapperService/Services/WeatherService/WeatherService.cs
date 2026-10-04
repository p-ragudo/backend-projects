using System.Text.Json;
using StackExchange.Redis;

namespace WeatherApiWrapperService.Services.WeatherService;

public class WeatherService(
    WeatherClient weatherClient, 
    IDatabase redisDb,
    ILogger<WeatherService> logger)
{

    public async Task<WeatherDataDto?> GetWeatherData(GetWeatherRequest request)
    {
        var cacheKey = $"weather:{request.Location.ToLower()}";
        var cacheResult = redisDb.StringGet(cacheKey);

        if (!cacheResult.IsNullOrEmpty)
        {
            logger.LogInformation("Fetched from cache");
            return JsonSerializer.Deserialize<WeatherDataDto>(cacheResult.ToString());
        }

        logger.LogInformation("No cached values. Fetching from weather API...");

        var result = await weatherClient.GetWeatherData(request);

        if (result is not null)
        {
            logger.LogInformation($"Result fetched from weather API. Caching at key {cacheKey}");
            await redisDb.StringSetAsync(
                cacheKey, 
                JsonSerializer.Serialize(result),
                TimeSpan.FromHours(24));   
        }
        
        return result;
    }
}