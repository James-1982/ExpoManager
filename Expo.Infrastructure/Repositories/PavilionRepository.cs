using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repo;
using Expo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Expo.Infrastructure.Repositories;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class PavilionRepository(ApplicationDbContext context) : Repository<Pavilion>(context), IPavilionRepository
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Pavilion?> GetWithStandsAsync(int id)
    {
        return await _context.Pavilions
                             .Include(p => p.Stands)
                             .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Pavilion>> GetAllWithStandsAsync()
    {
        return await _context.Pavilions
                             .Include(p => p.Stands)
                             .ToListAsync();
    }
}