using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace WebApp.LoggingConfiguration;

public sealed record LogSettings(
    LogFormattingType FormattingType,
    LogEventLevel DefaultLevel,
    List<LogLevelOverride>? Overrides
)
{
    public static LogSettings FromConfiguration(IConfiguration configuration, string sectionName = "LogSettings")
    {
        var logSettings = configuration.GetSection(sectionName).Get<LogSettings>() ??
                          throw new InvalidDataException($"Could not find {sectionName} section");
        var validationResult = LogSettingsValidator.Create().Validate(logSettings);
        if (!validationResult.IsValid)
        {
            throw new InvalidDataException(validationResult.ToString());
        }

        return logSettings;
    }
}