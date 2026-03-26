using Expo.Domain.DTO.DB;
using Expo.Domain.DTO.User;
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
            .Ignore(dest => dest.ImmaginePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<Pavilion, PavilionOutDto>.NewConfig()
            .Map(dest => dest.ImmagineUrl,
                 src => !string.IsNullOrEmpty(src.ImmaginePath)
                        ? $"{MapContext.Current.Parameters["BaseUrl"]}/images/{src.ImmaginePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>());


        // Settore
        TypeAdapterConfig<ExhibitionHallInDto, ExhibitionHall>.NewConfig()
            .Ignore(dest => dest.ImmaginePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<ExhibitionHall, ExhibitionHallOutDto>.NewConfig()
            .Map(dest => dest.ImmagineUrl,
                 src => !string.IsNullOrEmpty(src.ImmaginePath)
                        ? $"{MapContext.Current.Parameters["BaseUrl"]}/images/{src.ImmaginePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>())
            .Map(dest => dest.NumeroStands,
                  src => (src.Stands.Count != 0)
                         ? src.Stands.Count
                         : 0);

        // Categoria
        TypeAdapterConfig<CategoryInDto, Category>.NewConfig()
            .Ignore(dest => dest.ImmaginePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<Category, CategoryOutDto>.NewConfig()
            .Map(dest => dest.ImmagineUrl,
                 src => !string.IsNullOrEmpty(src.ImmaginePath)
                        ? $"{MapContext.Current.Parameters["BaseUrl"]}/images/{src.ImmaginePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>());

        // Stand
        TypeAdapterConfig<StandInDto, Stand>.NewConfig()
            .Ignore(dest => dest.ImmaginePath)
            .Ignore(dest => dest.Id);

        TypeAdapterConfig<Stand, StandOutDto>.NewConfig()
            .Map(dest => dest.ImmagineUrl,
                 src => !string.IsNullOrEmpty(src.ImmaginePath)
                        ? $"{MapContext.Current.Parameters["BaseUrl"]}/images/{src.ImmaginePath}"
                        : null)
            .Map(dest => dest.Tags, src => src.Tags ?? new List<string>())
            .Map(dest => dest.NomePadiglione,
                  src => src.Padiglione != null
                         ? src.Padiglione.Nome
                         : string.Empty)
            .Map(dest => dest.NomeSettore,
                  src => src.Settore != null
                         ? src.Settore.Nome
                         : string.Empty);

        // User
        TypeAdapterConfig<RegisterRequestDto, RegisterUserDto>.NewConfig();
    }
}