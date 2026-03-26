namespace Expo.Domain.Entities;

/// <summary>
/// Base shared property for DB entiyt
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name
    /// </summary>

    public string Nome { get; set; }

    /// <summary>
    /// Description
    /// </summary>

    public string? Descrizione { get; set; }

    /// <summary>
    /// Loacal image path
    /// </summary>

    public string? ImmaginePath { get; set; }

    /// <summary>
    /// List of tag
    /// </summary>

    public List<string> Tags { get; set; } = [];
}