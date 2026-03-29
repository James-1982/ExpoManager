using System.Linq.Expressions;

namespace Expo.Domain.Interfaces.Repositories
{
/// <summary>
/// Generic repository interface defining basic CRUD operations
/// </summary>
/// <typeparam name="T">Type of entity</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get an entity by its identifier
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <returns>The entity</returns>
    Task<T> GetByIdAsync(int id);

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>All entities</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Find entities matching a given predicate
    /// </summary>
    /// <param name="predicate">Expression predicate</param>
    /// <returns>Enumerable of matching entities</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Add a new entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Task</returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Update an existing entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    void Update(T entity);

    /// <summary>
    /// Remove an entity
    /// </summary>
    /// <param name="entity">Entity to remove</param>
    void Remove(T entity);
}
}