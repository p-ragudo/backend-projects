using StackExchange.Redis;

namespace WeatherApiWrapperService.Client;

public static class RedisClient
{
    public static IDatabase GetDatabase(string connectionString)
    {
        var muxer = ConnectionMultiplexer.Connect(connectionString);
        return muxer.GetDatabase();
    }

    public static bool Set(
        IDatabase db,
        RedisKey key, 
        RedisValue value,
        TimeSpan expiry = default)
    {
        return db.StringSet(key, value, expiry);
    }
}