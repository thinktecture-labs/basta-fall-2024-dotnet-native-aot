using Serilog.Events;

namespace WebApp.LoggingConfiguration;

public sealed record LogLevelOverride(string Namespace, LogEventLevel Level);