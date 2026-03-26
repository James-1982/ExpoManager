using Expo.Domain.Interfaces.Repo;
using Expo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Expo.Infrastructure.Repositories;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
{
    /// <summary>
    /// DB context
    /// </summary>
    protected readonly ApplicationDbContext _context = context;
    /// <summary>
    /// Generic DbSet
    /// </summary>
    protected readonly DbSet<T> _dbSet = context.Set<T>();
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entity"></param>
    public void Update(T entity) => _dbSet.Update(entity);
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entity"></param>
    public void Remove(T entity) => _dbSet.Remove(entity);
}