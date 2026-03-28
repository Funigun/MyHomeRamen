using System.Reflection;
using MyHomeRamen.Api.Common;
using MyHomeRamen.Api.Menu.Features.GetCategoriesOptions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Services.AddSharedServices();
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
builder.Services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

// Menu module
builder.Services.AddScoped<ICategoryService, CategoryService>();

WebApplication app = builder.Build();

app.UseExceptionHandler();
app.UseMiddlewares();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();
app.MapEndpoints();

app.Run();
