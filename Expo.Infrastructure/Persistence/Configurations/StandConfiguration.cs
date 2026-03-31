using Expo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StandConfiguration : IEntityTypeConfiguration<Stand>
{
    public void Configure(EntityTypeBuilder<Stand> entity)
    {
        entity.Property(s => s.Name)
              .IsRequired()
              .HasMaxLength(100);

        entity.Property(s => s.ModifyBy)
              .HasMaxLength(256);

        entity.Property(s => s.LastModify)
              .IsRequired(false);

        // VALUE OBJECT
        entity.OwnsOne(s => s.Dimensions, dim =>
        {
            dim.Property(d => d.Width)
               .HasColumnName("Width")
               .HasPrecision(18, 2);

            dim.Property(d => d.Length)
               .HasColumnName("Length")
               .HasPrecision(18, 2);
        });

        // RELAZIONI
        entity.HasOne(s => s.Pavilion)
              .WithMany(p => p.Stands)
              .HasForeignKey(s => s.PavilionId)
              .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(s => s.ExhibitionArea)
              .WithMany(e => e.Stands)
              .HasForeignKey(s => s.ExhibitionAreaId)
              .OnDelete(DeleteBehavior.SetNull);

        // TAGS
        entity.HasMany(s => s.Tags)
              .WithMany(t => t.Stands)
              .UsingEntity<Dictionary<string, object>>(
                  "StandTags",
                  j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                  j => j.HasOne<Stand>().WithMany().HasForeignKey("StandId"),
                  j =>
                  {
                      j.HasKey("StandId", "TagId");
                  });
    }
}