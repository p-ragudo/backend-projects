# Weather API Wrapper Service

A high-performance RESTful API built with ASP.NET Core that acts as a wrapper around the Visual Crossing Weather API. Implements distributed in-memory caching via Redis with automatic 12-hour expiration, typed resilient HTTP clients, and domain-model mapping.

This is **Project 3** of the [20 Backend Project Ideas Roadmap](https://roadmap.sh/backend/project-ideas).

---

## Tech Stack

- **Framework**: ASP.NET Core (.NET 10+)
- **In-Memory Cache**: Redis (StackExchange.Redis)
- **HTTP Client**: Typed `HttpClient` with standard resilience handlers (`Microsoft.Extensions.Http.Resilience`)
- **Third-Party Provider**: Visual Crossing Weather API
- **Documentation**: OpenAPI / Scalar API Reference
- **Architecture**: Controller-Service pattern using strongly typed records and DTOs

---

## Endpoints

### Weather (`/api/weather`)

| Method | Endpoint | Query Parameters | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/weather` | `Location` (required), `ApiKey` (required), `Date1` (optional), `Date2` (optional) | Retrieves weather forecast data for a specified location. Checks Redis cache first; if missed, queries upstream API and caches response for 12 hours. |

---

## Quick Example

### 1. Fetch Weather (Cache Miss & Store)
```http
GET /api/weather?Location=London,UK&ApiKey=YOUR_VISUAL_CROSSING_API_KEY
Accept: application/json
```

Response (200 OK)
```
{
  "address": "London",
  "timezone": "Europe/London",
  "days": [
    {
      "datetime": "2026-10-05",
      "tempmax": 16.4,
      "tempmin": 9.2,
      "temp": 13.1,
      "humidity": 78.5,
      "precip": 0.0,
      "windspeed": 14.8,
      "pressure": 1018.2,
      "cloudcover": 42.0,
      "conditions": "Partially cloudy"
    }
  ]
}
```

### 2. Fetch Weather by Date Range
```http
GET /api/weather?Location=Tokyo&ApiKey=YOUR_VISUAL_CROSSING_API_KEY&Date1=2026-10-05&Date2=2026-10-07
Accept: application/json
```

## Caching Strategy

- **Key Convention**: `weather:{location.ToLowerInvariant()}`
- **Expiration Policy**: Absolute expiration set to **12 hours** (`TimeSpan.FromHours(12)`).
- **Cache-Aside Pattern**:
  1. Requests hit the `WeatherService` and query Redis using `StringGet(cacheKey)`.
  2. If found (`!IsNullOrEmpty`), the cached JSON is deserialized directly to `WeatherDataDto` and returned.
  3. If missing, the request delegates to the resilient `WeatherClient` to fetch upstream data.
  4. On successful response, payload is serialized and cached via `StringSetAsync(cacheKey, json, 12h)`.

---

## Getting Started

### 1. Clone & Navigate
```bash
git clone [https://github.com/p-ragudo/backend-projects.git](https://github.com/p-ragudo/backend-projects.git)
cd backend-projects/WeatherApiWrapperService
```

### 2. Configure Environment & Secrets
Set your configuration values in appsettings.json, user secrets, or environment variables:

```json
{
  "WeatherApiUrl": "[https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/](https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/)",
  "ConnectionStrings": {
    "RedisConnection": "your-redis-host:port,password=your_password,ssl=False,abortConnect=False"
  }
}
```

### Run the Service
```bash
dotnet restore
dotnet run
```

Access the OpenAPI document at http://localhost:5046/scalar/v1 or http://localhost:{PORT}/scalar/v1.