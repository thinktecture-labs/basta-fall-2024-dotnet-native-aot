using Microsoft.AspNetCore.Builder;
using Serilog;
using WebApp.Reflection;

namespace WebApp.CompositionRoot;

public static class Middleware
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.MapReflectionEndpoint();
        app.MapHealthChecks("/");
        return app;
    }
}