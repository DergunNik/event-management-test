using Application.Dtos.Category;
using Application.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace WebAPI.Validators.Category;

public class CategoryPaginationDtoValidator : AbstractValidator<CategoryPaginationDto>
{
    public CategoryPaginationDtoValidator(IOptions<ContentRestrictions> restrictions)
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, restrictions.Value.PageSize)
            .WithMessage($"Page size must be between 1 and {restrictions.Value.PageSize}");
    }
}