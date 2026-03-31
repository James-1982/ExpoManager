namespace Expo.Domain.Entities;

/// <summary>
/// Base class for all entities with common properties and tags
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Name of the entity
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Optional description
    /// </summary>
    public string? Description { get; protected set; }

    /// <summary>
    /// Optional local image path
    /// </summary>
    public string? ImagePath { get; protected set; }

    /// <summary>
    /// Last modify date time
    /// </summary>
    public DateTime? LastModify { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last user who modified the entity
    /// </summary>
    public string? ModifyBy { get; set; }

    /// <summary>
    /// Tags associated with the entity
    /// </summary>
    public List<Tag> Tags { get; set; } = [];

    protected BaseEntity() { }

    protected BaseEntity(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        Name = name;
    }

    /// <summary>
    /// Add a tag to the entity
    /// </summary>
    public void AddTag(Tag tag)
    {
        if (!Tags.Any(t => t.Name == tag.Name))
            Tags.Add(tag);
    }

    /// <summary>
    /// Adds multiple tags at once
    /// </summary>
    public void AddTags(IEnumerable<Tag> tags)
    {
        if (tags == null) return;

        foreach (var tag in tags)
            AddTag(tag);
    }

    /// <summary>
    /// Remove a tag from the entity by id
    /// </summary>
    public void RemoveTag(int tagId)
    {
        var tag = Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag != null)
        {
            Tags.Remove(tag);
        }
    }

    /// <summary>
    /// Remove a tag from the entity by name
    /// </summary>
    public void RemoveTagByName(string tagName)
    {
        var tag = Tags.FirstOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        if (tag != null)
        {
            Tags.Remove(tag);
        }
    }

    /// <summary>
    /// Update the name of the entity
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        Name = name;
    }

    /// <summary>
    /// Update the description of the entity
    /// </summary>
    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    /// <summary>
    /// Update the image path of the entity
    /// </summary>
    public void UpdateImagePath(string? path)
    {
        ImagePath = path;
    }

    /// <summary>
    /// Update Audit: user who modified and last modify date time
    /// </summary>
    public void SetAuditInfo(string userName)
    {
        ModifyBy = userName;
        LastModify = DateTime.UtcNow;
    }
}