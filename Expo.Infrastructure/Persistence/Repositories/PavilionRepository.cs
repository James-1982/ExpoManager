using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Expo.Infrastructure.Persistence.Repositories;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class PavilionRepository(ApplicationDbContext context) : Repository<Pavilion>(context), IPavilionRepository
{
    public async Task<Pavilion?> GetWithRelationsAsync(int id)
    {
        return await _context.Pavilions
            .Include(p => p.Tags)
            .Include(p => p.Stands)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pavilion>> GetAllWithRelationsAsync()
    {
        return await _context.Pavilions
            .Include(p => p.Tags)
            .Include(p => p.Stands)
            .ToListAsync();
    }
}