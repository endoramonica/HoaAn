using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Category;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    #region CRUD Operations

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await _categoryService.CreateAsync(dto);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        var result = await _categoryService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPatch("{id}/soft-delete")]
    public async Task<ActionResult<ApiResponse<bool>>> SoftDelete(Guid id)
    {
        var result = await _categoryService.SoftDeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    #endregion

    #region Query Operations

    [HttpGet("store/{storeId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetByStoreId(Guid storeId)
    {
        var result = await _categoryService.GetByStoreIdAsync(storeId);
        return Ok(result);
    }

    [HttpGet("active/list")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetActiveCategories()
    {
        var result = await _categoryService.GetActiveCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("{parentId}/subcategories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetSubCategories(Guid parentId)
    {
        var result = await _categoryService.GetSubCategoriesAsync(parentId);
        return Ok(result);
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<ApiResponse<CategoryDetailDto>>> GetWithDetails(Guid id)
    {
        var result = await _categoryService.GetWithDetailsAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("hierarchy/{storeId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategoryHierarchy(Guid storeId)
    {
        var result = await _categoryService.GetCategoryHierarchyAsync(storeId);
        return Ok(result);
    }

    #endregion

    #region Pagination

    [HttpGet("paged/list")]
    public async Task<ActionResult<ApiResponse<PaginatedResult<CategoryDto>>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? storeId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? searchTerm = null)
    {
        var result = await _categoryService.GetPagedAsync(pageNumber, pageSize, storeId, isActive, searchTerm);
        return Ok(result);
    }

    #endregion

    #region Statistics

    [HttpGet("count/by-store/{storeId}")]
    public async Task<ActionResult<ApiResponse<int>>> CountByStore(Guid storeId)
    {
        var result = await _categoryService.CountByStoreAsync(storeId);
        return Ok(result);
    }

    [HttpGet("exists")]
    public async Task<ActionResult<ApiResponse<bool>>> ExistsByName([FromQuery] Guid storeId, [FromQuery] string name)
    {
        var result = await _categoryService.ExistsByNameAsync(storeId, name);
        return Ok(result);
    }

    #endregion

    #region Bulk Operations

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCategoryStatus(Guid id, [FromQuery] bool isActive)
    {
        var result = await _categoryService.UpdateCategoryStatusAsync(id, isActive);
        return result.Success ? Ok(result) : NotFound(result);
    }

    #endregion
}