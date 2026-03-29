using Expo.Domain.Enums;

namespace Expo.Domain.DTO.DB
{
    /// <summary>
    /// DTO containing data to add a new ExhibitionArea
    /// </summary>
    public class ExhibitionAreaInDto
    {
        public string Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public EntityState? State { get; set; }
        public bool Highlighted { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// DTO output for ExhibitionArea
    /// </summary>
    public class ExhibitionAreaOutDto : ExhibitionAreaInDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public int NumberOfStands { get; set; }
    }
}