using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MyHomeRamen.Worker.Common;

public static class DependencyInjection
{
    public static HostApplicationBuilder AddConfiguration(this HostApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

        return builder;
    }
}
