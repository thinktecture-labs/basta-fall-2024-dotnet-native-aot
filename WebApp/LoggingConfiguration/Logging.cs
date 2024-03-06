using Microsoft.AspNetCore.Builder;
using Serilog;

namespace WebApp.LoggingConfiguration;

public static class Logging
{
    public static ILogger CreateBootstrapLogger() =>
        new LoggerConfiguration()
           .WriteTo.Console()
           .CreateBootstrapLogger();

    public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(
            (context, loggerConfiguration) =>
            {
                var logSettings = LogSettings.FromConfiguration(context.Configuration);
                loggerConfiguration.ApplyLogSettings(logSettings);
            }
        );
        return builder;
    }
}