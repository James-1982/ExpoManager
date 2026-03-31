

using Expo.Domain.Entities;

namespace Expo.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface to manage <see cref="Category"/> entities
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    /// <summary>
    /// Get a Category by its id including related properties
    /// </summary>
    /// <param name="id">Category id</param>
    /// <returns>Category entity or null if not found</returns>
    Task<Category?> GetWithRelationsAsync(int id);

    /// <summary>
    /// Get all Categories including their properties
    /// </summary>
    /// <returns>Enumerable of Category entities</returns>
    Task<IEnumerable<Category>> GetAllWithRelationsAsync();
}
