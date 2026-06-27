using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.InfrastructureConfiguration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

IResourceBuilder<RedisResource> cache = builder.ConfigureRedis(config);
IResourceBuilder<RabbitMQServerResource> rabbitmq = builder.ConfigureRabbitMq(config);

IResourceBuilder<KeycloakResource> keyCloak = builder.ConfigureKeyCloak(config);

IResourceBuilder<ProjectResource> dbMigrator = builder.AddDbinitializer(config);
IResourceBuilder<ProjectResource> messagesHandler = builder.AddMessagesHandlerWorker(config)
                                                          .WithReference(rabbitmq)
                                                          .WaitFor(rabbitmq);

IResourceBuilder<ProjectResource> mailingWorker = builder.AddMailingWorker(config)
                                                         .WithReference(rabbitmq)
                                                         .WaitFor(rabbitmq);

IResourceBuilder<ProjectResource> apiService = builder.AddApiService(config)
                                                      .WithReference(rabbitmq)
                                                      .WithReference(cache)
                                                      .WithReference(keyCloak)
                                                      .WaitFor(rabbitmq)
                                                      .WaitFor(cache)
                                                      .WaitFor(dbMigrator)
                                                      .WaitFor(messagesHandler);

IResourceBuilder<ProjectResource> blazor = builder.AddBlazor(config)
                                                  .WithReference(apiService)
                                                  .WithReference(keyCloak)
                                                  .WaitFor(apiService)
                                                  .WaitFor(keyCloak)
                                                  .WithExplicitStart();

apiService.WithReference(blazor);

builder.Eventing.Subscribe<BeforeStartEvent>((_, _) =>
{
    keyCloak.WithEndpoint("https", ep => ep.TargetPort = 12000);
    return Task.CompletedTask;
});

await builder.Build().RunAsync();
