using Expo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ExhibitionAreaConfiguration : IEntityTypeConfiguration<ExhibitionArea>
{
    public void Configure(EntityTypeBuilder<ExhibitionArea> entity)
    {
        entity.Property(e => e.Name)
              .IsRequired()
              .HasMaxLength(100);

        entity.Property(e => e.Type)
              .HasConversion<string>()
              .HasMaxLength(50);

        entity.Property(e => e.State)
              .HasConversion<string>()
              .HasMaxLength(50);

        entity.Property(e => e.ModifyBy)
              .HasMaxLength(256);

        entity.Property(e => e.LastModify)
              .IsRequired(false);

        entity.HasIndex(e => e.Name).IsUnique();

        entity.HasMany(e => e.Tags)
              .WithMany(t => t.ExhibitionAreas)
              .UsingEntity<Dictionary<string, object>>(
                  "ExhibitionAreaTags",
                  j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                  j => j.HasOne<ExhibitionArea>().WithMany().HasForeignKey("ExhibitionAreaId"),
                  j =>
                  {
                      j.HasKey("ExhibitionAreaId", "TagId");
                  });
    }
}