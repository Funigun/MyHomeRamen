using MyHomeRamen.Worker.MessagesHandler;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults("my-home-ramen-messages-worker");
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
