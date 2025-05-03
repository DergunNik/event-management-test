using Application.Dtos.Category;
using Application.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace WebAPI.Validators.Category;

public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateDtoValidator(IOptions<ContentRestrictions> restrictions)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Invalid category ID");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(restrictions.Value.CategoryTitleLength)
            .WithMessage($"Category name must not exceed {restrictions.Value.CategoryTitleLength} characters");
    }
}