using Application.Dtos.Category;
using Application.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace WebAPI.Validators.Category;

public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator(IOptions<ContentRestrictions> restrictions)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(restrictions.Value.CategoryTitleLength)
            .WithMessage($"Category name must not exceed {restrictions.Value.CategoryTitleLength} characters");
    }
}