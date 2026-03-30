namespace Expo.Domain.Entities;

/// <summary>
/// Category entity with highlighting option
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Indicates if the category is highlighted
    /// </summary>
    public bool IsHighlighted { get; private set; }

    public Category(string name, bool isHighlighted = false) : base(name)
    {
        IsHighlighted = isHighlighted;
    }

    /// <summary>
    /// Set whether the category is highlighted
    /// </summary>
    public void SetHighlight(bool value) => IsHighlighted = value;
}
