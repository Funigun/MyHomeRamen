IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource> cache = builder.AddRedis("cache");

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.MyHomeRamen_Api>("my-home-ramen-api")
                                                      .WithHttpHealthCheck("/health");

IResourceBuilder<ProjectResource> mailingWorker = builder.AddProject<Projects.MyHomeRamen_Worker_MailSender>("my-home-ramen-mailing-worker")
                                                         .WithReference(apiService)
                                                         .WaitFor(apiService);

IResourceBuilder<ProjectResource> messagesWorker = builder.AddProject<Projects.MyHomeRamen_Worker_MessagesHandler>("my-home-ramen-messages-worker")
                                                          .WithReference(apiService)
                                                          .WaitFor(apiService);

builder.AddProject<Projects.MyHomeRamen_Blazor>("my-home-ramen-api-blazor")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

await builder.Build().RunAsync();
