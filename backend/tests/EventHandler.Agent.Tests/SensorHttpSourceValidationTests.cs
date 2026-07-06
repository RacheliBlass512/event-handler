using EventHandler.Agent.Sources;
using EventHandler.Contracts;

namespace EventHandler.Agent.Tests;

public class SensorHttpSourceValidationTests
{
    private static IncomingEventDto Valid() => new(
        SourceName: "sensor",
        SourceEventId: "s-1",
        Title: "Title",
        Description: "Description",
        Location: "Location",
        CreatedAt: DateTime.UtcNow,
        Priority: "Normal");

    [Fact]
    public void ValidPayload_Passes()
    {
        Assert.True(SensorHttpSource.TryValidate(Valid(), out var error));
        Assert.Null(error);
    }

    [Fact]
    public void BlankTitle_IsRejected()
    {
        Assert.False(SensorHttpSource.TryValidate(Valid() with { Title = " " }, out var error));
        Assert.Contains("Title", error);
    }

    [Fact]
    public void MissingSourceEventId_IsRejected()
    {
        // Model binding leaves absent JSON fields null — TryValidate must catch that.
        Assert.False(SensorHttpSource.TryValidate(Valid() with { SourceEventId = null! }, out var error));
        Assert.Contains("SourceEventId", error);
    }
}
