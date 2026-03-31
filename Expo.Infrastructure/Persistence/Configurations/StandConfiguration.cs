using Expo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expo.Infrastructure.Persistence.Configurations;

public class StandConfiguration : IEntityTypeConfiguration<Stand>
{
    public void Configure(EntityTypeBuilder<Stand> entity)
    {
        entity.HasKey(s => s.Id);

        entity.Property(s => s.Name)
              .IsRequired()
              .HasMaxLength(100);

        entity.Property(s => s.ModifyBy)
              .HasMaxLength(256);

        entity.Property(s => s.LastModify)
              .IsRequired(false);

        entity.OwnsOne(s => s.Dimensions, dim =>
        {
            dim.Property(d => d.Width)
               .HasColumnName("Width")
               .HasPrecision(18, 2);

            dim.Property(d => d.Length)
               .HasColumnName("Length")
               .HasPrecision(18, 2);
        });

        entity.HasIndex(e => e.Name).IsUnique();

        // RELAZIONI
        entity.HasOne(s => s.Pavilion)
              .WithMany(p => p.Stands)
              .HasForeignKey(s => s.PavilionId)
              .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(s => s.ExhibitionArea)
              .WithMany(e => e.Stands)
              .HasForeignKey(s => s.ExhibitionAreaId)
              .OnDelete(DeleteBehavior.SetNull);

        // RELAZIONE molti-a-molti con Tags
        entity.HasMany(s => s.Tags)
              .WithMany(t => t.Stands)
              .UsingEntity<Dictionary<string, object>>(
                  "StandTags",
                  j => j.HasOne<Tag>()
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade),
                  j => j.HasOne<Stand>()
                        .WithMany()
                        .HasForeignKey("StandId")
                        .OnDelete(DeleteBehavior.Cascade),
                  j =>
                  {
                      j.ToTable("StandTags");
                      j.HasKey("StandId", "TagId");
                  });

        // RELAZIONE molti-a-molti con Categories
        entity.HasMany(s => s.Categories)
              .WithMany(c => c.Stands)
              .UsingEntity<Dictionary<string, object>>(
                  "StandCategories",
                  j => j.HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade),
                  j => j.HasOne<Stand>()
                        .WithMany()
                        .HasForeignKey("StandId")
                        .OnDelete(DeleteBehavior.Cascade),
                  j =>
                  {
                      j.ToTable("StandCategories");
                      j.HasKey("StandId", "CategoryId");
                  });
    }
}