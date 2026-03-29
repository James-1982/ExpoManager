namespace Expo.Domain.Interfaces.Repositories;

/// <summary>
/// Unit of Work pattern interface to coordinate repositories
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Repository for Pavilions
    /// </summary>
    IPavilionRepository Pavilions { get; }

    /// <summary>
    /// Repository for Exhibition Halls
    /// </summary>
    IExhibitionAreaRepository ExhibitionHalls { get; }

    /// <summary>
    /// Repository for Categories
    /// </summary>
    ICategoryRepository Categories { get; }

    /// <summary>
    /// Repository for Stands
    /// </summary>
    IStandRepository Stands { get; }

    /// <summary>
    /// Repository for Refresh Tokens
    /// </summary>
    IRefreshTokenRepository RefreshTokens { get; }

    /// <summary>
    /// Return a repository for a given entity type
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>Repository instance or null</returns>
    IRepository<TEntity>? GetRepository<TEntity>() where TEntity : class;

    /// <summary>
    /// Save changes to the database
    /// </summary>
    /// <returns>Number of affected records</returns>
    Task<int> SaveAsync();
}