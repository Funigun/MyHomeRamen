WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults("my-home-ramen-api");

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

await app.RunAsync();
