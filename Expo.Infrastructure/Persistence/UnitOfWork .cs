using Expo.Domain.Entities;

using Expo.Domain.Interfaces.Repositories;
using Expo.Infrastructure.Persistence.Repositories;


namespace Expo.Infrastructure.Persistence;
/// <summary>
/// <inheritdoc/>
/// </summary>
/// <param name="context">Application DB context</param>
public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    /// <summary>
    /// Repository to manage Padiglioni Table
    /// </summary>
    public IPavilionRepository Pavilions { get; } = new PavilionRepository(context);
    /// <summary>
    /// Repository to manage Settori Table
    /// </summary>
    public IExhibitionAreaRepository ExhibitionAreas { get; } = new ExhibitionAreaRepository(context);
    /// <summary>
    /// Repository to manage Categorie Table
    /// </summary>
    public ICategoryRepository Categories { get; } = new CategoryRepository(context);
    /// <summary>
    /// Repository to manage Stands Table
    /// </summary>
    public IStandRepository Stands { get; } = new StandRepository(context);
    /// <summary>
    /// Repository to manage Tags Table
    /// </summary>
    public ITagRepository Tags { get; } = new TagRepository(context);
    /// <summary>
    /// Repository to manage RefreshToken
    /// </summary>
    public IRefreshTokenRepository RefreshTokens { get; } = new RefreshTokenRepository(context);
    /// <summary>
    /// Metodo to get A repository given its entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public IRepository<TEntity>? GetRepository<TEntity>() where TEntity : class
    {
        if (typeof(TEntity) == typeof(Pavilion)) return (IRepository<TEntity>)Pavilions;
        if (typeof(TEntity) == typeof(ExhibitionArea)) return (IRepository<TEntity>)ExhibitionAreas;
        if (typeof(TEntity) == typeof(Category)) return (IRepository<TEntity>)Categories;
        if (typeof(TEntity) == typeof(Stand)) return (IRepository<TEntity>)Stands;

        return null;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
}