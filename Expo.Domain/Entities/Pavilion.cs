namespace Expo.Domain.Entities;

/// <summary>
/// Entity Pavilion
/// </summary>
public class Pavilion : BaseEntity
{
    /// <summary>
    /// Name of Area
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// Sponsor
    /// </summary>
    public string? PoweredBy { get; set; }

    /// <summary>
    /// Associated Stand
    /// </summary>
    public ICollection<Stand> Stands { get; set; } = [];
}