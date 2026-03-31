using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Expo.Infrastructure.Persistence.Repositories;
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
                    .Include(s => s.Pavilion)
                    .Include(s => s.ExhibitionArea)
                    .Include(s => s.Categories)
                    .Include(s => s.Tags)
                    .ToListAsync();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<Stand?> GetWithRelationsAsync(int id)
    {
        return await _dbSet
                    .Include(s => s.Pavilion)
                    .Include(s => s.ExhibitionArea)
                    .Include(s => s.Categories)
                    .Include(s => s.Tags)
                    .FirstOrDefaultAsync(s => s.Id == id);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<int> CountByPavilionIdAsync(int pavilionId)
    {
        return await _dbSet.CountAsync(s => s.Pavilion.Id == pavilionId);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<int> CountByExhibitionAreaIdAsync(int exhibitionAreaId)
    {
        return await _dbSet.CountAsync(s => s.ExhibitionArea.Id == exhibitionAreaId);
    }
}
