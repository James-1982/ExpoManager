namespace Expo.Domain.DTO.DB;

/// <summary>
/// State of 'ExhibitionHall' Entity
/// </summary>
public enum EntityState
{
    /// <summary>
    /// Undefined
    /// </summary>
    Undefined,
    /// <summary>
    /// Generated
    /// </summary>
    Generato,
    /// <summary>
    /// Draft
    /// </summary>
    Bozza,
    /// <summary>
    /// Suspending
    /// </summary>
    InSospeso,
    /// <summary>
    /// Approved
    /// </summary>
    Approvato
}

/// <summary>
/// DTO containing data to add a new Settore
/// </summary>
public class ExhibitionHallInDto
{
    /// <summary>
    /// Name of Entity
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// Type of ExhibitionHall
    /// </summary>
    public string? Tipo { get; set; }

    /// <summary>
    /// Description of Entity
    /// </summary>
    public string? Descrizione { get; set; }

    /// <summary>
    /// State of entity
    /// </summary>
    public EntityState? Stato { get; set; }

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
/// DTO output for entity ExhibitionHall
/// </summary>
public class ExhibitionHallOutDto : ExhibitionHallInDto
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
    /// Number of Stands for this ExhibitionHall
    /// </summary>
    public int NumeroStands { get; set; }
}