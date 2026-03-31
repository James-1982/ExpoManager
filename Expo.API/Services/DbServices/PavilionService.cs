using Expo.Application.Interfaces.Services;
using Expo.Application.DTO.DB;
using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;
using FluentResults;
using Hangfire;
using MapsterMapper;

internal class PavilionService(
    ILogger<PavilionService> logger,
    IMapper mapper,
    IImageService imageService,
    IBackgroundJobClient backgroundJobClient,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IPavilionService
{
    private readonly ILogger<PavilionService> _logger = logger;
    private readonly IImageService _imageService = imageService;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _uow = uow;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    private readonly ICurrentUserService _currentUser = currentUser;
    public async Task<Result<IList<PavilionOutDto>>> GetAllAsync(string baseUrl)
    {
        _logger.LogInformation("Fetching all pavilions");

        var entities = await _uow.Pavilions.GetAllWithRelationsAsync();
        if (entities == null || !entities.Any())
            return Result.Fail<IList<PavilionOutDto>>("No data found");

        var dtos = _mapper.From(entities)
                          .AddParameters("BaseUrl", baseUrl)
                          .AdaptToType<List<PavilionOutDto>>();

        return Result.Ok<IList<PavilionOutDto>>(dtos);
    }

    public async Task<Result<PavilionOutDto>> GetByIdAsync(int id, string baseUrl)
    {
        var entity = await _uow.Pavilions.GetWithRelationsAsync(id);
        if (entity == null)
            return Result.Fail<PavilionOutDto>($"Pavilion {id} not found");

        var dto = _mapper.From(entity)
                         .AddParameters("BaseUrl", baseUrl)
                         .AdaptToType<PavilionOutDto>();

        return Result.Ok(dto);
    }

    public async Task<Result<PavilionOutDto>> CreateAsync(PavilionInDto dto, string baseUrl)
    {
        try
        {
            var entity = _mapper.Map<Pavilion>(dto);
            var tags = await _uow.Tags.GetOrCreateTagsAsync(dto.Tags);
            entity.AddTags(tags);
            entity.SetAuditInfo(_currentUser.UserName);
            await _uow.Pavilions.AddAsync(entity);
            await _uow.SaveAsync();

            var outDto = _mapper.From(entity)
                                .AddParameters("BaseUrl", baseUrl)
                                .AdaptToType<PavilionOutDto>();

            return Result.Ok(outDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<PavilionOutDto>(ex.Message);
        }
    }

    public async Task<Result<PavilionOutDto>> UpdateAsync(int id, PavilionInDto dto, string baseUrl)
    {
        try
        {
            var entity = await _uow.Pavilions.GetWithRelationsAsync(id);
            if (entity == null)
            {
                var msg = $"Pavilion {id} not found";
                _logger.LogWarning(msg);
                return Result.Fail<PavilionOutDto>(msg);
            }

            _mapper.Map(dto, entity);

            await entity.Tags.UpdateEntityTagsAsync(dto.Tags, _uow);
            entity.SetAuditInfo(_currentUser.UserName);
            uow.Pavilions.Update(entity);

            await _uow.SaveAsync();

            var outDto = _mapper.From(entity)
                                .AddParameters("BaseUrl", baseUrl)
                                .AdaptToType<PavilionOutDto>();

            return Result.Ok(outDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<PavilionOutDto>(ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        _backgroundJobClient.Enqueue(() => DeleteJob(id));
        _logger.LogInformation($"Scheduled deletion for pavilion {id}");
    }

    public async Task DeleteJob(int id)
    {
        var entity = await _uow.Pavilions.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning($"Pavilion {id} not found for deletion");
            return;
        }

        if (!string.IsNullOrEmpty(entity.ImagePath))
        {
            _logger.LogInformation($"Deleting image for pavilion {id}");
            await _imageService.DeleteImageAsync(entity.ImagePath);
        }

        _uow.Pavilions.Remove(entity);
        await _uow.SaveAsync();

        _logger.LogInformation($"Pavilion {id} deleted in background job");
    }

    public async Task<Result<string>> UploadImageAsync(int id, Stream imageStream, string fileName, string baseUrl)
    {
        if (imageStream == null)
            return Result.Fail<string>("Empty image");

        var entity = await _uow.Pavilions.GetByIdAsync(id);
        if (entity == null)
            return Result.Fail<string>($"Pavilion {id} not found");

        if (!string.IsNullOrEmpty(entity.ImagePath))
            await _imageService.DeleteImageAsync(entity.ImagePath);

        var result = await _imageService.SaveImageAsync(
            nameof(Pavilion),
            imageStream,
            entity.Id.ToString(),
            Path.GetExtension(fileName));

        if (result.IsFailed) return Result.Fail<string>(result.Errors.First().Message);

        entity.SetAuditInfo(_currentUser.UserName);
        entity.UpdateImagePath(result.Value);
        await _uow.SaveAsync();

        var url = $"{baseUrl}/{_imageService.ImagesFolder}/{entity.ImagePath}";
        _logger.LogInformation($"Image uploaded for pavilion {id}: {url}");

        return Result.Ok(url);
    }

    public async Task<Result<bool>> DeleteImageAsync(int id)
    {
        var entity = await _uow.Pavilions.GetByIdAsync(id);
        if (entity == null)
            return Result.Ok();

        if (!string.IsNullOrEmpty(entity.ImagePath))
            await _imageService.DeleteImageAsync(entity.ImagePath);

        entity.UpdateImagePath(null);
        await _uow.SaveAsync();

        _logger.LogInformation($"Image deleted for pavilion {id}");
        return Result.Ok();
    }
}