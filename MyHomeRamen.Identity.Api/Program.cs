WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults("my-home-ramen-identity-api");

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();
