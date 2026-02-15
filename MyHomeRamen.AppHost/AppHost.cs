using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.InfrastructureConfiguration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

IResourceBuilder<RedisResource> cache = builder.ConfigureRedis(config);
IResourceBuilder<RabbitMQServerResource> rabbitmq = builder.ConfigureRabbitMq(config);

IResourceBuilder<KeycloakResource> keyCloak = builder.ConfigureKeyCloak(config);

IResourceBuilder<ProjectResource> dbMigrator = builder.AddProject<Projects.MyHomeRamen_Worker_DatabaseInitializer>($"my-home-ramen-db-initializer");

IResourceBuilder<ProjectResource> identityApiService = builder.AddIdentityApiService(config)
                                                              .WithReference(rabbitmq)
                                                              .WithReference(cache)
                                                              .WithReference(keyCloak)
                                                              .WaitFor(rabbitmq)
                                                              .WaitFor(cache)
                                                              .WaitFor(keyCloak)
                                                              .WaitFor(dbMigrator);

IResourceBuilder<ProjectResource> apiService = builder.AddApiService(config)
                                                      .WithReference(identityApiService)
                                                      .WaitFor(identityApiService);

IResourceBuilder<ProjectResource> blazor = builder.AddBlazor(config)
                                                  .WithReference(identityApiService)
                                                  .WithReference(apiService)
                                                  .WaitFor(apiService);

identityApiService.WithReference(blazor);

builder.AddWorkers(config, apiService);

await builder.Build().RunAsync();
