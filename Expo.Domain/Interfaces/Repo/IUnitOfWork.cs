namespace Expo.Domain.Interfaces.Repo;

/// <summary>
/// Unit Of Work Pattern
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Pavilions repositoy
    /// </summary>
    IPavilionRepository Pavilions { get; }

    /// <summary>
    /// ExhibitionHalls repositoy
    /// </summary>
    IExhibitionHallRepository ExhibitionHalls { get; }

    /// <summary>
    /// Categories repositoy
    /// </summary>
    ICategoryRepository Categories { get; }

    /// <summary>
    /// Stands repositoy
    /// </summary>
    IStandRepository Stands { get; }

    /// <summary>
    /// Return a repository given its entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    IRepository<TEntity>? GetRepository<TEntity>() where TEntity : class;

    /// <summary>
    /// RefreshToken repositoy
    /// </summary>
    IRefreshTokenRepository RefreshToken { get; }

    /// <summary>
    /// Save changes
    /// </summary>
    /// <returns></returns>
    Task<int> SaveAsync();
}