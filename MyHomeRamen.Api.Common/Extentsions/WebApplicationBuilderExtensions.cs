using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MyHomeRamen.Api.Common.Extentsions;

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
