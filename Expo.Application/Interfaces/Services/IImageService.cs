using FluentResults;

namespace Expo.Application.Interfaces.Services
{
/// <summary>
/// Service to manage images
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Name of the folder where images are locally stored
    /// </summary>
    string ImagesFolder { get; }

    /// <summary>
    /// Save an image stream in local storage
    /// </summary>
    /// <param name="folderName">Folder name</param>
    /// <param name="fileStream">Image stream</param>
    /// <param name="fileName">File name</param>
    /// <param name="extension">File extension</param>
    /// <returns>Result containing the path or URL of saved image</returns>
    Task<Result<string>> SaveImageAsync(string folderName, Stream fileStream, string fileName, string extension);

    /// <summary>
    /// Delete an image from storage
    /// </summary>
    /// <param name="fileName">Name of the file to delete</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result<bool>> DeleteImageAsync(string fileName);

    /// <summary>
    /// Detect if an image stream contains a face
    /// </summary>
    /// <param name="fileStream">Image stream</param>
    /// <returns>Result indicating if a face is detected</returns>
    Task<Result<bool>> HasFaceAsync(Stream fileStream);
}
}