using MyHomeRamen.Worker.MailSender;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
