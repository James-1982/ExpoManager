using Expo.Domain.Interfaces.Repositories;
using FluentResults;

namespace Expo.Domain.Extensions;

public static class RepositoryExtensions
{
    /// <summary>
    /// Ensure ad entity exists
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repo"></param>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static async Task<Result<T>> EnsureExists<T>(this IRepository<T> repo, int id, string name) where T : class
    {
        var entity = await repo.GetByIdAsync(id);

        return (entity != null
            ? Result.Ok(entity)
            : Result.Fail($"{name} with {id} not exists"));
    }
}
