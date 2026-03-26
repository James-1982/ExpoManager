using Expo.Domain.DTO.DB;
using FluentResults;

namespace Expo.Domain.Interfaces.Services;

/// <summary>
/// Service to manage Stands
/// </summary>
public interface IStandService
{
    /// <summary>
    /// Get all entities
    /// </summary>
    /// <param name="baseUrl">Controller base url</param>
    /// <returns></returns>
    Task<Result<IList<StandOutDto>>> GetAllAsync(string baseUrl);
    /// <summary>
    /// Get an entity by id
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <param name="baseUrl">Controller base url</param>
    /// <returns></returns>
    Task<Result<StandOutDto>> GetByIdAsync(int id, string baseUrl);
    /// <summary>
    /// Create a new Entity
    /// </summary>
    /// <param name="dto">Data of new entity to create</param>
    /// <param name="baseUrl">Controller base url</param>
    /// <returns></returns>
    Task<Result<StandOutDto>> CreateAsync(StandInDto dto, string baseUrl);
    /// <summary>
    /// Update an exisiting Entity
    /// </summary>
    /// <param name="id">Id of entity to update</param>
    /// <param name="dto">New Entity data</param>
    /// <param name="baseUrl">Controller base url</param>
    /// <returns></returns>
    Task<Result<StandOutDto>> UpdateAsync(int id, StandInDto dto, string baseUrl);
    /// <summary>
    /// Request a delete to an entity
    /// </summary>
    /// <param name="id">ENity id to delete</param>
    /// <returns></returns>
    Task DeleteAsync(int id);
    /// <summary>
    /// Add an Image to an existing entity
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <param name="imageStream">Image stream</param>
    /// <param name="fileName">Name of image</param>
    /// <param name="baseUrl">Controller base url</param>
    /// <returns></returns>
    Task<Result<string>> UploadImageAsync(int id, Stream imageStream, string fileName, string baseUrl);
    /// <summary>
    /// Delete an Image from an existing entity
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <returns></returns>
    Task<Result<bool>> DeleteImageAsync(int id);
}
