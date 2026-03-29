using Expo.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Expo.Infrastructure.Persistence;

/// <summary>
/// Application DB context
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    // DbSets
    public DbSet<Pavilion> Pavilions { get; set; } = null!;
    public DbSet<ExhibitionArea> ExhibitionAreas { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Stand> Stands { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ValueConverter for Tags
        var tagsConverter = new ValueConverter<List<string>, string>(
            v => string.Join(',', v),
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => t.Trim())
                  .ToList()
        );

        var tagsComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
        );

        void ConfigureTags<TEntity>() where TEntity : BaseEntity
        {
            builder.Entity<TEntity>()
                   .Property(e => e.Tags)
                   .HasConversion(tagsConverter)
                   .Metadata.SetValueComparer(tagsComparer);
        }

        ConfigureTags<Pavilion>();
        ConfigureTags<ExhibitionArea>();
        ConfigureTags<Category>();
        ConfigureTags<Stand>();

        // Pavilion
        builder.Entity<Pavilion>(entity =>
        {
            entity.Property(p => p.Name).IsRequired();
            entity.Property(p => p.Area).HasMaxLength(200);
            entity.Property(p => p.PoweredBy).HasMaxLength(200);

            entity.HasMany(p => p.Stands)
                  .WithOne(s => s.Pavilion)
                  .HasForeignKey(s => s.PavilionId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ExhibitionArea
        builder.Entity<ExhibitionArea>(entity =>
        {
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Type).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);

            entity.HasMany(e => e.Stands)
                  .WithOne(s => s.ExhibitionArea)
                  .HasForeignKey(s => s.ExhibitionAreaId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Category
        builder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).IsRequired();
            entity.Property(c => c.IsHighlighted).IsRequired();
        });

        // Stand
        builder.Entity<Stand>(entity =>
        {
            entity.Property(s => s.Name).IsRequired();
            entity.OwnsOne(s => s.Dimensions, dim =>
            {
                dim.Property(d => d.Width).HasColumnName("Width");
                dim.Property(d => d.Length).HasColumnName("Length");
            });

            entity.HasOne(s => s.Pavilion)
                  .WithMany(p => p.Stands)
                  .HasForeignKey(s => s.PavilionId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(s => s.ExhibitionArea)
                  .WithMany(e => e.Stands)
                  .HasForeignKey(s => s.ExhibitionAreaId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // RefreshToken
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.Token).IsUnique();
            entity.Property(r => r.Token).IsRequired();
            entity.Property(r => r.UserId).IsRequired();
            entity.Property(r => r.Expires).IsRequired();
        });
    }
}