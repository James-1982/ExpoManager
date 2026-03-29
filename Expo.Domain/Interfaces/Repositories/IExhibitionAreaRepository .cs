using Expo.Domain.Entities;

using Expo.Domain.Interfaces.Repositories;

namespace Expo.Domain.Interfaces.Repositories
{
/// <summary>
/// Repository interface to manage <see cref="ExhibitionArea"/> entities
/// </summary>
public interface IExhibitionAreaRepository : IRepository<ExhibitionArea>
{
    /// <summary>
    /// Get an ExhibitionArea by its id including related stands
    /// </summary>
    /// <param name="id">ExhibitionArea id</param>
    /// <returns>ExhibitionArea entity or null if not found</returns>
    Task<ExhibitionArea?> GetWithStandsAsync(int id);

    /// <summary>
    /// Get all ExhibitionAreas including their related stands
    /// </summary>
    /// <returns>Enumerable of ExhibitionArea entities</returns>
    Task<IEnumerable<ExhibitionArea>> GetAllWithStandsAsync();

    /// <summary>
    /// Count the number of stands assigned to a given ExhibitionArea
    /// </summary>
    /// <param name="exhibitionAreaId">ExhibitionArea id</param>
    /// <returns>Number of stands in the ExhibitionArea</returns>
    Task<int> CountStandsByExhibitionAreaId(int exhibitionAreaId);
}
}