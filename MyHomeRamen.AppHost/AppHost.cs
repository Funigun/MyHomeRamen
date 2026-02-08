using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.InfrastructureConfiguration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;

IResourceBuilder<RedisResource> cache = builder.ConfigureRedis(config);
IResourceBuilder<RabbitMQServerResource> rabbitmq = builder.ConfigureRabbitMq(config);

IResourceBuilder<KeycloakResource> keyCloak = builder.ConfigureKeyCloak(config);

IResourceBuilder<PostgresServerResource> postgres = builder.ConfigurePostgresDb(config);
postgres.AddDatabase("db");

IResourceBuilder<ProjectResource> identityApiService = builder.AddIdentityApiService(config)
                                                      .WithReference(rabbitmq)
                                                      .WithReference(cache)
                                                      .WithReference(keyCloak)
                                                      .WithReference(postgres)
                                                      .WaitFor(rabbitmq)
                                                      .WaitFor(cache)
                                                      .WaitFor(keyCloak);

IResourceBuilder<ProjectResource> apiService = builder.AddApiService(config)
                                                      .WithReference(rabbitmq)
                                                      .WithReference(cache)
                                                      .WithReference(keyCloak)
                                                      .WithReference(postgres)
                                                      .WithReference(identityApiService)
                                                      .WaitFor(rabbitmq)
                                                      .WaitFor(cache)
                                                      .WaitFor(keyCloak)
                                                      .WaitFor(identityApiService);

builder.AddBlazor(config)
       .WithReference(keyCloak)
       .WithReference(apiService)
       .WithReference(identityApiService)
       .WaitFor(apiService);

builder.AddWorkers(config, apiService);

builder.ConfigureSeq(config);
builder.ConfigureJaeger(config);

await builder.Build().RunAsync();
