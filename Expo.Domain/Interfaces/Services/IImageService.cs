using FluentResults;

namespace Expo.Domain.Interfaces.Services;

/// <summary>
/// Service to manage images
/// </summary>
public interface IImageService
{
    /// <summary>
    /// The name of Folder where images are localy stored
    /// </summary>
    string ImagesFolder { get; }
    /// <summary>
    /// Save a stream image in local storage
    /// </summary>
    /// <param name="folderName">folder name</param>
    /// <param name="fileStream">image stream</param>
    /// <param name="fileName">name of file</param>
    /// <param name="extension">file extension</param>
    /// <returns></returns>
    Task<Result<string>> SaveImageAsync(string folderName, Stream fileStream, string fileName, string extension);
    /// <summary>
    /// Delete an image from resource
    /// </summary>
    /// <param name="fileName">Name of file</param>
    Result DeleteImage(string fileName);
    /// <summary>
    /// Detect if an image stream contains a face or not
    /// </summary>
    /// <param name="fileStream">Stream image</param>
    /// <returns></returns>
    Task<Result<bool>> HasFace(Stream fileStream);
}
