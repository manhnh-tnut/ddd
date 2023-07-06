
namespace PRO.Api.Features.User.Validations;
using FluentValidation;
using Commands;
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(ILogger<RegisterUserCommandValidator> logger)
    {
        RuleFor(command => command.userName).NotEmpty().MinimumLength(6);
        RuleFor(command => command.password).NotEmpty().MinimumLength(6);
        RuleFor(command => command.firstName).NotEmpty().MinimumLength(1);
        RuleFor(command => command.lastName).NotEmpty().MinimumLength(1);
        RuleFor(command => command.departmentId).GreaterThan(default(short));
        logger.LogTrace("----- INSTANCE GET - {ClassName}", GetType().Name);
    }
}
