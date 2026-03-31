namespace Expo.Application.DTO.DB
{
    /// <summary>
    /// DTO containing data to add a new Pavilion
    /// </summary>
    public class PavilionInDto
    {
        /// <summary>
        /// Name of the pavilion
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Area name
        /// </summary>
        public string? Area { get; set; }

        /// <summary>
        /// Sponsor or powered by information
        /// </summary>
        public string? PoweredBy { get; set; }

        /// <summary>
        /// Description of the pavilion
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// List of tags
        /// </summary>
        public List<string> Tags { get; set; } = [];
    }

    /// <summary>
    /// DTO output for Pavilion
    /// </summary>
    public class PavilionOutDto : PavilionInDto
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
        /// Last modify date time
        /// </summary>
        public string? LastModify { get; set; }

        /// <summary>
        /// Last user who modified the entity
        /// </summary>
        public string? ModifyBy { get; set; }
    }
}