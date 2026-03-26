using Asp.Versioning;
using Expo.API.Utils;
using Expo.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Expo.API.Controllers;

/// <summary>
/// Controller for image-related operations
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="imageService">Image Service</param>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(ApiConstants.V1)]
public class ImageController(
    ILogger<ImageController> logger,
    IImageService imageService) : ControllerBase
{
    private readonly ILogger<ImageController> _logger = logger;
    private readonly IImageService _imageService = imageService;

    /// <summary>
    /// Check if an image contains a face
    /// </summary>
    /// <param name="file">image file</param>
    /// <returns>true/false</returns>
    [HttpPost("has-face")]
    [MapToApiVersion(ApiConstants.V1)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CheckFace(IFormFile file)
    {
        if (file == null)
        {
            _logger.LogWarning("CheckFace called with no file uploaded.");
            return BadRequest("No file provided.");
        }

        try
        {
            _logger.LogInformation($"Checking for face in uploaded file: {file.FileName}");

            var result = await _imageService.HasFace(file.OpenReadStream());

            _logger.LogInformation($"Face detection result for file {file.FileName}: {result}");

            return result.IsSuccess
                ? Ok(new { faceDetected = result.Value })
                : BadRequest(result.Errors.FirstOrDefault().Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while checking face in file {file?.FileName ?? "null"}");
            return Problem($"An error occurred while processing the image.");
        }
    }
}