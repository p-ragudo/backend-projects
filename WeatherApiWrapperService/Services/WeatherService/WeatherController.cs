using Microsoft.AspNetCore.Mvc;

namespace WeatherApiWrapperService.Services.WeatherService;

[ApiController]
[Route("api/weather")]
public class WeatherController(WeatherClient client) : ControllerBase
{
    [HttpGet]
    public async Task<object> Get([FromQuery] GetWeatherRequest request)
    {
        return await client.GetWeatherData(request);
    }
}