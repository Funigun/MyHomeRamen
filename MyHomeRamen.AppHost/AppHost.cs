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

IResourceBuilder<ProjectResource> identityApiService = builder.AddIdentityApiService(config)
                                                              .WithReference(rabbitmq)
                                                              .WithReference(cache)
                                                              .WithReference(keyCloak)
                                                              .WaitFor(rabbitmq)
                                                              .WaitFor(cache)
                                                              .WaitFor(keyCloak)
                                                              .WaitFor(dbMigrator)
                                                              .WaitFor(messagesHandler);

IResourceBuilder<ProjectResource> apiService = builder.AddApiService(config)
                                                      .WithReference(rabbitmq)
                                                      .WithReference(cache)
                                                      .WithReference(keyCloak)
                                                      .WaitFor(rabbitmq)
                                                      .WaitFor(cache)
                                                      .WaitFor(dbMigrator)
                                                      .WaitFor(messagesHandler);

IResourceBuilder<ProjectResource> blazor = builder.AddBlazor(config)
                                                  .WithReference(identityApiService)
                                                  .WithReference(apiService)
                                                  .WithReference(keyCloak)
                                                  .WaitFor(identityApiService)
                                                  .WaitFor(apiService)
                                                  .WaitFor(keyCloak)
                                                  .WithExplicitStart();

identityApiService.WithReference(blazor);
apiService.WithReference(blazor);

await builder.Build().RunAsync();
