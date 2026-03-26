using Asp.Versioning;
using Expo.API.Extensions;
using Expo.API.Utils;
using Expo.Domain.Constants;
using Expo.Domain.DTO.DB;
using Expo.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expo.API.Controllers.Database;

/// <summary>
/// Controller to manage 'Categorie'
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="service">Category service</param>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(ApiConstants.V1)]
public class CategoryController(
    ILogger<CategoryController> logger,
    ICategoriaService service) : ControllerBase
{
    #region Fields

    private readonly ILogger<CategoryController> _logger = logger;
    private readonly ICategoriaService _service = service;

    #endregion
    /// <summary>
    /// Get all 'Categorie'
    /// </summary>
    /// <returns>List of categories</returns>
    [HttpGet]
    [MapToApiVersion(ApiConstants.V1)]
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
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }
    }
    /// <summary>
    /// Get 'Categoria' by Id
    /// </summary>
    /// <param name="id">'Categoria' Id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
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
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }
    }
    /// <summary>
    /// Create a new 'Categoria'
    /// </summary>
    /// <param name="model">'Categoria' input model</param>
    /// <returns>Created category</returns>
    [HttpPost]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanCreateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CategoryInDto model)
    {
        try
        {
            var result = await _service.CreateAsync(model, this.GetBaseUrl());

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }
    }
    /// <summary>
    /// Update an existing 'Categoria'
    /// </summary>
    /// <param name="id">'Categoria' Id</param>
    /// <param name="model">'Categoria' input model</param>
    /// <returns>Updated category</returns>
    [HttpPut("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryInDto model)
    {
        try
        {
            var result = await _service.UpdateAsync(id, model, this.GetBaseUrl());

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }
    }
    /// <summary>
    /// Upload a new image for an exisiting 'Categoria'
    /// </summary>
    /// <param name="id">'Categoria' Id</param>
    /// <param name="immagine">Image file</param>
    /// <returns>URL of uploaded image</returns>
    [HttpPost("{id}/image")]
    [MapToApiVersion(ApiConstants.V1)]
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
                var msg = "Empty image";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = await _service.UploadImageAsync(id, immagine.OpenReadStream(), immagine.FileName, this.GetBaseUrl());

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }
    }
    /// <summary>
    /// Delete an image linked to an exisitng 'Categoria'
    /// </summary>
    /// <param name="id">'Categoria' Id</param>
    /// <returns>Status</returns>
    [HttpDelete("{id}/image")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanUpdateEntity)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(int id)
    {
        try
        {
            var result = await _service.DeleteImageAsync(id);

            return result.IsSuccess
                ? Ok()
                : BadRequest(result.Errors.First().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Problem(ex.Message);
        }
    }
    /// <summary>
    /// Request a delete operation to an exisitng 'Categoria'
    /// </summary>
    /// <param name="id">'Categoria' Id</param>
    /// <returns>Status</returns>
    [HttpDelete("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Entity.CanDeleteEntity)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult Delete(int id)
    {
        _service.DeleteAsync(id);

        return Accepted();
    }
}