using Expo.Application.DTO.DB;
using Expo.Application.DTO.User;
using Expo.Domain.Entities;
using Expo.Domain.ValuiesObject;
using Mapster;

namespace Expo.API.Utils;

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
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<Pavilion, PavilionOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{GetBaseUrl()}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>());

        // ExhibitionArea
        TypeAdapterConfig<ExhibitionAreaInDto, ExhibitionArea>.NewConfig()
            .ConstructUsing(src => new ExhibitionArea(src.Name, src.Type, src.Highlighted))
            .Ignore(dest => dest.ImagePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<ExhibitionArea, ExhibitionAreaOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{GetBaseUrl()}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>())
            .Map(dest => dest.NumberOfStands,
                  src => src.Stands != null ? src.Stands.Count : 0);

        // Categoria
        TypeAdapterConfig<CategoryInDto, Category>.NewConfig()
            .ConstructUsing(src => new Category(src.Name, src.Highlighted))
            .Ignore(dest => dest.ImagePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<Category, CategoryOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{GetBaseUrl()}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>());

        // Stand
        TypeAdapterConfig<StandInDto, Stand>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Pavilion)
            .Ignore(dest => dest.PavilionId)
            .Ignore(dest => dest.ExhibitionArea)
            .Ignore(dest => dest.ExhibitionAreaId)
            .ConstructUsing(src => new Stand(src.Name, src.Width, src.Length));

        TypeAdapterConfig<Stand, StandOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{GetBaseUrl()}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>())
            .Map(dest => dest.PavilionName, src => src.Pavilion != null ? src.Pavilion.Name : string.Empty)
            .Map(dest => dest.ExhibitionAreaName, src => src.ExhibitionArea != null ? src.ExhibitionArea.Name : string.Empty);

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