using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Expo.Infrastructure.Persistence.Repositories;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class CategoryRepository(ApplicationDbContext context) : Repository<Category>(context), ICategoryRepository
{
    /// <summary>
    /// Recupera una categoria con i tag inclusi
    /// </summary>
    public async Task<Category?> GetWithRelationsAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Recupera tutte le categorie con i tag inclusi
    /// </summary>
    public async Task<IEnumerable<Category>> GetAllWithRelationsAsync()
    {
        return await _context.Categories
            .Include(c => c.Tags)
            .ToListAsync();
    }
}
