using Application.Dtos.Event;
using Application.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace WebAPI.Validators.Event;

public class EventFilterDtoValidator : AbstractValidator<EventFilterDto>
{
    public EventFilterDtoValidator(IOptions<ContentRestrictions> restrictions)
    {
        RuleFor(x => x.Title)
            .MaximumLength(restrictions.Value.EventTitleLength)
            .WithMessage($"Title must not exceed {restrictions.Value.EventTitleLength} characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        When(x => x.FromDate.HasValue || x.ToDate.HasValue, () =>
        {
            RuleFor(x => x.FromDate)
                .LessThanOrEqualTo(x => x.ToDate)
                .When(x => x.ToDate.HasValue)
                .WithMessage("FromDate must be less than or equal to ToDate");

            RuleFor(x => x.ToDate)
                .GreaterThanOrEqualTo(x => x.FromDate)
                .When(x => x.FromDate.HasValue)
                .WithMessage("ToDate must be greater than or equal to FromDate");
        });

        RuleFor(x => x.Location)
            .MaximumLength(restrictions.Value.EventLocationLength)
            .WithMessage($"Location must not exceed {restrictions.Value.EventLocationLength} characters")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId must be greater than 0")
            .When(x => x.CategoryId.HasValue);
    }
}