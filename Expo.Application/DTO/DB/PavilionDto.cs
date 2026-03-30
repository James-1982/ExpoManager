namespace Expo.Application.DTO.DB
{
/// <summary>
/// DTO containing data to add a new Pavilion
/// </summary>
public class PavilionInDto
{
    public string Name { get; set; }
    public string? Area { get; set; }
    public string? PoweredBy { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// DTO output for Pavilion
/// </summary>
public class PavilionOutDto : PavilionInDto
{
    public int Id { get; set; }
    public string? ImageUrl { get; set; }
}
}