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
    ICategoryService service) : ControllerBase
{
    private readonly ILogger<CategoryController> _logger = logger;
    private readonly ICategoryService _service = service;

    #region CRUD Endpoints

    /// <summary>
    /// Get all 'Categories'
    /// </summary>
    [HttpGet]
    [MapToApiVersion(ApiConstants.V1)]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _service.GetAllAsync(this.GetBaseUrl());
            return this.ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all categories");
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Get 'Category' by Id
    /// </summary>
    [HttpGet("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, this.GetBaseUrl());
            return this.ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching category with id {CategoryId}", id);
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Create a new 'Category'
    /// </summary>
    [HttpPost]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanCreateEntity)]
    public async Task<IActionResult> Create([FromBody] CategoryInDto model)
    {
        try
        {
            var result = await _service.CreateAsync(model, this.GetBaseUrl());

            return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : this.ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing 'Category'
    /// </summary>
    [HttpPut("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryInDto model)
    {
        try
        {
            var result = await _service.UpdateAsync(id, model, this.GetBaseUrl());
            return this.ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category with id {CategoryId}", id);
            return Problem(ex.Message);
        }
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
    public async Task<IActionResult> UploadImage(int id, IFormFile? immagine)
    {
        if (immagine == null)
        {
            const string msg = "Empty image";
            _logger.LogError(msg);
            return BadRequest(msg);
        }

        try
        {
            var result = await _service.UploadImageAsync(
                id,
                immagine.OpenReadStream(),
                immagine.FileName,
                this.GetBaseUrl());

            return result.IsSuccess
                ? Ok(result.Value)
                : result.Errors.Any(e => e.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    ? NotFound(result.Errors.First().Message)
                    : BadRequest(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for category {CategoryId}", id);
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Delete an image linked to an existing 'Category'
    /// </summary>
    [HttpDelete("{id}/image")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    public async Task<IActionResult> DeleteImage(int id)
    {
        try
        {
            var result = await _service.DeleteImageAsync(id);

            return result.IsSuccess
                ? Ok()
                : result.Errors.Any(e => e.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    ? NotFound(result.Errors.First().Message)
                    : BadRequest(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image for category {CategoryId}", id);
            return Problem(ex.Message);
        }
    }

    #endregion
}