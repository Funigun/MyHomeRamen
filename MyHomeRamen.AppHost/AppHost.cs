using Microsoft.Extensions.Configuration;
using MyHomeRamen.AppHost.InfrastructureConfiguration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IConfiguration config = builder.Configuration;
string resourcePrefix = config["CustomConfig:ResourcePrefix"]!;

IResourceBuilder<ParameterResource>? username = builder.AddParameter("UserName", secret: true);
IResourceBuilder<ParameterResource>? password = builder.AddParameter("Password", secret: true);

IResourceBuilder<RedisResource> cache = builder.AddRedis(resourcePrefix, password);
IResourceBuilder<RabbitMQServerResource> rabbitmq = builder.AddRabbitMq(resourcePrefix, username, password);

IResourceBuilder<ProjectResource> apiService = builder.AddApiService(resourcePrefix, cache, rabbitmq);
IResourceBuilder<ProjectResource> identityApiService = builder.AddIdentityApiService(resourcePrefix);

builder.AddBlazor(resourcePrefix, cache, apiService, identityApiService);
builder.AddWorkers(resourcePrefix, apiService);

builder.AddSeq(config, apiService);
builder.AddJaeger();

await builder.Build().RunAsync();
