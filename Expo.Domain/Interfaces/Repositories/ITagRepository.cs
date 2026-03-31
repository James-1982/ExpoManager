using Expo.Domain.Entities;

namespace Expo.Domain.Interfaces.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name);
    Task<List<Tag>> GetByNamesAsync(IEnumerable<string> names);
    Task<List<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames);
}