using System;
using Light.GuardClauses;
using Serilog;
using Serilog.Formatting.Compact;

namespace WebApp.LoggingConfiguration;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration ApplyLogSettings(
        this LoggerConfiguration loggerConfiguration,
        LogSettings settings
    )
    {
        switch (settings.FormattingType)
        {
            case LogFormattingType.Text:
                loggerConfiguration.WriteTo.Console();
                break;
            case LogFormattingType.CompactJson:
                loggerConfiguration.WriteTo.Console(formatter: new CompactJsonFormatter());
                break;
            default:
                throw new ArgumentException("Invalid log formatter type", nameof(settings));
        }

        loggerConfiguration.MinimumLevel.Is(settings.DefaultLevel);

        if (!settings.Overrides.IsNullOrEmpty())
        {
            foreach (var logLevelOverride in settings.Overrides)
            {
                loggerConfiguration.MinimumLevel.Override(logLevelOverride.Namespace, logLevelOverride.Level);
            }
        }

        return loggerConfiguration;
    }
}