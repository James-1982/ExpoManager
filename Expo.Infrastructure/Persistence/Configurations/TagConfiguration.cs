using Expo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expo.Infrastructure.Persistence.Configurations
{
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> entity)
    {
        entity.ToTable("Tags");

        entity.HasKey(t => t.Id);

        entity.Property(t => t.Name)
              .IsRequired()
              .HasMaxLength(50);

        entity.HasIndex(t => t.Name)
              .IsUnique();
    }
}
}