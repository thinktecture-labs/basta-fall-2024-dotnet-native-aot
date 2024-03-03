using FluentValidation;

namespace WebApp.LoggingConfiguration;

public sealed class LogSettingsValidator : AbstractValidator<LogSettings>
{
    public LogSettingsValidator(LogLevelOverrideValidator logLevelOverrideValidator)
    {
        RuleFor(x => x.FormattingType).IsInEnum();
        RuleFor(x => x.DefaultLevel).IsInEnum();
        RuleForEach(x => x.Overrides).SetValidator(logLevelOverrideValidator).When(x => x.Overrides is not null);
    }
    
    public static LogSettingsValidator Create() => new (new LogLevelOverrideValidator());
}