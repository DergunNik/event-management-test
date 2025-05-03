using Application.Dtos.Auth;
using Application.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace WebAPI.Validators.Auth;

public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
{
    public RegistrationRequestValidator(IOptions<AuthOptions> options)
    {
        var validation = options.Value;

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(validation.NameMaxLength)
            .WithMessage($"First name must be at most {validation.NameMaxLength} characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(validation.NameMaxLength)
            .WithMessage($"Last name must be at most {validation.NameMaxLength} characters long.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required.")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(validation.PasswordMinLength)
            .WithMessage($"Password must be at least {validation.PasswordMinLength} characters long.")
            .MaximumLength(validation.PasswordMaxLength)
            .WithMessage($"Password must be at most {validation.PasswordMaxLength} characters long.");
    }
}