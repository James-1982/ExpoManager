using Expo.Domain.Entities;

namespace Expo.Domain.Interfaces.Repo;

/// <summary>
/// Repository to manage 'ExhibitionHall' entity
/// </summary>
public interface IExhibitionHallRepository : IRepository<ExhibitionHall>
{
    /// <summary>
    /// Return 'ExhibitionHall' using id with related stands entities
    /// </summary>
    /// <param name="id">Entity id</param>
    /// <returns>ExhibitionHall entity</returns>
    Task<ExhibitionHall?> GetWithStandsAsync(int id);

    /// <summary>
    /// Return All ExhibitionHall entities with related stands entities
    /// </summary>
    /// <returns>Enumerable of ExhibitionHall</returns>
    Task<IEnumerable<ExhibitionHall>> GetAllWithStandsAsync();

    /// <summary>
    /// Return number of stands related to this entity
    /// </summary>
    /// <param name="hallId">Entity id</param>
    /// <returns>Number of stand placed in this ExhibitionHall</returns>
    Task<int> CountStandsBySettoreId(int hallId);
}

