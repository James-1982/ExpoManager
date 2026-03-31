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
/// Controller to manage 'Pavilion'
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="service">Pavilion service</param>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(ApiConstants.V1)]
public class PavilionController(
    ILogger<PavilionController> logger,
    IPavilionService service) : BaseController(logger)
{
    private readonly IPavilionService _service = service;
    /// <summary>
    /// Get all 'Pavilion'
    /// </summary>
    /// <returns>List of categories</returns>
    [HttpGet]
    [MapToApiVersion(ApiConstants.V1)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
        return await HandleServiceCall(
            async () => await _service.GetAllAsync(this.GetBaseUrl()),
            "fetching all pavilions");
    }
    /// <summary>
    /// Get 'Pavilion' by Id
    /// </summary>
    /// <param name="id">'Pavilion' Id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        return await HandleServiceCall(
            async () => await _service.GetByIdAsync(id, this.GetBaseUrl()),
            $"fetching pavilion {id}");
    }
    /// <summary>
    /// Create a new 'Pavilion'
    /// </summary>
    /// <param name="dto">'Pavilion' input model</param>
    /// <returns>Created Pavilion</returns>
    [HttpPost]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanCreateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] PavilionInDto dto)
    {
        return await HandleServiceCall(
            async () => await _service.CreateAsync(dto, this.GetBaseUrl()),
            $"creating pavilion {dto.Name}");
    }
    /// <summary>
    /// Update an existing 'Pavilion'
    /// </summary>
    /// <param name="id">'Pavilion' Id</param>
    /// <param name="dto">'Pavilion' input model</param>
    /// <returns>Updated Pavilion</returns>
    [HttpPut("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] PavilionInDto dto)
    {
        return await HandleServiceCall(
            async () => await _service.UpdateAsync(id, dto, this.GetBaseUrl()),
            $"updating pavilion {dto.Name}");
    }
    /// <summary>
    /// Upload a new image for an exisiting 'Pavilion'
    /// </summary>
    /// <param name="id">'Pavilion' Id</param>
    /// <param name="image">Image file</param>
    /// <returns>URL of uploaded image</returns>
    [HttpPost("{id}/image")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(int id, IFormFile? image)
    {
        if (image == null)
            return BadRequest("Empty image");

        return await HandleServiceCall(
            async () => await _service.UploadImageAsync(id, image.OpenReadStream(), image.FileName, this.GetBaseUrl()),
            $"uploading image for pavilion {id}");
    }
    /// <summary>
    /// Delete an image linked to an exisitng 'Pavilion'
    /// </summary>
    /// <param name="id">'Pavilion' Id</param>
    /// <returns>Status</returns>
    [HttpDelete("{id}/image")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(int id)
    {
        return await HandleServiceCall(
            async () => await _service.DeleteImageAsync(id),
            $"deleting image for pavilion {id}");
    }
    /// <summary>
    /// Request a delete operation to an exisitng 'Pavilion'
    /// </summary>
    /// <param name="id">'Pavilion' Id</param>
    /// <returns>Status</returns>
    [HttpDelete("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanDeleteEntity)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult Delete(int id)
    {
        _service.DeleteAsync(id); // fire-and-forget
        return Accepted();
    }
}
