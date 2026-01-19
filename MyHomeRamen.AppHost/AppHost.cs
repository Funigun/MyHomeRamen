using Aspire.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

string resourcePrefix = builder.Configuration["CustomConfig:ResourcePrefix"]!;

IResourceBuilder<ParameterResource>? username = builder.AddParameter("UserName", secret: true);
IResourceBuilder<ParameterResource>? password = builder.AddParameter("Password", secret: true);

IResourceBuilder<RedisResource> cache = builder.AddRedis($"{resourcePrefix}cache", null, password)
                                               .WithRedisInsight();

IResourceBuilder<RabbitMQServerResource> rabbitmq = builder.AddRabbitMQ($"{resourcePrefix}messaging", username, password)
                                                           .WithManagementPlugin();

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.MyHomeRamen_Api>($"{resourcePrefix}api")
                                                      .WithHttpHealthCheck("/health")
                                                      .WithReference(cache)
                                                      .WaitFor(cache)
                                                      .WaitFor(rabbitmq)
                                                      .WithReference(rabbitmq);

IResourceBuilder<ProjectResource> identityApiService = builder.AddProject<Projects.MyHomeRamen_Identity_Api>($"{resourcePrefix}identity-api")
                                                      .WithHttpHealthCheck("/health");

builder.AddProject<Projects.MyHomeRamen_Blazor>($"{resourcePrefix}api-blazor")
       .WithExternalHttpEndpoints()
       .WithHttpHealthCheck("/health")
       .WithReference(cache)
       .WaitFor(cache)
       .WithReference(apiService)
       .WaitFor(apiService)
       .WithReference(apiService)
       .WaitFor(identityApiService)
       .WithReference(identityApiService);

IResourceBuilder<ProjectResource> mailingWorker = builder.AddProject<Projects.MyHomeRamen_Worker_MailSender>($"{resourcePrefix}mailing-worker")
                                                         .WithReference(apiService)
                                                         .WaitFor(apiService);

IResourceBuilder<ProjectResource> messagesWorker = builder.AddProject<Projects.MyHomeRamen_Worker_MessagesHandler>($"{resourcePrefix}messages-worker")
                                                          .WithReference(apiService)
                                                          .WaitFor(apiService);

await builder.Build().RunAsync();
