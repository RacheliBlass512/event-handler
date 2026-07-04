using EventHandler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHandler.Server.Infrastructure.Persistence.Configurations;

public sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).IsRequired();
        builder.Property(e => e.SourceName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Location).IsRequired().HasMaxLength(200);

        // Status/Priority store as int by EF Core's default enum convention (skeleton-plan.md §5).

        builder.HasMany(e => e.History)
            .WithOne()
            .HasForeignKey(h => h.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
