using Expo.Application.DTO.DB;
using Expo.Application.DTO.User;
using Expo.Domain.Entities;
using Mapster;

namespace Expo.Application;

/// <summary>
/// Class that define mapping objects
/// </summary>
public static class MapsterConfig
{
    /// <summary>
    /// Registration of objects mapping
    /// </summary>
    public static void RegisterMappings()
    {
        // Padiglione
        TypeAdapterConfig<PavilionInDto, Pavilion>.NewConfig()
                .ConstructUsing(src => new Pavilion(src.Name, src.Area, src.PoweredBy))
                .Ignore(dest => dest.ImagePath)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Tags);

        TypeAdapterConfig<Pavilion, PavilionOutDto>.NewConfig()
                .Map(dest => dest.ImageUrl,
                     src => !string.IsNullOrEmpty(src.ImagePath)
                            ? $"{GetBaseUrl()}/images/{src.ImagePath}"
                            : null)
                .Map(dest => dest.Tags,
                     src => src.Tags != null
                            ? src.Tags.Select(t => t.Name).ToList()
                            : new List<string>());

        // ExhibitionArea
        TypeAdapterConfig<ExhibitionAreaInDto, ExhibitionArea>.NewConfig()
            .ConstructUsing(src => new ExhibitionArea(src.Name, src.Type.Value, src.Highlighted))
            .Ignore(dest => dest.ImagePath)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Tags);

        TypeAdapterConfig<ExhibitionArea, ExhibitionAreaOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{GetBaseUrl()}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.Tags,
                     src => src.Tags != null
                            ? src.Tags.Select(t => t.Name).ToList()
                            : new List<string>())
            .Map(dest => dest.NumberOfStands,
                  src => src.Stands != null ? src.Stands.Count : 0);

        // Categoria
        TypeAdapterConfig<CategoryInDto, Category>.NewConfig()
            .ConstructUsing(src => new Category(src.Name, src.Highlighted))
            .Ignore(dest => dest.ImagePath)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Tags);

        TypeAdapterConfig<Category, CategoryOutDto>.NewConfig()
                .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{GetBaseUrl()}/images/{src.ImagePath}"
                        : null)
                .Map(dest => dest.Tags,
                     src => src.Tags != null
                            ? src.Tags.Select(t => t.Name).ToList()
                            : new List<string>());

        // Stand
        TypeAdapterConfig<StandInDto, Stand>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Pavilion)
            .Ignore(dest => dest.PavilionId)
            .Ignore(dest => dest.ExhibitionArea)
            .Ignore(dest => dest.ExhibitionAreaId)
            .Ignore(dest => dest.Tags)
            .Ignore(dest => dest.Categories)
            .ConstructUsing(src => new Stand(src.Name, src.Width, src.Length));

        TypeAdapterConfig<Stand, StandOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{GetBaseUrl()}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.PavilionName, src => src.Pavilion != null ? src.Pavilion.Name : string.Empty)
            .Map(dest => dest.ExhibitionAreaName, src => src.ExhibitionArea != null ? src.ExhibitionArea.Name : string.Empty)
            .Map(dest => dest.Width, src => src.Dimensions.Width)
            .Map(dest => dest.Length, src => src.Dimensions.Length)
            .Map(dest => dest.Tags,
                 src => src.Tags != null
                        ? src.Tags.Select(t => t.Name).ToList()
                        : new List<string>())
            .Map(dest => dest.Categories,
                 src => src.Categories != null
                        ? src.Categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList()
                        : new List<CategoryDto>());

        // User
        TypeAdapterConfig<RegisterRequestDto, RegisterUserDto>.NewConfig();
    }

    /// <summary>
    /// Helper method to get base URL safely
    /// </summary>
    private static string GetBaseUrl()
    {
        try
        {
            return MapContext.Current?.Parameters?["BaseUrl"]?.ToString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}