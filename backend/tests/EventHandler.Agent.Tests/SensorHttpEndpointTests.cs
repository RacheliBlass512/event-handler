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

    public SensorHttpEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.AddSingleton<IServerClient, StubServerClient>()))
            .CreateClient();
    }

    private sealed class StubServerClient : IServerClient
    {
        public Task<IntakeResponseDto> SendAsync(IncomingEventDto evt, CancellationToken ct) =>
            Task.FromResult(new IntakeResponseDto(Guid.NewGuid(), Accepted: true));
    }

    private static IncomingEventDto Valid() => new(
        SourceName: "sensor",
        SourceEventId: "s-1",
        Title: "Title",
        Description: "Description",
        Location: "Location",
        OccurredAt: DateTimeOffset.UtcNow,
        Severity: "Normal");

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
