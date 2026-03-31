using Expo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
    {
        entity.Property(c => c.Name)
              .IsRequired()
              .HasMaxLength(100);

        entity.Property(c => c.IsHighlighted)
              .IsRequired();

        entity.Property(c => c.ModifyBy)
              .HasMaxLength(256);

        entity.Property(c => c.LastModify)
              .IsRequired(false);

        entity.HasIndex(c => c.Name).IsUnique();

        entity.HasMany(c => c.Tags)
              .WithMany(t => t.Categories)
              .UsingEntity<Dictionary<string, object>>(
                  "CategoryTags",
                  j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                  j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                  j =>
                  {
                      j.HasKey("CategoryId", "TagId");
                  });
    }
}