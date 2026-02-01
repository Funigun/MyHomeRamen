using MyHomeRamen.Worker.MailSender;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults("my-home-ramen-mailing-worker");
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
