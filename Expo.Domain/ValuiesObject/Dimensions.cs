using Microsoft.Extensions.Logging.Abstractions;

namespace Expo.Domain.ValuiesObject;

/// <summary>
/// Value object representing width and length of an entity
/// </summary>
public class Dimensions
{
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
        if (width < 0)
            return;

        Width = width;
    }

    public void UpdateLength(int length)
    {
        if (length < 0)
            return;

        Length = length;
    }

    public override string ToString() => $"{Width}x{Length}";
}
