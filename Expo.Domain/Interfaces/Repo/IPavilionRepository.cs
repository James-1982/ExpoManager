using Expo.Domain.Entities;

namespace Expo.Domain.Interfaces.Repo;

/// <summary>
/// Repository to manage 'Pavilion' entity
/// </summary>
public interface IPavilionRepository : IRepository<Pavilion>
{
    /// <summary>
    /// Return Pavilion using id with related stands entities
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <returns>Entity Pavilion</returns>
    Task<Pavilion?> GetWithStandsAsync(int id);

    /// <summary>
    /// Return All Pavilion entities with related stands entities
    /// </summary>
    /// <returns>IEnumerabel of Pavilion</returns>
    Task<IEnumerable<Pavilion>> GetAllWithStandsAsync();
}
