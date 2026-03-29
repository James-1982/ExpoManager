
using Expo.Domain.DTO.DB;
using Expo.Domain.Enums;

namespace Expo.Domain.Entities;

/// <summary>
/// Exhibition area containing stands and with a state
/// </summary>
public class ExhibitionArea : BaseEntity
{
    /// <summary>
    /// Type of the exhibition area
    /// </summary>
    public string? Type { get; private set; }

    /// <summary>
    /// Current state of the exhibition area
    /// </summary>
    public EntityState State { get; private set; } = EntityState.Undefined;

    /// <summary>
    /// Indicates if the area is highlighted
    /// </summary>
    public bool IsHighlighted { get; private set; }

    private readonly List<Stand> _stands = new();
    /// <summary>
    /// Collection of stands in this exhibition area
    /// </summary>
    public IReadOnlyCollection<Stand> Stands => _stands.AsReadOnly();

    public ExhibitionArea(string name, string? type = null, bool isHighlighted = false)
        : base(name)
    {
        Type = type;
        IsHighlighted = isHighlighted;
    }

    /// <summary>
    /// Add a stand to the exhibition area
    /// </summary>
    public void AddStand(Stand stand)
    {
        if (_stands.Contains(stand))
            throw new Exception("Stand already associated with the exhibition area");

        _stands.Add(stand);
    }

    /// <summary>
    /// Remove a stand from the exhibition area
    /// </summary>
    public void RemoveStand(Stand stand) => _stands.Remove(stand);

    /// <summary>
    /// Set the state of the exhibition area
    /// </summary>
    public void SetState(EntityState state) => State = state;

    /// <summary>
    /// Set whether the exhibition area is highlighted
    /// </summary>
    public void SetHighlight(bool value) => IsHighlighted = value;

    /// <summary>
    /// Update the type of the exhibition area
    /// </summary>
    public void UpdateType(string? type) => Type = type;
}