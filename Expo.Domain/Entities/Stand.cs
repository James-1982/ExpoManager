using Expo.Domain.ValuiesObject;

namespace Expo.Domain.Entities;

/// <summary>
/// Stand entity associated with a pavilion and an exhibition area
/// </summary>
public class Stand : BaseEntity
{
    public Dimensions Dimensions { get; private set; }

    // Foreign keys
    public int? PavilionId { get; private set; }
    public int? ExhibitionAreaId { get; private set; }

    // Navigation properties
    public Pavilion Pavilion { get; private set; }
    public ExhibitionArea ExhibitionArea { get; private set; }

    // Costruttore vuoto richiesto da EF Core
    protected Stand() : base() { }

    // Costruttore minimale: solo nome e dimensioni
    public Stand(string name, int width, int length) : base(name)
    {
        Dimensions = new Dimensions(width, length);
    }

    // Metodi per aggiornare i riferimenti
    public void ChangePavilion(Pavilion newPavilion)
    {
        Pavilion = newPavilion ?? throw new ArgumentNullException(nameof(newPavilion));
        PavilionId = newPavilion.Id;
    }

    public void ChangeExhibitionArea(ExhibitionArea newArea)
    {
        ExhibitionArea = newArea ?? throw new ArgumentNullException(nameof(newArea));
        ExhibitionAreaId = newArea.Id;
    }

    public void UpdateDimensions(Dimensions newDimensions)
    {
        Dimensions = newDimensions ?? throw new ArgumentNullException(nameof(newDimensions));
    }
}