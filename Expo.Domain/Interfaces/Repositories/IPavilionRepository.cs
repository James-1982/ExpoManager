using Expo.Domain.Entities;

using Expo.Domain.Interfaces.Repositories;

namespace Expo.Domain.Interfaces.Repositories
{
/// <summary>
/// Repository interface to manage <see cref="Pavilion"/> entities
/// </summary>
public interface IPavilionRepository : IRepository<Pavilion>
{
    /// <summary>
    /// Get a Pavilion by its id including related stands
    /// </summary>
    /// <param name="id">Pavilion id</param>
    /// <returns>Pavilion entity or null if not found</returns>
    Task<Pavilion?> GetWithStandsAsync(int id);

    /// <summary>
    /// Get all Pavilions including their related stands
    /// </summary>
    /// <returns>Enumerable of Pavilion entities</returns>
    Task<IEnumerable<Pavilion>> GetAllWithStandsAsync();
}
}