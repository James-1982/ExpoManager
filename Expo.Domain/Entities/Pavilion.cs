namespace Expo.Domain.Entities;

/// <summary>
/// Pavilion entity containing stands
/// </summary>
public class Pavilion : BaseEntity
{
    /// <summary>
    /// Area name
    /// </summary>
    public string? Area { get; private set; }

    /// <summary>
    /// Sponsor or powered by information
    /// </summary>
    public string? PoweredBy { get; private set; }

    private readonly List<Stand> _stands = [];
    /// <summary>
    /// Collection of stands associated with the pavilion
    /// </summary>
    public IReadOnlyCollection<Stand> Stands => _stands.AsReadOnly();

    public Pavilion(string name, string? area = null, string? poweredBy = null) : base(name)
    {
        Area = area;
        PoweredBy = poweredBy;
    }

    public void AddStand(Stand stand)
    {
        if (_stands.Contains(stand))
            throw new Exception("Stand already exists in pavilion");

        _stands.Add(stand);
    }

    public void RemoveStand(Stand stand)
    {
        _stands.Remove(stand);
    }

    public void UpdateArea(string? area) => Area = area;

    public void UpdatePoweredBy(string? poweredBy) => PoweredBy = poweredBy;
}