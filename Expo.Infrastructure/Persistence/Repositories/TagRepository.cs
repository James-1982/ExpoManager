using Expo.Domain.Entities;
using Expo.Domain.Interfaces.Repositories;
using Expo.Infrastructure.Persistence;
using Expo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class TagRepository(ApplicationDbContext context) : Repository<Tag>(context), ITagRepository
{
    public async Task<Tag> GetByIdAsync(int id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags.ToListAsync();
    }

    public async Task<IEnumerable<Tag>> FindAsync(Expression<Func<Tag, bool>> predicate)
    {
        return await _context.Tags.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(Tag entity)
    {
        await _context.Tags.AddAsync(entity);
    }

    public void Update(Tag entity)
    {
        _context.Tags.Update(entity);
    }

    public void Remove(Tag entity)
    {
        _context.Tags.Remove(entity);
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        var normalized = name.Trim().ToLower();

        return await _context.Tags
            .FirstOrDefaultAsync(t => t.Name == normalized);
    }

    public async Task<List<Tag>> GetByNamesAsync(IEnumerable<string> names)
    {
        var normalized = names
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim().ToLower())
            .Distinct()
            .ToList();

        return await _context.Tags
            .Where(t => normalized.Contains(t.Name))
            .ToListAsync();
    }

    public async Task<List<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames)
    {
        if (tagNames == null) 
            return [];

        var normalized = tagNames
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        var trackedTags = _context.Tags.Local
                        .Where(t => normalized.Contains(t.Name))
                        .ToList();

        var existingNames = trackedTags.Select(t => t.Name).ToHashSet();

        // Prendi dal DB quelli non già tracciati
        var dbTags = await _context.Tags
                          .Where(t => normalized.Except(existingNames).Contains(t.Name))
                          .ToListAsync();

        existingNames.UnionWith(dbTags.Select(t => t.Name));

        // Crea solo i tag veramente nuovi
        var newTags = normalized
            .Where(n => !existingNames.Contains(n))
            .Select(n => new Tag(n))
            .ToList();

        if (newTags.Any())
            await _context.Tags.AddRangeAsync(newTags);

        return trackedTags.Concat(dbTags).Concat(newTags).ToList();
    }
}