namespace MyHomeRamen.Api.WebPresentation;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

        return builder;
    }

    public static bool IsTesting(this WebApplicationBuilder builder)
    {
        return builder.Environment.IsEnvironment("Test");
    }
}
