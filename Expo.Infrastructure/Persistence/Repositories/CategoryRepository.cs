using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;


namespace Expo.Infrastructure.Persistence.Repositories;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class CategoryRepository(ApplicationDbContext context) : Repository<Category>(context), ICategoryRepository
{
}
