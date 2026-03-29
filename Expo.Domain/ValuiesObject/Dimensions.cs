namespace Expo.Domain.ValuiesObject;

/// <summary>
/// Value object representing width and length of an entity
/// </summary>
public class Dimensions
{
    /// <summary>
    /// Width of the entity
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Length of the entity
    /// </summary>
    public int Length { get; }

    public Dimensions(int width, int length)
    {
        if (width <= 0 || length <= 0)
            throw new ArgumentException("Invalid dimensions");

        Width = width;
        Length = length;
    }

    public override string ToString() => $"{Width}x{Length}";
}
