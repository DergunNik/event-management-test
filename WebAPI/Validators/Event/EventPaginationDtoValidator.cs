using Application.Dtos.Event;
using FluentValidation;

namespace WebAPI.Validators.Event;

public class EventPaginationDtoValidator : AbstractValidator<EventPaginationDto>
{
    public EventPaginationDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");

        RuleFor(x => x.SortBy)
            .IsInEnum().WithMessage("Invalid sort field.");
    }
}