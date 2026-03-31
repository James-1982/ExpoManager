using Expo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PavilionConfiguration : IEntityTypeConfiguration<Pavilion>
{
    public void Configure(EntityTypeBuilder<Pavilion> entity)
    {
        entity.Property(p => p.Name)
              .IsRequired()
              .HasMaxLength(100);

        entity.Property(p => p.Area)
              .HasMaxLength(200);

        entity.Property(p => p.PoweredBy)
              .HasMaxLength(200);

        entity.Property(p => p.ModifyBy)
              .HasMaxLength(256);

        entity.Property(p => p.LastModify)
              .IsRequired(false);

        entity.HasIndex(p => p.Name).IsUnique();

        // MANY-TO-MANY TAGS
        entity.HasMany(p => p.Tags)
              .WithMany(t => t.Pavilions)
              .UsingEntity<Dictionary<string, object>>(
                  "PavilionTags",
                  j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                  j => j.HasOne<Pavilion>().WithMany().HasForeignKey("PavilionId"),
                  j =>
                  {
                      j.HasKey("PavilionId", "TagId");
                  });
    }
}