namespace Expo.Application.DTO.DB
{
/// <summary>
/// DTO containing data to add a new Category
/// </summary>
public class CategoryInDto
{
    /// <summary>
    /// Name of the category
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of the category
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if the category is highlighted
    /// </summary>
    public bool Highlighted { get; set; }

    /// <summary>
    /// List of tags
    /// </summary>
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// DTO output for category entity
/// </summary>
public class CategoryOutDto : CategoryInDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Public URL for the associated image if present
    /// </summary>
    public string? ImageUrl { get; set; }
}
}