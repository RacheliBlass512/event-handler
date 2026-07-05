using EventHandler.Agent.Hosting;
using EventHandler.Agent.Ingestion;
using EventHandler.Agent.ServerClient;
using EventHandler.Agent.Sources;

var builder = WebApplication.CreateBuilder(args);

var serverBaseUrl = builder.Configuration["Server:BaseUrl"] ?? "http://localhost:5027/";

builder.Services.AddHttpClient<IServerClient, HttpServerClient>(client =>
{
    client.BaseAddress = new Uri(serverBaseUrl);
});

builder.Services.AddSingleton<IEventSink, EventIngestionService>();

// Onboarding a new source: poll-style → implement IEventSource, push-style → implement
// IHttpEventSource. Either way: register it here, add its config section.
builder.Services.AddSingleton<IHttpEventSource, SensorHttpSource>();

builder.Services.AddHostedService<SourceHostService>();

var app = builder.Build();

var sink = app.Services.GetRequiredService<IEventSink>();
foreach (var source in app.Services.GetServices<IHttpEventSource>())
{
    source.MapRoutes(app, sink);
}

app.Run();

// Marker for WebApplicationFactory<Program> in EventHandler.Agent.Tests.
public partial class Program { }
