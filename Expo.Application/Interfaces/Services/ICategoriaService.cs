using Expo.Application.DTO.DB;
using FluentResults;

namespace Expo.Application.Interfaces.Services
{
/// <summary>
/// Service to manage Categories
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Get all entities
    /// </summary>
    /// <param name="baseUrl">Controller base URL</param>
    /// <returns>List of Category DTOs</returns>
    Task<Result<IList<CategoryOutDto>>> GetAllAsync(string baseUrl);

    /// <summary>
    /// Get an entity by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="baseUrl">Controller base URL</param>
    /// <returns>Category DTO</returns>
    Task<Result<CategoryOutDto>> GetByIdAsync(int id, string baseUrl);

    /// <summary>
    /// Create a new entity
    /// </summary>
    /// <param name="dto">Data of the entity to create</param>
    /// <param name="baseUrl">Controller base URL</param>
    /// <returns>Created Category DTO</returns>
    Task<Result<CategoryOutDto>> CreateAsync(CategoryInDto dto, string baseUrl);

    /// <summary>
    /// Update an existing entity
    /// </summary>
    /// <param name="id">ID of entity to update</param>
    /// <param name="dto">New entity data</param>
    /// <param name="baseUrl">Controller base URL</param>
    /// <returns>Updated Category DTO</returns>
    Task<Result<CategoryOutDto>> UpdateAsync(int id, CategoryInDto dto, string baseUrl);

    /// <summary>
    /// Delete an entity
    /// </summary>
    /// <param name="id">Entity ID to delete</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Add an image to an existing entity
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="imageStream">Image stream</param>
    /// <param name="fileName">Name of image</param>
    /// <param name="baseUrl">Controller base URL</param>
    /// <returns>Result containing image URL</returns>
    Task<Result<string>> UploadImageAsync(int id, Stream imageStream, string fileName, string baseUrl);

    /// <summary>
    /// Delete an image from an existing entity
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result<bool>> DeleteImageAsync(int id);
}
}