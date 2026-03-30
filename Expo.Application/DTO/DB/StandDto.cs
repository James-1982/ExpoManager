namespace Expo.Application.DTO.DB;

/// <summary>
/// DTO containing data to add a new Stand
/// </summary>
public class StandInDto
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Length { get; set; }
    public int? PavilionId { get; set; }
    public int? ExhibitionAreaId { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// DTO output for Stand
/// </summary>
public class StandOutDto : StandInDto
{
public int Id { get; set; }
public string? ImageUrl { get; set; }
public string? PavilionName { get; set; }
public string? ExhibitionAreaName { get; set; }
}