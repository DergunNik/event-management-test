using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.DateOfBirth)
            .IsRequired();

        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.Email)
            .IsUnique();
    }
}