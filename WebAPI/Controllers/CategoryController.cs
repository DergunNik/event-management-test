using System.Linq.Expressions;
using Application.Dtos.Category;
using Application.Services;
using Asp.Versioning;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CategoryPageDto>> GetAllCategoriesPaged([FromQuery] CategoryPaginationDto dto,
        CancellationToken cancellationToken = default)
    {
        var categoriesPage = await categoryService.GetEventsPageAsync(dto, cancellationToken);
        return Ok(categoriesPage);
    }

    [HttpGet("{categoryId:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await categoryService.GetCategoryAsync(categoryId, cancellationToken);
        if (category is null) return NotFound();
        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        await categoryService.AddCategoryAsync(dto, cancellationToken);
        return StatusCode(201);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        await categoryService.UpdateCategoryAsync(dto, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{categoryId:int}")]
    public async Task<IActionResult> DeleteCategory(int categoryId, CancellationToken cancellationToken = default)
    {
        await categoryService.DeleteCategoryAsync(categoryId, cancellationToken);
        return NoContent();
    }
}