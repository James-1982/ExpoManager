using Expo.Application.DTO.DB;
using Expo.Application.Interfaces.Services;
using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;
using FluentResults;
using Hangfire;
using MapsterMapper;
using Expo.Domain.Extensions;

namespace Expo.API.Services.DbServices;

/// <summary>
/// Service to manage Stands
/// </summary>
internal class StandService(
    ILogger<StandService> logger,
    IMapper mapper,
    IImageService imageService,
    IBackgroundJobClient backgroundJobClient,
        ICurrentUserService currentUser,
    IUnitOfWork uow) : IStandService
{
    #region Fields

    private readonly ILogger<StandService> _logger = logger;
    private readonly IImageService _imageService = imageService;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _uow = uow;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    private readonly ICurrentUserService _currentUser = currentUser;

    #endregion

    public async Task<Result<IList<StandOutDto>>> GetAllAsync(string baseUrl)
    {
        _logger.LogInformation("Fetching all stands");

        var entities = await _uow.Stands.GetAllWithRelationsAsync();
        if (entities == null || !entities.Any())
            return Result.Fail<IList<StandOutDto>>("No stands found");

        var dtos = _mapper.From(entities)
                          .AddParameters("BaseUrl", baseUrl)
                          .AdaptToType<List<StandOutDto>>();

        return Result.Ok<IList<StandOutDto>>(dtos);
    }

    public async Task<Result<StandOutDto>> GetByIdAsync(int id, string baseUrl)
    {
        _logger.LogInformation($"Fetching stand with ID {id}");
        var entity = await _uow.Stands.GetWithRelationsAsync(id);
        if (entity == null)
            return Result.Fail<StandOutDto>($"Stand with ID {id} not found");

        var dto = _mapper.From(entity)
                         .AddParameters("BaseUrl", baseUrl)
                         .AdaptToType<StandOutDto>();

        return Result.Ok(dto);
    }

    public async Task<Result<StandOutDto>> CreateAsync(StandInDto dto, string baseUrl)
    {
        try
        {
            _logger.LogInformation($"Creating new stand: {dto.Name}");

            if (dto.PavilionId.HasValue)
            {
                var pad = await _uow.Pavilions.EnsureExists(dto.PavilionId.Value, "Pavilion");
                if (pad.IsFailed) return Result.Fail<StandOutDto>(pad.Errors.First().Message);
            }

            if (dto.ExhibitionAreaId.HasValue)
            {
                var sector = await _uow.ExhibitionAreas.EnsureExists(dto.ExhibitionAreaId.Value, "ExhibitionArea");
                if (sector.IsFailed) return Result.Fail<StandOutDto>(sector.Errors.First().Message);
            }

            var entity = _mapper.Map<Stand>(dto);

            var tags = await _uow.Tags.GetOrCreateTagsAsync(dto.Tags);
            entity.AddTags(tags);

            await entity.UpdateEntityCategoriesAsync(dto.CategoryIds, _uow);

            entity.SetAuditInfo(_currentUser.UserName);
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
            return Result.Fail<StandOutDto>(ex.Message);
        }
    }

    public async Task<Result<StandOutDto>> UpdateAsync(int id, StandInDto dto, string baseUrl)
    {
        try
        {
            var entity = await _uow.Stands.GetWithRelationsAsync(id);
            if (entity == null)
            {
                var msg = $"Stand {id} not found";
                _logger.LogWarning(msg);
                return Result.Fail<StandOutDto>(msg);
            }

            _mapper.Map(dto, entity);

            if (dto.PavilionId.HasValue)
            {
                var pad = await _uow.Pavilions.EnsureExists(dto.PavilionId.Value, "Pavilion");
                if (pad.IsFailed) return Result.Fail<StandOutDto>(pad.Errors.First().Message);
                entity.ChangePavilion(pad.Value);
            }

            if (dto.ExhibitionAreaId.HasValue)
            {
                var sector = await _uow.ExhibitionAreas.EnsureExists(dto.ExhibitionAreaId.Value, "ExhibitionArea");
                if (sector.IsFailed) return Result.Fail<StandOutDto>(sector.Errors.First().Message);
                entity.ChangeExhibitionArea(sector.Value);
            }

            entity.UpdateDimensions(dto.Width, dto.Length);

            await entity.Tags.UpdateEntityTagsAsync(dto.Tags, _uow);

            await entity.UpdateEntityCategoriesAsync(dto.CategoryIds, _uow);

            entity.SetAuditInfo(_currentUser.UserName);
            _uow.Stands.Update(entity);
            await _uow.SaveAsync();
            var update = await _uow.Stands.GetWithRelationsAsync(entity.Id);

            var dtoOut = _mapper.From(update)
                                .AddParameters("BaseUrl", baseUrl)
                                .AdaptToType<StandOutDto>();

            _logger.LogInformation($"Stand {id} updated");

            return Result.Ok(dtoOut);
        }
        catch (Exception ex)
        {
            return Result.Fail<StandOutDto>(ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        _backgroundJobClient.Enqueue(() => DeleteJob(id));
        _logger.LogInformation($"Scheduled deletion for stand {id}");
    }

    public async Task DeleteJob(int id)
    {
        var entity = await _uow.Stands.GetWithRelationsAsync(id);
        if (entity == null)
        {
            _logger.LogWarning($"Stand {id} not found for deletion");
            return;
        }

        _uow.Stands.Remove(entity);
        await _uow.SaveAsync();

        // Delete image if exists
        if (!string.IsNullOrEmpty(entity.ImagePath))
        {
            _logger.LogInformation($"Deleting image for stand {id}");
            await _imageService.DeleteImageAsync(entity.ImagePath);
        }

        _logger.LogInformation($"Stand {id} deleted in background job");
    }

    public async Task<Result<string>> UploadImageAsync(int id, Stream imageStream, string fileName, string baseUrl)
    {
        if (imageStream == null)
            return Result.Fail<string>("Empty image");

        var entity = await _uow.Stands.GetByIdAsync(id);
        if (entity == null)
            return Result.Fail<string>($"Stand {id} not found");

        if (!string.IsNullOrEmpty(entity.ImagePath))
            await _imageService.DeleteImageAsync(entity.ImagePath);

        var result = await _imageService.SaveImageAsync(nameof(Stand), imageStream, entity.Id.ToString(), Path.GetExtension(fileName));
        if (result.IsFailed) return Result.Fail<string>(result.Errors.First().Message);

        entity.SetAuditInfo(_currentUser.UserName);
        entity.UpdateImagePath(result.Value);
        await _uow.SaveAsync();

        var url = $"{baseUrl}/{_imageService.ImagesFolder}/{entity.ImagePath}";
        _logger.LogInformation($"Image uploaded for stand {id}: {url}");

        return Result.Ok(url);
    }

    public async Task<Result<bool>> DeleteImageAsync(int id)
    {
        var entity = await _uow.Stands.GetByIdAsync(id);
        if (entity == null) return Result.Ok();

        if (!string.IsNullOrEmpty(entity.ImagePath))
            await _imageService.DeleteImageAsync(entity.ImagePath);

        entity.UpdateImagePath(null);
        await _uow.SaveAsync();

        _logger.LogInformation($"Image deleted for stand {id}");
        return Result.Ok();
    }
}