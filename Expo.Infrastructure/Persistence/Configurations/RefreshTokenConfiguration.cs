using Expo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.HasKey(r => r.Id);

        entity.HasIndex(r => r.Token).IsUnique();

        entity.Property(r => r.Token)
              .IsRequired()
              .HasMaxLength(500);

        entity.Property(r => r.UserId)
              .IsRequired();

        entity.Property(r => r.Expires)
              .IsRequired();
    }
}