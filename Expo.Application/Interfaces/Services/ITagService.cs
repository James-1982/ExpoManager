using Expo.Domain.Entities;

namespace Expo.Application.Interfaces.Services;

public interface ITagService
{
    Task<List<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames);
}
