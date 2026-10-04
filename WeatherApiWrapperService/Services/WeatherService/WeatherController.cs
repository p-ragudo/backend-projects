using Microsoft.AspNetCore.Mvc;

namespace WeatherApiWrapperService.Services.WeatherService;

[ApiController]
[Route("api/weather")]
public class WeatherController(WeatherService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WeatherDataDto>> Get([FromQuery] GetWeatherRequest request)
    {
        var response = await service.GetWeatherData(request);

        if (response is null)
            return NotFound($"Weather data for location {request.Location} not found.");

        return Ok(response);
    }
}