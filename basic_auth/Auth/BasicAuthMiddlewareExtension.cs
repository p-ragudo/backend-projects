namespace basic_auth.Auth;

public static class BasicAuthMiddlewareExtension
{
    public static IApplicationBuilder UseBasicAuth(this IApplicationBuilder app)
    {
        return app.UseMiddleware<BasicAuthMiddleware>();
    }
}