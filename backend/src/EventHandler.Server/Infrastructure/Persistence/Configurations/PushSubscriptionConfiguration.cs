using EventHandler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventHandler.Server.Infrastructure.Persistence.Configurations;

public sealed class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.ToTable("PushSubscriptions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Endpoint).IsRequired();
        builder.Property(p => p.P256dh).IsRequired();
        builder.Property(p => p.Auth).IsRequired();
    }
}
