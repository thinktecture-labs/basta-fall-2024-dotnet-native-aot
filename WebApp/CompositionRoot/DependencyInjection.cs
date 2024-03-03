using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebApp.JsonAccess;
using WebApp.LoggingConfiguration;
using WebApp.ToDo;

namespace WebApp.CompositionRoot;

public static class DependencyInjection
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.UseSerilog();
        builder
           .Services
           .AddJsonSerializationContext()
           .AddToDoModule()
           .AddHealthChecks();
        return builder;
    }
}