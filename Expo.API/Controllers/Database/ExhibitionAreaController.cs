using Asp.Versioning;
using Expo.API.Extensions;
using Expo.API.Utils;
using Expo.Application.Interfaces.Services;
using Expo.Domain.Constants;
using Expo.Domain.DTO.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expo.API.Controllers.Database;

/// <summary>
/// Controller to manage 'ExhibitionArea'
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="service">ExhibitionArea service</param>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(ApiConstants.V1)]
public class ExhibitionAreaController(
    ILogger<ExhibitionAreaController> logger,
    IExhibitionAreaService service) : ControllerBase
{
    private readonly ILogger<ExhibitionAreaController> _logger = logger;
    private readonly IExhibitionAreaService _service = service;

    /// <summary>
    /// Get all 'ExhibitionArea'
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _service.GetAllAsync(this.GetBaseUrl());
            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all ExhibitionAreas");
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Get 'ExhibitionArea' by Id
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id, this.GetBaseUrl());
            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ExhibitionArea {ExhibitionAreaId}", id);
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Create a new 'ExhibitionArea'
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Policy.Entity.CanCreateEntity)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ExhibitionAreaInDto model)
    {
        try
        {
            var result = await _service.CreateAsync(model, this.GetBaseUrl());
            return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : BadRequest(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ExhibitionArea");
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing 'ExhibitionArea'
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ExhibitionAreaInDto model)
    {
        try
        {
            var result = await _service.UpdateAsync(id, model, this.GetBaseUrl());
            if (!result.IsSuccess && result.Errors.Any(e => e.Message.Contains("not found")))
                return NotFound(result.Errors.First().Message);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ExhibitionArea {ExhibitionAreaId}", id);
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Upload a new image for an existing 'ExhibitionArea'
    /// </summary>
    [HttpPost("{id}/image")]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(int id, IFormFile? immagine)
    {
        try
        {
            if (immagine == null)
            {
                _logger.LogError("Empty image upload attempted for ExhibitionArea {ExhibitionAreaId}", id);
                return BadRequest("Empty image");
            }

            var result = await _service.UploadImageAsync(id, immagine.OpenReadStream(), immagine.FileName, this.GetBaseUrl());
            if (!result.IsSuccess && result.Errors.Any(e => e.Message.Contains("not found")))
                return NotFound(result.Errors.First().Message);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for ExhibitionArea {ExhibitionAreaId}", id);
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Delete an image linked to an existing 'ExhibitionArea'
    /// </summary>
    [HttpDelete("{id}/image")]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(int id)
    {
        try
        {
            var result = await _service.DeleteImageAsync(id);
            if (!result.IsSuccess && result.Errors.Any(e => e.Message.Contains("not found")))
                return NotFound(result.Errors.First().Message);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image for ExhibitionArea {ExhibitionAreaId}", id);
            return Problem(ex.Message);
        }
    }

    /// <summary>
    /// Request a delete operation for an existing 'ExhibitionArea'
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = Policy.Entity.CanDeleteEntity)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult Delete(int id)
    {
        _service.DeleteAsync(id);
        _logger.LogInformation("Scheduled deletion for ExhibitionArea {ExhibitionAreaId}", id);
        return Accepted();
    }
}