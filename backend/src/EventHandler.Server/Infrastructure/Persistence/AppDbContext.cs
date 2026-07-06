using EventHandler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventHandler.Server.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventHistoryEntry> EventHistory => Set<EventHistoryEntry>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PushSubscription> PushSubscriptions => Set<PushSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    // SQL Server's `datetime2` is timezone-naive, so EF reads DateTimes back with Kind=Unspecified
    // and System.Text.Json serializes them without the trailing 'Z'. We store UTC everywhere, so
    // stamp Kind=Utc on read — one place, every DateTime property, so the wire is unambiguously UTC
    // and the frontend can convert to local time.
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        => configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();

    private sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
        write => write,
        read => DateTime.SpecifyKind(read, DateTimeKind.Utc));
}
