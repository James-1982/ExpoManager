using Expo.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Expo.Infrastructure.Data;

/// <summary>
/// Application DB context
/// </summary>
/// <param name="options"></param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    /// <summary>
    /// Pavilions
    /// </summary>
    public DbSet<Pavilion> Pavilions { get; set; }
    /// <summary>
    /// ExhibitionHalls
    /// </summary>
    public DbSet<ExhibitionHall> ExhibitionHalls { get; set; }
    /// <summary>
    /// Categories
    /// </summary>
    public DbSet<Category> Categories { get; set; }
    /// <summary>
    /// Stands
    /// </summary>
    public DbSet<Stand> Stands { get; set; }
    /// <summary>
    /// Refresh token
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    /// <summary>
    /// Initialization DB model
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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
        ConfigureTags<ExhibitionHall>();
        ConfigureTags<Category>();
        ConfigureTags<Stand>();

        builder.Entity<Pavilion>(entity =>
        {
            entity.Property(p => p.Nome)
                  .IsRequired();

            entity.Property(p => p.Area)
                  .HasMaxLength(200);

            entity.Property(p => p.PoweredBy)
                  .HasMaxLength(200);

            entity.HasMany(p => p.Stands)
                  .WithOne(s => s.Padiglione)
                  .HasForeignKey(s => s.PadiglioneId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<ExhibitionHall>(entity =>
        {
            entity.Property(s => s.Nome)
                  .IsRequired();

            entity.Property(s => s.Tipo)
                  .HasMaxLength(100);

            entity.Property(s => s.Stato)
                  .HasConversion<string>();

            entity.HasMany(s => s.Stands)
                  .WithOne(st => st.Settore)
                  .HasForeignKey(st => st.SettoreId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Nome)
                  .IsRequired();

            entity.Property(c => c.InEvidenza)
                  .IsRequired();
        });

        builder.Entity<Stand>(entity =>
        {
            entity.Property(s => s.Nome)
                  .IsRequired();

            entity.Property(s => s.Dimensioni)
                  .HasMaxLength(100);

            entity.HasOne(s => s.Padiglione)
                  .WithMany(p => p.Stands)
                  .HasForeignKey(s => s.PadiglioneId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(s => s.Settore)
                  .WithMany(sec => sec.Stands)
                  .HasForeignKey(s => s.SettoreId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Token).IsUnique();

            entity.Property(x => x.Token)
                .IsRequired();
        });
    }
}