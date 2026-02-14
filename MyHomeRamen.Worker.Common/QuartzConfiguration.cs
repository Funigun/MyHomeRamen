using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MyHomeRamen.Worker.Common;

public static class QuartzConfiguration
{
    public static IServiceCollection AddQuartzServices(this IServiceCollection services)
    {
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true;
            options.Scheduling.OverWriteExistingData = true;
        });

        services.AddQuartz(q =>
        {
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });
        });


        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}
