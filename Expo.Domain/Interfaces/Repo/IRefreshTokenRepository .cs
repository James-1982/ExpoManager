using Expo.Domain.Entities;
using System.Linq.Expressions;

namespace Expo.Domain.Interfaces.Repo;

/// <summary>
/// Repository to manage Refresh token of logged user
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    /// <summary>
    /// Return the refresh token by token
    /// </summary>
    /// <param name="token">The requested refresh token</param>
    /// <returns></returns>
    Task<RefreshToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Return all refresh token by expression
    /// </summary>
    /// <param name="predicate">Expression predicate</param>
    /// <returns>IEnumerable of RefreshToken</returns>
    Task<IEnumerable<RefreshToken>> GetAllAsync(Expression<Func<RefreshToken, bool>> predicate);
}
