namespace Expo.Domain.Entities;

/// <summary>
/// Entity Stand
/// </summary>
public class Stand : BaseEntity
{
    /// <summary>
    /// Dimension
    /// </summary>
    public string? Dimensioni { get; set; }

    /// <summary>
    /// Id of associated Padiglione
    /// </summary>
    public int? PadiglioneId { get; set; }

    /// <summary>
    /// Associated Padiglione
    /// </summary>
    public Pavilion? Padiglione { get; set; }

    /// <summary>
    /// Id of associated Settore
    /// </summary>
    public int? SettoreId { get; set; }

    /// <summary>
    /// Associated Settore
    /// </summary>
    public ExhibitionHall? Settore { get; set; }
}