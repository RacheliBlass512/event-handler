using System.Net;
using System.Net.Http.Json;
using System.Text;
using EventHandler.Agent.ServerClient;
using EventHandler.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EventHandler.Agent.Tests;

public class SensorHttpEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly StubServerClient _stub = new();

    public SensorHttpEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.AddSingleton<IServerClient>(_stub)))
            .CreateClient();
    }

    private sealed class StubServerClient : IServerClient
    {
        public IncomingEventDto? Last { get; private set; }

        public Task<IntakeResponseDto> SendAsync(IncomingEventDto evt, CancellationToken ct)
        {
            Last = evt;
            return Task.FromResult(new IntakeResponseDto(Guid.NewGuid(), Accepted: true));
        }
    }

    private static IncomingEventDto Valid() => new(
        SourceName: "sensor",
        SourceEventId: "s-1",
        Title: "Title",
        Description: "Description",
        Location: "Location",
        CreatedAt: DateTime.UtcNow,
        Priority: "Normal");

    [Fact]
    public async Task ValidPayload_Returns202WithIntakeResponse()
    {
        var response = await _client.PostAsJsonAsync("/events/sensor", Valid());

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<IntakeResponseDto>();
        Assert.NotNull(body);
        Assert.True(body.Accepted);
    }

    [Fact]
    public async Task Agent_StampsCreatedAt_IgnoringSourceSuppliedValue()
    {
        // Source sends a stale timestamp; the Agent must overwrite it with ingestion time.
        var stale = Valid() with { CreatedAt = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var before = DateTime.UtcNow;

        var response = await _client.PostAsJsonAsync("/events/sensor", stale);

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.NotNull(_stub.Last);
        Assert.InRange(_stub.Last!.CreatedAt, before.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task MissingTitle_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/events/sensor", Valid() with { Title = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task MalformedJson_Returns400()
    {
        var content = new StringContent("{ this is not json", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/events/sensor", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
