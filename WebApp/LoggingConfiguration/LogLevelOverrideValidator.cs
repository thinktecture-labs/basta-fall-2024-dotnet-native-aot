using FluentValidation;

namespace WebApp.LoggingConfiguration;

public sealed class LogLevelOverrideValidator : AbstractValidator<LogLevelOverride>
{
    public LogLevelOverrideValidator()
    {
        RuleFor(x => x.Namespace).NotEmpty();
        RuleFor(x => x.Level).IsInEnum();
    }
}