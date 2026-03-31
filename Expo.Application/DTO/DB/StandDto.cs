namespace Expo.Application.DTO.DB;

/// <summary>
/// DTO containing data to add a new Stand
/// </summary>
public class StandInDto
{
    /// <summary>
    /// Name of the stand
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Width of the stand (optional)
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Length of the stand (optional)
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// Pavilion Id (optional)
    /// </summary>
    public int? PavilionId { get; set; }

    /// <summary>
    /// Exhibition Area Id (optional)
    /// </summary>
    public int? ExhibitionAreaId { get; set; }

    /// <summary>
    /// Description of the stand
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// List of tags
    /// </summary>
    public List<string> Tags { get; set; } = [];
}

/// <summary>
/// DTO output for Stand
/// </summary>
public class StandOutDto : StandInDto
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
    /// Associated pavilion name
    /// </summary>
    public string? PavilionName { get; set; }

    /// <summary>
    /// Associated exhibition area name
    /// </summary>
    public string? ExhibitionAreaName { get; set; }

    /// <summary>
    /// Last modify date time
    /// </summary>
    public string? LastModify { get; set; }

    /// <summary>
    /// Last user who modified the entity
    /// </summary>
    public string? ModifyBy { get; set; }
}