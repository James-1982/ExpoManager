using FluentResults;
using Hangfire;
using Expo.Application.DTO.DB;
using Expo.Domain.Entities;
using MapsterMapper;
using Expo.Application.Interfaces.Services;
using Expo.Domain.Interfaces.Repositories;

namespace Expo.API.Services.DbServices;

/// <summary>
/// Service to manage Exhibition Area (Sectors)
/// </summary>
internal class ExhibitionAreaService(
    ILogger<ExhibitionAreaService> logger,
    IMapper mapper,
    IImageService imageService,
    IBackgroundJobClient backgroundJobClient,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IExhibitionAreaService
{
    private readonly ILogger<ExhibitionAreaService> _logger = logger;
    private readonly IImageService _imageService = imageService;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _uow = uow;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<Result<IList<ExhibitionAreaOutDto>>> GetAllAsync(string baseUrl)
    {
        _logger.LogInformation("Fetching all sectors");

        var entities = await _uow.ExhibitionAreas.GetAllWithRelationsAsync();

        if (entities == null || !entities.Any())
            return Result.Fail<IList<ExhibitionAreaOutDto>>("No data found");

        var dtos = _mapper.From(entities)
                          .AddParameters("BaseUrl", baseUrl)
                          .AdaptToType<List<ExhibitionAreaOutDto>>();

        return Result.Ok<IList<ExhibitionAreaOutDto>>(dtos);
    }

    public async Task<Result<ExhibitionAreaOutDto>> GetByIdAsync(int id, string baseUrl)
    {
        var entity = await _uow.ExhibitionAreas.GetWithRelationsAsync(id);
        if (entity == null)
        {
            _logger.LogInformation($"Sector {id} not found");
            return Result.Fail<ExhibitionAreaOutDto>($"Sector {id} not found");
        }

        var dto = _mapper.From(entity)
                         .AddParameters("BaseUrl", baseUrl)
                         .AdaptToType<ExhibitionAreaOutDto>();

        return Result.Ok(dto);
    }

    public async Task<Result<ExhibitionAreaOutDto>> CreateAsync(ExhibitionAreaInDto dto, string baseUrl)
    {
        try
        {
            var entity = _mapper.Map<ExhibitionArea>(dto);
            var tags = await _uow.Tags.GetOrCreateTagsAsync(dto.Tags);
            entity.AddTags(tags);
            entity.SetAuditInfo(_currentUser.UserName);
            await _uow.ExhibitionAreas.AddAsync(entity);
            await _uow.SaveAsync();

            var outDto = _mapper.From(entity)
                                .AddParameters("BaseUrl", baseUrl)
                                .AdaptToType<ExhibitionAreaOutDto>();

            return Result.Ok(outDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<ExhibitionAreaOutDto>(ex.Message);
        }
    }

    public async Task<Result<ExhibitionAreaOutDto>> UpdateAsync(int id, ExhibitionAreaInDto dto, string baseUrl)
    {
        try
        {
            var entity = await _uow.ExhibitionAreas.GetWithRelationsAsync(id);
            if (entity == null)
            {
                var msg = $"Sector {id} not found";
                _logger.LogWarning(msg);
                return Result.Fail<ExhibitionAreaOutDto>(msg);
            }

            _mapper.Map(dto, entity);

            await entity.Tags.UpdateEntityTagsAsync(dto.Tags, _uow);
            entity.SetAuditInfo(_currentUser.UserName);
            _uow.ExhibitionAreas.Update(entity);

            await _uow.SaveAsync();

            var outDto = _mapper.From(entity)
                                .AddParameters("BaseUrl", baseUrl)
                                .AdaptToType<ExhibitionAreaOutDto>();

            return Result.Ok(outDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<ExhibitionAreaOutDto>(ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        _backgroundJobClient.Enqueue(() => DeleteJob(id));
        _logger.LogInformation($"Scheduled deletion for sector {id}");
    }

    public async Task DeleteJob(int id)
    {
        var entity = await _uow.ExhibitionAreas.GetByIdAsync(id);
        if (entity == null)
            return;

        var imagePath = entity.ImagePath; // salva il path prima della cancellazione

        _uow.ExhibitionAreas.Remove(entity);
        await _uow.SaveAsync();

        if (!string.IsNullOrEmpty(imagePath))
        {
            _logger.LogInformation($"Deleting image for sector {id} in background job");
            await _imageService.DeleteImageAsync(imagePath);
        }

        _logger.LogInformation($"Sector {id} deleted in background job");
    }

    public async Task<Result<string>> UploadImageAsync(int id, Stream imageStream, string fileName, string baseUrl)
    {
        if (imageStream == null)
        {
            var msg = "Empty image";
            _logger.LogError(msg);
            return Result.Fail<string>(msg);
        }

        var entity = await _uow.ExhibitionAreas.GetByIdAsync(id);
        if (entity == null)
        {
            var msg = $"Sector {id} not found";
            _logger.LogWarning(msg);
            return Result.Fail<string>(msg);
        }

        if (!string.IsNullOrEmpty(entity.ImagePath))
            await _imageService.DeleteImageAsync(entity.ImagePath);

        var result = await _imageService.SaveImageAsync(
            nameof(ExhibitionArea),
            imageStream,
            entity.Id.ToString(),
            Path.GetExtension(fileName));

        if (result.IsSuccess)
        {
            entity.UpdateImagePath(result.Value);
            entity.SetAuditInfo(_currentUser.UserName);
            await _uow.SaveAsync();

            var url = $"{baseUrl}/{_imageService.ImagesFolder}/{entity.ImagePath}";
            _logger.LogInformation($"Image uploaded for sector {id}: {url}");

            return Result.Ok(url);
        }

        return Result.Fail<string>(result.Errors.FirstOrDefault()?.Message ?? "Error saving image");
    }

    public async Task<Result<bool>> DeleteImageAsync(int id)
    {
        var entity = await _uow.ExhibitionAreas.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning($"Sector {id} not found");
            return Result.Ok(); 
        }

        if (!string.IsNullOrEmpty(entity.ImagePath))
            await _imageService.DeleteImageAsync(entity.ImagePath);

        entity.UpdateImagePath(null);
        await _uow.SaveAsync();

        _logger.LogInformation($"Image deleted for sector {id}");
        return Result.Ok();
    }
}