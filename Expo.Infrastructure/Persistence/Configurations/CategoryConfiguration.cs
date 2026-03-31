using Expo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expo.Infrastructure.Persistence.Configurations
{
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
    {
        entity.HasKey(t => t.Id);

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
                      j => j.HasOne<Tag>()
                            .WithMany()
                            .HasForeignKey("TagId")
                            .OnDelete(DeleteBehavior.Cascade),
                      j => j.HasOne<Category>()
                            .WithMany()
                            .HasForeignKey("CategoryId")
                            .OnDelete(DeleteBehavior.Cascade),
                      j =>
                      {
                          j.ToTable("CategoryTags");
                          j.HasKey("CategoryId", "TagId");
                      });
    }
}
}