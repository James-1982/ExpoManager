using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repo;
using Expo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Expo.Infrastructure.Repositories;
/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class StandRepository(ApplicationDbContext context) : Repository<Stand>(context), IStandRepository
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Stand>> GetAllWithRelationsAsync()
    {
        return await _dbSet
                    .Include(s => s.Padiglione)
                    .Include(s => s.Settore)
                    .ToListAsync();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<Stand?> GetWithRelationsAsync(int id)
    {
        return await _dbSet
                    .Include(s => s.Padiglione)
                    .Include(s => s.Settore)
                    .FirstOrDefaultAsync(s => s.Id == id);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<int> CountByPadiglioneIdAsync(int padiglioneId)
    {
        return await _dbSet.CountAsync(s => s.PadiglioneId == padiglioneId);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<int> CountBySettoreIdAsync(int settoreId)
    {
        return await _dbSet.CountAsync(s => s.SettoreId == settoreId);
    }
}
