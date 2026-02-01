using Microsoft.Extensions.Configuration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;

string resourcePrefix = config["CustomConfig:ResourcePrefix"]!;

IResourceBuilder<ParameterResource>? username = builder.AddParameter("UserName", secret: true);
IResourceBuilder<ParameterResource>? password = builder.AddParameter("Password", secret: true);

IResourceBuilder<RedisResource> cache = builder.AddRedis($"{resourcePrefix}cache", null, password)
                                               .WithRedisInsight()
                                               .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<RabbitMQServerResource> rabbitmq = builder.AddRabbitMQ($"{resourcePrefix}messaging", username, password)
                                                           .WithManagementPlugin()
                                                           .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.MyHomeRamen_Api>($"{resourcePrefix}api")
                                                      .WithHttpHealthCheck("/health")
                                                      .WithReference(cache)
                                                      .WaitFor(cache)
                                                      .WaitFor(rabbitmq)
                                                      .WithReference(rabbitmq);

IResourceBuilder<ProjectResource> identityApiService = builder.AddProject<Projects.MyHomeRamen_Identity_Api>($"{resourcePrefix}identity-api")
                                                              .WithHttpHealthCheck("/health");

builder.AddProject<Projects.MyHomeRamen_Blazor>($"{resourcePrefix}blazor")
       .WithExternalHttpEndpoints()
       .WithHttpHealthCheck("/health")
       .WithReference(cache)
       .WaitFor(cache)
       .WithReference(apiService)
       .WaitFor(apiService)
       .WithReference(apiService)
       .WaitFor(identityApiService)
       .WithReference(identityApiService);

builder.AddProject<Projects.MyHomeRamen_Worker_MailSender>($"{resourcePrefix}mailing-worker")
       .WithReference(apiService)
       .WaitFor(apiService)
       .WithExplicitStart();

builder.AddProject<Projects.MyHomeRamen_Worker_MessagesHandler>($"{resourcePrefix}messages-worker")
       .WithReference(apiService)
       .WaitFor(apiService)
       .WithExplicitStart();

builder.AddContainer("seq", "datalust/seq")
       .WithContainerName("seq-aspire")
       .WithEnvironment("ACCEPT_EULA", "Y")
       .WithBindMount(config["InfrastructureConfig:Seq:BindMountFrom"]!, config["InfrastructureConfig:Seq:BindMountTo"]!)
       .WithHttpEndpoint(8081, 80)
       .WithReference(apiService)
       .WithLifetime(ContainerLifetime.Persistent);

builder.AddContainer("jaeger", "jaegertracing/all-in-one")
       .WithContainerName("jaeger-aspire")
       .WithHttpEndpoint(16686, targetPort: 16686, name: "jaegerPortal")
       .WithHttpEndpoint(4317, targetPort: 4317, name: "jaegerEndpoint")
       .WithLifetime(ContainerLifetime.Persistent);

await builder.Build().RunAsync();
