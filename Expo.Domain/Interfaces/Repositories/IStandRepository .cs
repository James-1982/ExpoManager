using Expo.Domain.Entities;

namespace Expo.Domain.Interfaces.Repositories
{
/// <summary>
/// Repository interface to manage <see cref="Stand"/> entities
/// </summary>
public interface IStandRepository : IRepository<Stand>
{
    /// <summary>
    /// Get all stands including related Pavilion and ExhibitionArea
    /// </summary>
    /// <returns>Enumerable of Stand entities with relations</returns>
    Task<IEnumerable<Stand>> GetAllWithRelationsAsync();

    /// <summary>
    /// Get a stand by id including related Pavilion and ExhibitionArea
    /// </summary>
    /// <param name="id">Stand id</param>
    /// <returns>Stand entity or null if not found</returns>
    Task<Stand?> GetWithRelationsAsync(int id);

    /// <summary>
    /// Count how many stands belong to a given Pavilion
    /// </summary>
    /// <param name="pavilionId">Pavilion id</param>
    /// <returns>Number of stands in the Pavilion</returns>
    Task<int> CountByPavilionIdAsync(int pavilionId);

    /// <summary>
    /// Count how many stands belong to a given ExhibitionArea
    /// </summary>
    /// <param name="exhibitionAreaId">ExhibitionArea id</param>
    /// <returns>Number of stands in the ExhibitionArea</returns>
    Task<int> CountByExhibitionAreaIdAsync(int exhibitionAreaId);
}
}