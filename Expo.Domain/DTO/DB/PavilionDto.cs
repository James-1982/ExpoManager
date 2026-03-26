namespace Expo.Domain.DTO.DB;

/// <summary>
/// DTO containing data to add a new Pavilion
/// </summary>
public class PavilionInDto
{
    /// <summary>
    /// Name of Entity
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// Area type
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// Provider sponsorship
    /// </summary>
    public string? PoweredBy { get; set; }

    /// <summary>
    /// Description of Entity
    /// </summary>
    public string? Descrizione { get; set; }

    /// <summary>
    /// List of tags
    /// </summary>
    public List<string> Tags { get; set; } = [];
}

/// <summary>
/// DTO output for entity Categoria
/// </summary>
public class PavilionOutDto : PavilionInDto
{
    /// <summary>
    /// Unique Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Public url to use to download associate image if present
    /// </summary>
    public string? ImmagineUrl { get; set; }
}