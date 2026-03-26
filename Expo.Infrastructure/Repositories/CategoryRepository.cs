using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repo;
using Expo.Infrastructure.Data;

namespace Expo.Infrastructure.Repositories;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class CategoryRepository(ApplicationDbContext context) : Repository<Category>(context), ICategoryRepository
{
}
