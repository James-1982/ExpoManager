using Expo.Domain.Enums;

namespace Expo.Application.DTO.DB;

/// <summary>
/// DTO containing data to add a new ExhibitionArea
/// </summary>
public class ExhibitionAreaInDto
{
    /// <summary>
    /// Name of the exhibition area
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of the exhibition area
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Description of the exhibition area
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Current state of the exhibition area
    /// </summary>
    public EntityState? State { get; set; }

    /// <summary>
    /// Indicates if the exhibition area is highlighted
    /// </summary>
    public bool Highlighted { get; set; }

    /// <summary>
    /// List of tags
    /// </summary>
    public List<string> Tags { get; set; } = [];
}

/// <summary>
/// DTO output for ExhibitionArea
/// </summary>
public class ExhibitionAreaOutDto : ExhibitionAreaInDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Public URL for the associated image if present
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Number of stands in this exhibition area
    /// </summary>
    public int NumberOfStands { get; set; }
}