using Application.Dtos.User;
using Application.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace WebAPI.Validators.User;

public class UserPaginationDtoValidator : AbstractValidator<UserPaginationDto>
{
    public UserPaginationDtoValidator(IOptions<ContentRestrictions> restrictions)
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(restrictions.Value.PageSize)
            .WithMessage($"Page size cannot exceed {restrictions.Value.PageSize}");

        RuleFor(x => x.SortBy)
            .IsInEnum()
            .WithMessage("Invalid sort field");
    }
}