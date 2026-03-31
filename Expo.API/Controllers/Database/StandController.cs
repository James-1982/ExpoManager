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
/// Controller to manage 'Stand'
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="service">Stands service</param>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(ApiConstants.V1)]
public class StandController(
    ILogger<StandController> logger,
    IStandService service) : BaseController(logger)
{
    private readonly IStandService _service = service;

    /// <summary>
    /// Get all 'Stand'
    /// </summary>
    /// <returns>List of Stand</returns>
    [HttpGet]
    [MapToApiVersion(ApiConstants.V1)]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
       return await HandleServiceCall(
            async () => await _service.GetAllAsync(this.GetBaseUrl()),
            "fetching all stands");
    }
    /// <summary>
    /// Get 'Stand' by Id
    /// </summary>
    /// <param name="id">'Stand' Id</param>
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
            $"fetching stand {id}");
    }
    /// <summary>
    /// Create a new 'Stand'
    /// </summary>
    /// <param name="dto">'Stand' input model</param>
    /// <returns>Created Stand</returns>
    [HttpPost]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanCreateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] StandInDto dto)
    {
       return await HandleServiceCall(
            async () => await _service.CreateAsync(dto, this.GetBaseUrl()),
            $"creating stand {dto.Name}");
    }
    /// <summary>
    /// Update an existing 'Stand'
    /// </summary>
    /// <param name="id">'Stand' Id</param>
    /// <param name="dto">'Stand' input model</param>
    /// <returns>Updated Stand</returns>
    [HttpPut("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] StandInDto dto)
    {
        return await HandleServiceCall(
            async () => await _service.UpdateAsync(id, dto, this.GetBaseUrl()),
            $"updating stand {dto.Name}");
    }
    /// <summary>
    /// Upload a new image for an exisiting 'Stand'
    /// </summary>
    /// <param name="id">'Stand' Id</param>
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
            $"uploading image for stand {id}");
    }
    /// <summary>
    /// Delete an image linked to an exisitng 'Stand'
    /// </summary>
    /// <param name="id">'Stand' Id</param>
    /// <returns>Status</returns>
    [HttpDelete("{id}/image")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanDeleteEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(int id)
    {
        return await HandleServiceCall(
            async () => await _service.DeleteImageAsync(id),
            $"deleting image for stand {id}");
    }
    /// <summary>
    /// Request a delete operation to an exisitng 'Stand'
    /// </summary>
    /// <param name="id">'Stand' Id</param>
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