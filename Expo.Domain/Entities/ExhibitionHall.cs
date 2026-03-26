
using Expo.Domain.DTO.DB;

namespace Expo.Domain.Entities;

/// <summary>
/// Entity Exhibition Hall
/// </summary>
public class ExhibitionHall : BaseEntity
{
    /// <summary>
    /// Type of Settore
    /// </summary>
    public string? Tipo { get; set; }

    /// <summary>
    /// State of entity
    /// </summary>
    public EntityState? Stato { get; set; }

    /// <summary>
    /// Highlighted
    /// </summary>
    public bool InEvidenza { get; set; }

    /// <summary>
    /// Associated Stand
    /// </summary>
    public ICollection<Stand> Stands { get; set; } = [];
}