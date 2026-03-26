using System.Linq.Expressions;

namespace Expo.Domain.Interfaces.Repo;

/// <summary>
/// Generics repositoty
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get Entity By id
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <returns></returns>
    Task<T> GetByIdAsync(int id);

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>All requested entities</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Find entity using predicate
    /// </summary>
    /// <param name="predicate">Expression predicate</param>
    /// <returns>Enumerable entities</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Add a new entity
    /// </summary>
    /// <param name="entity">Entiy to add</param>
    /// <returns></returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Update an existing entity
    /// </summary>
    /// <param name="entity">Entiy to update</param>
    void Update(T entity);

    /// <summary>
    /// remove entity
    /// </summary>
    /// <param name="entity">Entity to remove</param>
    void Remove(T entity);
}
