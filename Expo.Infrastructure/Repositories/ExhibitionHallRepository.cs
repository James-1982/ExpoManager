using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repo;
using Expo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Expo.Infrastructure.Repositories;
/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class ExhibitionHallRepository(ApplicationDbContext context) : Repository<ExhibitionHall>(context), IExhibitionHallRepository
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ExhibitionHall?> GetWithStandsAsync(int id)
    {
        return await _context.ExhibitionHalls
                             .Include(p => p.Stands)
                             .FirstOrDefaultAsync(p => p.Id == id);
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<ExhibitionHall>> GetAllWithStandsAsync()
    {
        return await _context.ExhibitionHalls
                             .Include(p => p.Stands)
                             .ToListAsync();
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="settoreId"></param>
    /// <returns></returns>
    public async Task<int> CountStandsBySettoreId(int settoreId)
    {
        return await _context.Stands.CountAsync(s => s.SettoreId == settoreId);
    }
}