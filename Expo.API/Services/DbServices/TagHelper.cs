using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;

internal static class TagHelper
{
    /// <summary>
    /// Aggiorna i tag di un'entità: rimuove quelli non più presenti e aggiunge quelli nuovi
    /// </summary>
    /// <param name="entityTags">La lista dei tag dell'entità</param>
    /// <param name="tagNames">I nuovi nomi dei tag dal DTO</param>
    /// <param name="uow">UnitOfWork per creare eventuali nuovi tag</param>
    public static async Task UpdateEntityTagsAsync(
        this ICollection<Tag> entityTags,
        IEnumerable<string> tagNames,
        IUnitOfWork uow)
    {
        tagNames ??= Enumerable.Empty<string>();

        // Recupera o crea i tag dal DB
        var newTags = await uow.Tags.GetOrCreateTagsAsync(tagNames);

        // Rimuovi tag non più presenti
        var tagsToRemove = entityTags.Where(t => !newTags.Any(nt => nt.Id == t.Id)).ToList();
        foreach (var t in tagsToRemove)
            entityTags.Remove(t);

        // Aggiungi tag nuovi
        var tagsToAdd = newTags.Where(nt => !entityTags.Any(et => et.Id == nt.Id)).ToList();
        foreach (var t in tagsToAdd)
            entityTags.Add(t);
    }
}