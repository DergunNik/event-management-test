using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.HasKey(ep => ep.Id);

        builder.Property(ep => ep.RegistrationDate)
            .IsRequired();

        builder.HasOne(ep => ep.Event)
            .WithMany(e => e.Participants)
            .HasForeignKey(ep => ep.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ep => ep.User)
            .WithMany(u => u.Events)
            .HasForeignKey(ep => ep.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}