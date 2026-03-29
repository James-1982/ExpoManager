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
    /// Tags associated with the entity
    /// </summary>
    public List<string> Tags {get; set;}

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
    public void AddTag(string tag)
    {
        if (!Tags.Contains(tag))
            Tags.Add(tag);
    }

    /// <summary>
    /// Remove a tag from the entity
    /// </summary>
    public void RemoveTag(string tag)
    {
        Tags.Remove(tag);
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
    public void UpdateImmaginePath(string? path)
    {
        ImagePath = path;
    }
}