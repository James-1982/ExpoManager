namespace Expo.Domain.ValuiesObject;

/// <summary>
/// Value object representing width and length of an entity
/// </summary>
public class Dimensions
{
    private const int MinimumDimension = 0;

    /// <summary>
    /// Width of the entity
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Length of the entity
    /// </summary>
    public int Length { get; private set; }

    public Dimensions()
    {
    }

    public void UpdateWidth(int width)
    {
        if (width < MinimumDimension)
            throw new ArgumentException($"Width cannot be negative. Value: {width}", nameof(width));

        Width = width;
    }

    public void UpdateLength(int length)
    {
        if (length < MinimumDimension)
            throw new ArgumentException($"Length cannot be negative. Value: {length}", nameof(length));

        Length = length;
    }

    public override string ToString() => $"{Width}x{Length}";
}
