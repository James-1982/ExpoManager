using Expo.Application.DTO.DB;
using FluentResults;

namespace Expo.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface to manage Stands
    /// </summary>
    public interface IStandService
    {
        /// <summary>
        /// Get all stands
        /// </summary>
        /// <param name="baseUrl">Base URL of the controller for image links</param>
        /// <returns>Result containing a list of <see cref="StandOutDto"/></returns>
        Task<Result<IList<StandOutDto>>> GetAllAsync(string baseUrl);

        /// <summary>
        /// Get a stand by its id
        /// </summary>
        /// <param name="id">Id of the stand</param>
        /// <param name="baseUrl">Base URL of the controller for image links</param>
        /// <returns>Result containing <see cref="StandOutDto"/></returns>
        Task<Result<StandOutDto>> GetByIdAsync(int id, string baseUrl);

        /// <summary>
        /// Create a new stand
        /// </summary>
        /// <param name="dto">Data to create the stand</param>
        /// <param name="baseUrl">Base URL of the controller for image links</param>
        /// <returns>Result containing the created <see cref="StandOutDto"/></returns>
        Task<Result<StandOutDto>> CreateAsync(StandInDto dto, string baseUrl);

        /// <summary>
        /// Update an existing stand
        /// </summary>
        /// <param name="id">Id of the stand to update</param>
        /// <param name="dto">New data for the stand</param>
        /// <param name="baseUrl">Base URL of the controller for image links</param>
        /// <returns>Result containing the updated <see cref="StandOutDto"/></returns>
        Task<Result<StandOutDto>> UpdateAsync(int id, StandInDto dto, string baseUrl);

        /// <summary>
        /// Request deletion of a stand
        /// </summary>
        /// <param name="id">Id of the stand to delete</param>
        /// <returns>Task</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Add an image to an existing stand
        /// </summary>
        /// <param name="id">Id of the stand</param>
        /// <param name="imageStream">Image stream to upload</param>
        /// <param name="fileName">File name of the image</param>
        /// <param name="baseUrl">Base URL of the controller for image links</param>
        /// <returns>Result containing the public URL of the uploaded image</returns>
        Task<Result<string>> UploadImageAsync(int id, Stream imageStream, string fileName, string baseUrl);

        /// <summary>
        /// Delete an image from an existing stand
        /// </summary>
        /// <param name="id">Id of the stand</param>
        /// <returns>Result indicating success or failure</returns>
        Task<Result<bool>> DeleteImageAsync(int id);
    }
}