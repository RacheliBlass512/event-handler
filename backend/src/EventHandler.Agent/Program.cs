using EventHandler.Agent.Hosting;
using EventHandler.Agent.Ingestion;
using EventHandler.Agent.ServerClient;
using EventHandler.Agent.Sources;

var builder = Host.CreateApplicationBuilder(args);

var serverBaseUrl = builder.Configuration["Server:BaseUrl"] ?? "http://localhost:5027/";

builder.Services.AddHttpClient<IServerClient, HttpServerClient>(client =>
{
    client.BaseAddress = new Uri(serverBaseUrl);
});

builder.Services.AddSingleton<IEventSink, EventIngestionService>();
builder.Services.AddSingleton<IEventSource, MockEventSource>();

// Onboarding a new source: implement IEventSource, register it here, add its config section.
// e.g. builder.Services.AddSingleton<IEventSource, SensorHttpSource>();

builder.Services.AddHostedService<SourceHostService>();

var host = builder.Build();
host.Run();
