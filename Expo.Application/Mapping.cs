using Expo.Application.DTO.DB;
using Expo.Application.DTO.User;
using Expo.Domain.Entities;
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
            .Ignore(dest => dest.ImagePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<Pavilion, PavilionOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{MapContext.Current.Parameters["BaseUrl"]}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>());

        // ExhibitionArea
        TypeAdapterConfig<ExhibitionAreaInDto, ExhibitionArea>.NewConfig()
            .Ignore(dest => dest.ImagePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<ExhibitionArea, ExhibitionAreaOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{MapContext.Current.Parameters["BaseUrl"]}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>())
            .Map(dest => dest.NumberOfStands,
                  src => src.Stands != null ? src.Stands.Count : 0);

        // Categoria
        TypeAdapterConfig<CategoryInDto, Category>.NewConfig()
            .Ignore(dest => dest.ImagePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<Category, CategoryOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{MapContext.Current.Parameters["BaseUrl"]}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>());

        // Stand
        TypeAdapterConfig<StandInDto, Stand>.NewConfig()
            .Ignore(dest => dest.ImagePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<Stand, StandOutDto>.NewConfig()
            .Map(dest => dest.ImageUrl,
                 src => !string.IsNullOrEmpty(src.ImagePath)
                        ? $"{MapContext.Current.Parameters["BaseUrl"]}/images/{src.ImagePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>())
            .Map(dest => dest.PavilionName,
                  src => src.Pavilion != null ? src.Pavilion.Name : string.Empty)
            .Map(dest => dest.ExhibitionHallName,
                  src => src.ExhibitionArea != null ? src.ExhibitionArea.Name : string.Empty);

        // User
        TypeAdapterConfig<RegisterRequestDto, RegisterUserDto>.NewConfig();
    }
}