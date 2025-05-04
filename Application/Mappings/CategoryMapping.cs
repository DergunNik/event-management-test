using Application.Abstractions;
using Application.Dtos.Category;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class CategoryMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDto>();

        config.NewConfig<CategoryDto, Category>();

        config.NewConfig<Category, CategoryUpdateDto>();

        config.NewConfig<CategoryUpdateDto, Category>();

        config.NewConfig<Category, CategoryCreateDto>();

        config.NewConfig<CategoryCreateDto, Category>();

        config.NewConfig<PagedResult<Category>, CategoryPageDto>()
            .Ignore(dest => dest.Categories);
    }
}