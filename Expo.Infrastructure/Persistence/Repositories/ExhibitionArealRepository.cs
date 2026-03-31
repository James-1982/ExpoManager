using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Expo.Infrastructure.Persistence.Repositories;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class ExhibitionAreaRepository(ApplicationDbContext context)
    : Repository<ExhibitionArea>(context), IExhibitionAreaRepository
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ExhibitionArea?> GetWithRelationsAsync(int id)
    {
        return await _context.ExhibitionAreas
                             .Include(p => p.Stands)
                             .Include(p => p.Tags)
                             .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<ExhibitionArea>> GetAllWithRelationsAsync()
    {
        return await _context.ExhibitionAreas
                             .Include(p => p.Stands)
                             .Include(p => p.Tags)
                             .ToListAsync();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="areaId"></param>
    /// <returns></returns>
    public async Task<int> CountStandsByExhibitionAreaId(int exhibitionAreaId)
    {
        return await _context.Stands.CountAsync(s => s.ExhibitionArea.Id == exhibitionAreaId);
    }
}