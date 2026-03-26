namespace Expo.Domain.DTO.DB;

/// <summary>
/// DTO containing data to add a new Categoria
/// </summary>
public class CategoryInDto
{
    /// <summary>
    /// Name of Entity
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// Description of Entity
    /// </summary>
    public string? Descrizione { get; set; }

    /// <summary>
    /// Define if it's Highlighted
    /// </summary>
    public bool InEvidenza { get; set; }

    /// <summary>
    /// List of tags
    /// </summary>
    public List<string> Tags { get; set; } = [];
}

/// <summary>
/// DTO output for entity Categoria
/// </summary>
public class CategoryOutDto : CategoryInDto
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
