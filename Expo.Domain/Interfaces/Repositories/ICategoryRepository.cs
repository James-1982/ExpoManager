

using Expo.Domain.Entities;

namespace Expo.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface to manage <see cref="Category"/> entities
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    // Add Category-specific methods here if needed
}
