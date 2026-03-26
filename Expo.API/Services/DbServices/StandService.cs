using FluentResults;
using Hangfire;
using Expo.API.Extensions;
using Expo.Domain.DTO.DB;
using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repo;
using Expo.Domain.Interfaces.Services;
using MapsterMapper;

namespace Expo.API.Services.DbServices;

/// <summary>
/// Service to manage Stands
/// </summary>
/// <param name="logger">Logger for diagnostic purposes.</param>
/// <param name="mapper">Mapper used for object-to-object mapping.</param>
/// <param name="imageService">Service used to manage image processing and storage.</param>
/// <param name="backgroundJobClient">Background job scheduler used to run async tasks.</param>
/// <param name="uow">Unit of Work for transactional persistence.</param>
/// <remarks>
/// This constructor injects all required services for category operations.
/// </remarks>
internal class StandService(
    ILogger<StandService> logger,
    IMapper mapper,
    IImageService imageService,
    IBackgroundJobClient backgroundJobClient,
    IUnitOfWork uow) : IStandService
{
    #region Fields

    private readonly ILogger<StandService> _logger = logger;
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
    public async Task<Result<IList<StandOutDto>>> GetAllAsync(string baseUrl)
    {
        _logger.LogInformation("Fetching all pavilions");

        var entities = await _uow.Stands.GetAllWithRelationsAsync();

        if (entities == null || !entities.Any())
        {
            var msg = "No stands found";
            _logger.LogInformation(msg);
            return Result.Fail(msg);
        }

        var dtos = _mapper.From(entities)
                  .AddParameters("BaseUrl", baseUrl)
                  .AdaptToType<List<StandOutDto>>();

        return Result.Ok<IList<StandOutDto>>(dtos);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public async Task<Result<StandOutDto>> GetByIdAsync(int id, string baseUrl)
    {
        _logger.LogInformation($"Fetching stand with ID {id}");
        var entity = await _uow.Stands.GetWithRelationsAsync(id);

        if (entity == null)
        {
            var msg = $"Data with {id} not found";
            _logger.LogInformation(msg);
            return Result.Fail(msg);
        }

        var dto = _mapper.From(entity)
                 .AddParameters("BaseUrl", baseUrl)
                 .AdaptToType<StandOutDto>();

        return Result.Ok(dto);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public async Task<Result<StandOutDto>> CreateAsync(StandInDto dto, string baseUrl)
    {
        try
        {
            _logger.LogInformation($"Creating new stand: {dto.Nome}");


            if (dto.PadiglioneId.HasValue)
            {
                var pad = await _uow.Pavilions.EnsureExists(dto.PadiglioneId.Value, "pavilion");

                if (pad.IsFailed)
                    return Result.Fail(pad.Errors.FirstOrDefault().Message);
            }

            if (dto.SettoreId.HasValue)
            {
                var sector = await _uow.ExhibitionHalls.EnsureExists(dto.SettoreId.Value, "ExhibitionHall");

                if (sector.IsFailed)
                    return Result.Fail(sector.Errors.FirstOrDefault().Message);
            }

            var entity = _mapper.Map<Stand>(dto);

            await _uow.Stands.AddAsync(entity);

            await _uow.SaveAsync();

            var added = await _uow.Stands.GetWithRelationsAsync(entity.Id);

            var dtoOut = _mapper.From(added)
                      .AddParameters("BaseUrl", baseUrl)
                      .AdaptToType<StandOutDto>();

            return Result.Ok(dtoOut);
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
    public async Task<Result<StandOutDto>> UpdateAsync(int id, StandInDto dto, string baseUrl)
    {
        try
        {
            var entity = await _uow.Stands.GetWithRelationsAsync(id);

            if (entity == null)
            {
                _logger.LogWarning($"Stand with ID {id} not found for update");
                return Result.Fail($"Stand with ID {id} not found for update");
            }

            if (dto.PadiglioneId.HasValue)
            {
                var pad = await _uow.Pavilions.EnsureExists(dto.PadiglioneId.Value, "pavilion");

                if (pad.IsFailed)
                    return Result.Fail(pad.Errors.FirstOrDefault().Message);
            }

            if (dto.SettoreId.HasValue)
            {
                var sector = await _uow.ExhibitionHalls.EnsureExists(dto.SettoreId.Value, "ExhibitionHall");

                if (sector.IsFailed)
                    return Result.Fail(sector.Errors.FirstOrDefault().Message);
            }

            _mapper.Map(dto, entity);

            await _uow.SaveAsync();

            var dtoOut = _mapper.From(entity)
                      .AddParameters("BaseUrl", baseUrl)
                      .AdaptToType<StandOutDto>();

            _logger.LogInformation($"Stand {id} updated");

            return Result.Ok(dtoOut);
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

        _logger.LogInformation($"Scheduled deletion for stand {id}");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteJob(int id)
    {
        var entity = await _uow.Stands.GetByIdAsync(id);

        if (entity == null)
            return;

        _uow.Stands.Remove(entity);

        await _uow.SaveAsync();

        entity = await _uow.Stands.GetByIdAsync(id);

        if (entity == null)
        {
            if (!string.IsNullOrEmpty(entity.ImmaginePath))
            {
                _logger.LogInformation($"Deleting image for stand {id} in background job");
                _imageService.DeleteImage(entity.ImmaginePath);
            }

            _logger.LogInformation($"Stand {id} deleted in background job");
        }
        else
            _logger.LogError($"Error while deleting stand {id}");
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

        var entity = await _uow.Stands.GetByIdAsync(id);

        if (entity == null)
        {
            var msg = $"Stand {id} not found";
            _logger.LogWarning(msg);
            return Result.Fail(msg);
        }

        if (!string.IsNullOrEmpty(entity.ImmaginePath))
            _imageService.DeleteImage(entity.ImmaginePath);

        var result = await _imageService.SaveImageAsync(
            nameof(Stand),
            imageStream,
            entity.Id.ToString(),
            Path.GetExtension(fileName));

        entity.ImmaginePath = result.Value;

        await _uow.SaveAsync();

        if (result.IsSuccess)
        {
            var url = $"{baseUrl}/{_imageService.ImagesFolder}/{entity.ImmaginePath}";

            _logger.LogInformation($"Image uploaded for stand {id}: {url}");

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
        var entity = await _uow.Stands.GetByIdAsync(id);

        if (entity == null)
        {
            _logger.LogWarning($"Stand {id} not found");
            return Result.Ok();
        }

        if (!string.IsNullOrEmpty(entity.ImmaginePath))
            _imageService.DeleteImage(entity.ImmaginePath);

        entity.ImmaginePath = null;

        await _uow.SaveAsync();

        _logger.LogInformation($"Image deleted for stand {id}");

        return Result.Ok();
    }
}
