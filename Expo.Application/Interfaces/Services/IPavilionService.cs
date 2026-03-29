using Expo.Domain.DTO.DB;
using FluentResults;

namespace Expo.Application.Interfaces.Services
{
/// <summary>
/// Service interface to manage Pavilions
/// </summary>
public interface IPavilionService
{
    /// <summary>
    /// Get all pavilions
    /// </summary>
    /// <param name="baseUrl">Base URL of the controller for image links</param>
    /// <returns>Result containing a list of <see cref="PavilionOutDto"/></returns>
    Task<Result<IList<PavilionOutDto>>> GetAllAsync(string baseUrl);

    /// <summary>
    /// Get a pavilion by its id
    /// </summary>
    /// <param name="id">Id of the pavilion</param>
    /// <param name="baseUrl">Base URL of the controller for image links</param>
    /// <returns>Result containing <see cref="PavilionOutDto"/></returns>
    Task<Result<PavilionOutDto>> GetByIdAsync(int id, string baseUrl);

    /// <summary>
    /// Create a new pavilion
    /// </summary>
    /// <param name="dto">Data to create the pavilion</param>
    /// <param name="baseUrl">Base URL of the controller for image links</param>
    /// <returns>Result containing the created <see cref="PavilionOutDto"/></returns>
    Task<Result<PavilionOutDto>> CreateAsync(PavilionInDto dto, string baseUrl);

    /// <summary>
    /// Update an existing pavilion
    /// </summary>
    /// <param name="id">Id of the pavilion to update</param>
    /// <param name="dto">New data for the pavilion</param>
    /// <param name="baseUrl">Base URL of the controller for image links</param>
    /// <returns>Result containing the updated <see cref="PavilionOutDto"/></returns>
    Task<Result<PavilionOutDto>> UpdateAsync(int id, PavilionInDto dto, string baseUrl);

    /// <summary>
    /// Request deletion of a pavilion
    /// </summary>
    /// <param name="id">Id of the pavilion to delete</param>
    /// <returns>Result indicating success or failure</returns>
    Task DeleteAsync(int id);

    /// <summary>
    /// Add an image to an existing pavilion
    /// </summary>
    /// <param name="id">Id of the pavilion</param>
    /// <param name="imageStream">Image stream to upload</param>
    /// <param name="fileName">File name of the image</param>
    /// <param name="baseUrl">Base URL of the controller for image links</param>
    /// <returns>Result containing the public URL of the uploaded image</returns>
    Task<Result<string>> UploadImageAsync(int id, Stream imageStream, string fileName, string baseUrl);

    /// <summary>
    /// Delete an image from an existing pavilion
    /// </summary>
    /// <param name="id">Id of the pavilion</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result<bool>> DeleteImageAsync(int id);
}
}