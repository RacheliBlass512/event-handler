using EventHandler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHandler.Server.Infrastructure.Persistence.Configurations;

public sealed class EventHistoryEntryConfiguration : IEntityTypeConfiguration<EventHistoryEntry>
{
    public void Configure(EntityTypeBuilder<EventHistoryEntry> builder)
    {
        builder.ToTable("EventHistory");
        builder.HasKey(h => h.Id);

        // Index for the UI timeline query (skeleton-plan.md §5).
        builder.HasIndex(h => new { h.EventId, h.ChangedAt });
    }
}
