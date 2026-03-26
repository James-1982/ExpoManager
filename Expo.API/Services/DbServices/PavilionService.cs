using FluentResults;
using Hangfire;
using Expo.Domain.DTO.DB;
using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repo;
using Expo.Domain.Interfaces.Services;
using MapsterMapper;

namespace Expo.API.Services.DbServices;

/// <summary>
/// Service to manage Padiglioni
/// </summary>
/// <param name="logger">Logger for diagnostic purposes.</param>
/// <param name="mapper">Mapper used for object-to-object mapping.</param>
/// <param name="imageService">Service used to manage image processing and storage.</param>
/// <param name="backgroundJobClient">Background job scheduler used to run async tasks.</param>
/// <param name="uow">Unit of Work for transactional persistence.</param>
/// <remarks>
/// This constructor injects all required services for category operations.
/// </remarks>
internal class PavilionService(
    ILogger<PavilionService> logger,
    IMapper mapper,
    IImageService imageService,
    IBackgroundJobClient backgroundJobClient,
    IUnitOfWork uow) : IPavilionService
{
    #region Fields

    private readonly ILogger<PavilionService> _logger = logger;
    private readonly IImageService _imageService = imageService;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _uow = uow;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public async Task<Result<IList<PavilionOutDto>>> GetAllAsync(string baseUrl)
    {
        _logger.LogInformation("Fetching all pavilions");

        var entities = await _uow.Pavilions.GetAllAsync();

        if (entities == null || !entities.Any())
        {
            var msg = "No data found";
            _logger.LogInformation(msg);
            return Result.Fail(msg);
        }

        var dtos = _mapper.From(entities)
                  .AddParameters("BaseUrl", baseUrl)
                  .AdaptToType<List<PavilionOutDto>>();

        return Result.Ok<IList<PavilionOutDto>>(dtos);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public async Task<Result<PavilionOutDto>> GetByIdAsync(int id, string baseUrl)
    {
        var entity = await _uow.Pavilions.GetByIdAsync(id);

        if (entity == null)
        {
            var msg = $"Data with {id} not found";
            _logger.LogInformation(msg);
            return Result.Fail(msg);
        }

        var dto = _mapper.From(entity)
                 .AddParameters("BaseUrl", baseUrl)
                 .AdaptToType<PavilionOutDto>();

        return Result.Ok(dto);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public async Task<Result<PavilionOutDto>> CreateAsync(PavilionInDto dto, string baseUrl)
    {
        try
        {
            var entity = _mapper.Map<Pavilion>(dto);

            await _uow.Pavilions.AddAsync(entity);

            await _uow.SaveAsync();

            var outDto = _mapper.From(entity)
             .AddParameters("BaseUrl", baseUrl)
             .AdaptToType<PavilionOutDto>();

            return Result.Ok(outDto);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public async Task<Result<PavilionOutDto>> UpdateAsync(int id, PavilionInDto dto, string baseUrl)
    {
        try
        {
            var entity = await _uow.Pavilions.GetByIdAsync(id);

            if (entity == null)
            {
                var msg = $"Entity {id} not found";
                _logger.LogWarning(msg);
                return Result.Fail(msg);
            }

            _mapper.Map(dto, entity); // Update existing entity with new data

            await _uow.SaveAsync();

            var outDto = _mapper.From(entity)
             .AddParameters("BaseUrl", baseUrl)
             .AdaptToType<PavilionOutDto>();

            return Result.Ok(outDto);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteAsync(int id)
    {
        _backgroundJobClient.Enqueue(() => DeleteJob(id));

        _logger.LogInformation($"Scheduled deletion for category {id}");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteJob(int id)
    {
        var entity = await _uow.Pavilions.GetByIdAsync(id);

        if (entity == null)
            return;

        _uow.Pavilions.Remove(entity);

        await _uow.SaveAsync();

        entity = await _uow.Pavilions.GetByIdAsync(id);

        if (entity == null)
        {
            if (!string.IsNullOrEmpty(entity.ImmaginePath))
            {
                _logger.LogInformation($"Deleting image for categoria {id} in background job");
                _imageService.DeleteImage(entity.ImmaginePath);
            }

            _logger.LogInformation($"Categoria {id} deleted in background job");
        }
        else
            _logger.LogError($"Error while deleting categoria {id}");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="imageStream"></param>
    /// <param name="fileName"></param>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public async Task<Result<string>> UploadImageAsync(int id, Stream imageStream, string fileName, string baseUrl)
    {
        if (imageStream == null)
        {
            var msg = "Empty image";
            _logger.LogError(msg);
            return Result.Fail(msg);
        }

        var entity = await _uow.Pavilions.GetByIdAsync(id);

        if (entity == null)
        {
            var msg = $"pavilion {id} not found";
            _logger.LogWarning(msg);
            return Result.Fail(msg);
        }

        if (!string.IsNullOrEmpty(entity.ImmaginePath))
            _imageService.DeleteImage(entity.ImmaginePath);

        var result = await _imageService.SaveImageAsync(
            nameof(Pavilion),
            imageStream,
            entity.Id.ToString(),
            Path.GetExtension(fileName));

        entity.ImmaginePath = result.Value;

        await _uow.SaveAsync();

        if (result.IsSuccess)
        {
            var url = $"{baseUrl}/{_imageService.ImagesFolder}/{entity.ImmaginePath}";

            _logger.LogInformation($"Image uploaded for pavilion {id}: {url}");

            return Result.Ok(url);
        }

        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Result<bool>> DeleteImageAsync(int id)
    {
        var entity = await _uow.Pavilions.GetByIdAsync(id);

        if (entity == null)
        {
            _logger.LogWarning($"pavilion {id} not found");
            return Result.Ok();
        }

        if (!string.IsNullOrEmpty(entity.ImmaginePath))
            _imageService.DeleteImage(entity.ImmaginePath);

        entity.ImmaginePath = null;

        await _uow.SaveAsync();

        _logger.LogInformation($"Image deleted for pavilion {id}");

        return Result.Ok();
    }
}
