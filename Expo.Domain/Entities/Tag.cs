namespace Expo.Domain.Entities;

public class Tag
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    // Costruttore vuoto richiesto da EF Core
    protected Tag() { }

    public Tag(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name is required");

        Name = name.Trim().ToLower();
    }

    // Proprietà di navigazione per le relazioni Many-to-Many
    public virtual ICollection<ExhibitionArea> ExhibitionAreas { get; private set; } = [];
    public virtual ICollection<Pavilion> Pavilions { get; private set; } = [];
    public virtual ICollection<Category> Categories { get; private set; } = [];
    public virtual ICollection<Stand> Stands { get; private set; } = [];
}
