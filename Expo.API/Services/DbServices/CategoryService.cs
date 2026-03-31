using Expo.Application.Interfaces.Services;
using Expo.Application.DTO.DB;
using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;
using FluentResults;
using Hangfire;
using MapsterMapper;

namespace Expo.API.Services.DbServices;

internal class CategoryService(
    ILogger<CategoryService> logger,
    IMapper mapper,
    IImageService imageService,
    IBackgroundJobClient backgroundJobClient,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : ICategoryService
{
    #region Fields

    private readonly ILogger<CategoryService> _logger = logger;
    private readonly IImageService _imageService = imageService;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _uow = uow;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    private readonly ICurrentUserService _currentUser = currentUser;

    #endregion

    public async Task<Result<IList<CategoryOutDto>>> GetAllAsync(string baseUrl)
    {
        _logger.LogInformation("Fetching all categories");

        var entities = await _uow.Categories.GetAllWithRelationsAsync();
        if (entities == null || !entities.Any())
            return Result.Fail<IList<CategoryOutDto>>("No data found");

        var dtos = _mapper.From(entities)
                          .AddParameters("BaseUrl", baseUrl)
                          .AdaptToType<List<CategoryOutDto>>();

        return Result.Ok<IList<CategoryOutDto>>(dtos);
    }

    public async Task<Result<CategoryOutDto>> GetByIdAsync(int id, string baseUrl)
    {
        var entity = await _uow.Categories.GetWithRelationsAsync(id);
        if (entity == null)
        {
            _logger.LogInformation($"Category {id} not found");
            return Result.Fail<CategoryOutDto>($"Category {id} not found");
        }

        var dto = _mapper.From(entity)
                         .AddParameters("BaseUrl", baseUrl)
                         .AdaptToType<CategoryOutDto>();

        return Result.Ok(dto);
    }

    public async Task<Result<CategoryOutDto>> CreateAsync(CategoryInDto dto, string baseUrl)
    {
        try
        {
            var entity = _mapper.Map<Category>(dto);
            var tags = await _uow.Tags.GetOrCreateTagsAsync(dto.Tags);
            entity.AddTags(tags);
            entity.SetAuditInfo(_currentUser.UserName);
            await _uow.Categories.AddAsync(entity);
            await _uow.SaveAsync();

            var outDto = _mapper.From(entity)
                                .AddParameters("BaseUrl", baseUrl)
                                .AdaptToType<CategoryOutDto>();

            return Result.Ok(outDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<CategoryOutDto>(ex.Message);
        }
    }

    public async Task<Result<CategoryOutDto>> UpdateAsync(int id, CategoryInDto dto, string baseUrl)
    {
        try
        {
            var entity = await _uow.Categories.GetWithRelationsAsync(id);
            if (entity == null)
            {
                var msg = $"Category {id} not found";
                _logger.LogWarning(msg);
                return Result.Fail<CategoryOutDto>(msg);
            }

            _mapper.Map(dto, entity);

            await entity.Tags.UpdateEntityTagsAsync(dto.Tags, _uow);
            entity.SetAuditInfo(_currentUser.UserName);
            _uow.Categories.Update(entity);
            await _uow.SaveAsync();

            var outDto = _mapper.From(entity)
                                .AddParameters("BaseUrl", baseUrl)
                                .AdaptToType<CategoryOutDto>();

            return Result.Ok(outDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<CategoryOutDto>(ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        _backgroundJobClient.Enqueue(() => DeleteJob(id));
        _logger.LogInformation($"Scheduled deletion for category {id}");
    }

    public async Task DeleteJob(int id)
    {
        var entity = await _uow.Categories.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning($"Category {id} not found for deletion");
            return;
        }

        var imagePath = entity.ImagePath;

        _uow.Categories.Remove(entity);
        await _uow.SaveAsync();

        if (!string.IsNullOrEmpty(imagePath))
        {
            _logger.LogInformation($"Deleting image for category {id} in background job");
            await _imageService.DeleteImageAsync(imagePath);
        }

        _logger.LogInformation($"Category {id} deleted in background job");
    }

    public async Task<Result<string>> UploadImageAsync(int id, Stream imageStream, string fileName, string baseUrl)
    {
        if (imageStream == null)
            return Result.Fail<string>("Empty image");

        var entity = await _uow.Categories.GetByIdAsync(id);
        if (entity == null)
            return Result.Fail<string>($"Category {id} not found");

        if (!string.IsNullOrEmpty(entity.ImagePath))
            await _imageService.DeleteImageAsync(entity.ImagePath);

        var result = await _imageService.SaveImageAsync(
            nameof(Category),
            imageStream,
            entity.Id.ToString(),
            Path.GetExtension(fileName));

        if (!result.IsSuccess)
            return Result.Fail<string>(result.Errors);

        entity.SetAuditInfo(_currentUser.UserName);
        entity.UpdateImagePath(result.Value);
        await _uow.SaveAsync();

        var url = $"{baseUrl}/{_imageService.ImagesFolder}/{entity.ImagePath}";
        _logger.LogInformation($"Image uploaded for category {id}: {url}");

        return Result.Ok(url);
    }

    public async Task<Result<bool>> DeleteImageAsync(int id)
    {
        var entity = await _uow.Categories.GetByIdAsync(id);
        if (entity == null)
            return Result.Ok();

        if (!string.IsNullOrEmpty(entity.ImagePath))
            await _imageService.DeleteImageAsync(entity.ImagePath);

        entity.UpdateImagePath(null);
        await _uow.SaveAsync();

        _logger.LogInformation($"Image deleted for category {id}");
        return Result.Ok(); 
    }
}