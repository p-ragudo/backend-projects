using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WeatherApiWrapperService.Services.WeatherService;

[ApiController]
[Route("api/weather")]
public class WeatherController(WeatherClient client, WeatherService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WeatherDataDto>> Get([FromQuery] GetWeatherRequest request)
    {
        var response = await client.GetWeatherData(request);

        if (response is null)
            return NotFound($"Weather data for location {request.Location} not found.");
        
        service.PrintWeatherData(response);
        return Ok(response);
    }
}