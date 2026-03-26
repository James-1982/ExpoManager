namespace Expo.Domain.DTO.DB;

/// <summary>
/// DTO containing data to add a new Stand
/// </summary>
public class StandInDto
{
    /// <summary>
    /// Name of Entity
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// If already assigned, the id of the associated Pavilion
    /// </summary>
    public int? PadiglioneId { get; set; }

    /// <summary>
    /// If already assigned, the id of the associated Section
    /// </summary>
    public int? SettoreId { get; set; }

    /// <summary>
    /// Size of stand (3x3, 4x4)
    /// </summary>
    public string? Dimensioni { get; set; }

    /// <summary>
    /// Description of Entity
    /// </summary>
    public string? Descrizione { get; set; }

    /// <summary>
    /// List of tags
    /// </summary>
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// DTO output for entity Stand
/// </summary>
public class StandOutDto : StandInDto
{
    /// <summary>
    /// Unique Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Public url to use to download associate image if present
    /// </summary>
    public string? ImmagineUrl { get; set; }

    /// <summary>
    /// If already assigned, the name of the associated Pavilion
    /// </summary>
    public string? NomePadiglione { get; set; }

    /// <summary>
    /// If already assigned, the name of the associated Section
    /// </summary>
    public string? NomeSettore { get; set; }


}
