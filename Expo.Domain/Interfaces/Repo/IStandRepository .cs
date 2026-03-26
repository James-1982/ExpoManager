using Expo.Domain.Entities;

namespace Expo.Domain.Interfaces.Repo;

/// <summary>
/// Repository to manage 'Stand' entity
/// </summary>
public interface IStandRepository : IRepository<Stand>
{
    /// <summary>
    /// Recupera tutti gli Stand includendo Padiglione e Settore
    /// </summary>
    Task<IEnumerable<Stand>> GetAllWithRelationsAsync();

    /// <summary>
    /// Recupera uno stand per Id includendo Padiglione e Settore
    /// </summary>
    Task<Stand?> GetWithRelationsAsync(int id);

    /// <summary>
    /// Conta quanti stand appartengono a un padiglione
    /// </summary>
    Task<int> CountByPadiglioneIdAsync(int padiglioneId);

    /// <summary>
    /// Conta quanti stand appartengono a un settore
    /// </summary>
    Task<int> CountBySettoreIdAsync(int settoreId);
}
