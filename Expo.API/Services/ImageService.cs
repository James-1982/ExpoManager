using Expo.Domain.Interfaces.Services;
using FluentResults;
using OpenCvSharp;

namespace Expo.API.Services;

/// <summary>
/// <inheritdoc/>
/// </summary>
internal class ImageService : IImageService
{
    private readonly string _rootPath;
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private readonly string _relativeImageFolder;
    public ImageService(
        IConfiguration config,
        IWebHostEnvironment env)
    {
        _relativeImageFolder = config["ImageSettings:Path"] ?? "images";
        _rootPath = Path.Combine(env.WebRootPath, _relativeImageFolder);

        if (!Directory.Exists(_rootPath))
            Directory.CreateDirectory(_rootPath);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string ImagesFolder => _relativeImageFolder;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<string>> SaveImageAsync(string folderName, Stream fileStream, string fileName, string extension)
    {
        if (fileStream == null || fileStream.Length == 0)
            return Result.Fail("Invalid input file");

        if (fileStream.Length > MaxFileSize)
            return Result.Fail("File too large");

        extension = extension.ToLowerInvariant();
        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            return Result.Fail("Invalid file type");

        var folderPath = Path.Combine(_rootPath, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var fullFileName = $"{fileName}{extension}";
        var filePath = Path.Combine(folderPath, fullFileName);

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(stream);

        // ritorna path relativo per URL
        return Result.Ok(Path.Combine(folderName, fullFileName).Replace("\\", "/"));
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result DeleteImage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            Result.Ok();

        var path = Path.Combine(_rootPath, fileName);

        if (File.Exists(path))
            File.Delete(path);

        return File.Exists(path)
            ? Result.Fail("Can not delete image")
            : Result.Ok();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<Result<bool>> HasFace(Stream fileStream)
    {
        if (fileStream == null || fileStream.Length == 0)
            return Result.Fail("Invalid input file");

        byte[] bytes;
        using (var ms = new MemoryStream())
        {
            await fileStream.CopyToAsync(ms);
            bytes = ms.ToArray();
        }

        using var mat = Cv2.ImDecode(bytes, ImreadModes.Color);
        if (mat.Empty())
            return Result.Fail("Invalid input file");

        using var gray = new Mat();
        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

        var faceCascade = new CascadeClassifier("Assets/haarcascade_frontalface_default.xml");
        Rect[] faces = faceCascade.DetectMultiScale(gray, 1.1, 5);

        return Result.Ok(faces.Length > 0);
    }
}