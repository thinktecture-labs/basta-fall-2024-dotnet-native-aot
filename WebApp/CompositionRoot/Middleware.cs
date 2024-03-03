using Microsoft.AspNetCore.Builder;
using Serilog;
using WebApp.Reflection;
using WebApp.ToDo;

namespace WebApp.CompositionRoot;

public static class Middleware
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.MapReflectionEndpoint()
           .MapUnboundReflectionEndpoint()
           .MapToDoEndpoints();
        app.MapHealthChecks("/");
        return app;
    }
}