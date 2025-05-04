using Application.Dtos.Event;
using Application.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace WebAPI.Validators.Event;

public class EventUpdateDtoValidator : AbstractValidator<EventUpdateDto>
{
    public EventUpdateDtoValidator(IOptions<ContentRestrictions> restrictionsOptions)
    {
        var restrictions = restrictionsOptions.Value;
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(restrictions.EventTitleLength)
            .WithMessage($"Title must not exceed {restrictions.EventTitleLength} characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(restrictions.EventLocationLength)
            .WithMessage($"Description must not exceed {restrictions.EventLocationLength} characters");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required")
            .MaximumLength(restrictions.EventLocationLength)
            .WithMessage($"Location must not exceed {restrictions.EventLocationLength} characters");

        RuleFor(x => x.DateTime)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("DateTime must be in the future");

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0)
            .WithMessage("MaxParticipants must be greater than 0");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId must be valid");

        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be valid");
    }
}