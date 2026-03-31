using Asp.Versioning;
using Expo.API.Extensions;
using Expo.API.Utils;
using Expo.Application.DTO.DB;
using Expo.Application.Interfaces.Services;
using Expo.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expo.API.Controllers.Database;

/// <summary>
/// Controller to manage 'Category'
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="service">Category service</param>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(ApiConstants.V1)]
public class CategoryController(
    ILogger<CategoryController> logger,
    ICategoryService service) : BaseController(logger)
{
    private readonly ICategoryService _service = service;

    #region CRUD

    /// <summary>
    /// Get all 'Categories'
    /// </summary>
    [HttpGet]
    [MapToApiVersion(ApiConstants.V1)]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        return await HandleServiceCall(
              () => _service.GetAllAsync(this.GetBaseUrl()),
              "fetching all categories");
    }

    /// <summary>
    /// Get 'Category' by Id
    /// </summary>
    [HttpGet("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        return await HandleServiceCall(
            () => _service.GetByIdAsync(id, this.GetBaseUrl()),
            $"fetching category {id}");
    }

    /// <summary>
    /// Create a new 'Category'
    /// </summary>
    [HttpPost]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanCreateEntity)]
    public async Task<IActionResult> Create([FromBody] CategoryInDto dto)
    {
        return await HandleServiceCall(
            async () => await _service.CreateAsync(dto, this.GetBaseUrl()),
            $"creating category {dto.Name}");
    }

    /// <summary>
    /// Update an existing 'Category'
    /// </summary>
    [HttpPut("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryInDto dto)
    {
        return await HandleServiceCall(
            async () => await _service.UpdateAsync(id, dto, this.GetBaseUrl()),
            $"updating category {dto.Name}");
    }

    /// <summary>
    /// Request a delete operation for a 'Category'
    /// </summary>
    [HttpDelete("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanDeleteEntity)]
    public IActionResult Delete(int id)
    {
        try
        {
            _service.DeleteAsync(id); // Async fire-and-forget

            _logger.LogInformation("Scheduled deletion for category {CategoryId}", id);

            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling deletion for category {CategoryId}", id);
            return Problem(ex.Message);
        }
    }

    #endregion

    #region Image Endpoints

    /// <summary>
    /// Upload a new image for an existing 'Category'
    /// </summary>
    [HttpPost("{id}/image")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    public async Task<IActionResult> UploadImage(int id, IFormFile? image)
    {
        if (image == null)
        {
            string msg = "Empty image";
            _logger.LogError(msg);
            return BadRequest(msg);
        }

        return await HandleImageUpload(
        () => _service.UploadImageAsync(id, image.OpenReadStream(), image.FileName, this.GetBaseUrl()),
        "Category", id);
    }

    /// <summary>
    /// Delete an image linked to an existing 'Category'
    /// </summary>
    [HttpDelete("{id}/image")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    public async Task<IActionResult> DeleteImage(int id)
    {
        return await HandleImageDelete(
            () => _service.DeleteImageAsync(id),
            "Category",
            id);
    }

    #endregion
}