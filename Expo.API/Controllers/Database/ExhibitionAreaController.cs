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
/// Controller to manage 'ExhibitionArea'
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="service">ExhibitionArea service</param>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(ApiConstants.V1)]
public class ExhibitionAreaController(
    ILogger<ExhibitionAreaController> logger,
    IExhibitionAreaService service) : BaseController(logger)
{
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
        return await HandleServiceCall(
                    async () => await _service.GetAllAsync(this.GetBaseUrl()),
                    "fetching all exhibition areas");
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
        return await HandleServiceCall(
            async () => await _service.GetByIdAsync(id, this.GetBaseUrl()),
            $"fetching exhibition area {id}");

    }

    /// <summary>
    /// Create a new 'ExhibitionArea'
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Policy.Entity.CanCreateEntity)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ExhibitionAreaInDto dto)
    {
        return await HandleServiceCall(
            async () => await _service.CreateAsync(dto, this.GetBaseUrl()),
            $"creating exhibition area {dto.Name}");
    }

    /// <summary>
    /// Update an existing 'ExhibitionArea'
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ExhibitionAreaInDto dto)
    {
        return await HandleServiceCall(
            async () => await _service.UpdateAsync(id, dto, this.GetBaseUrl()),
            $"updating exhibition area {dto.Name}");
    }

    /// <summary>
    /// Request a delete operation for an existing 'ExhibitionArea'
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = Policy.Entity.CanDeleteEntity)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult Delete(int id)
    {
        _service.DeleteAsync(id); // Fire-and-forget background job
        return Accepted();
    }

    /// <summary>
    /// Upload a new image for an existing 'ExhibitionArea'
    /// </summary>
    [HttpPost("{id}/image")]
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
            $"uploading image for exhibition area {id}");
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
        return await HandleServiceCall(
            async () => await _service.DeleteImageAsync(id),
            $"deleting image for exhibition area {id}");
    }
}