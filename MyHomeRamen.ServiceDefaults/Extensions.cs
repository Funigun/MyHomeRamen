using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static TBuilder AddApiServiceDefaults<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
    {
        builder.WithHttpMetrics()
               .WithConfiguredLogging();

        builder.Services.AddOpenTelemetry()
                        .WithTracing(tracing =>
                           {
                               tracing.AddSource(builder.Environment.ApplicationName)
                                   .AddAspNetCoreInstrumentation(tracing =>
                                       tracing.Filter = context =>
                                           !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                                           && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                                   )
                                   .AddHttpClientInstrumentation()
                                   .AddEntityFrameworkCoreInstrumentation()
                                   .AddSqlClientInstrumentation()
                                   .AddRedisInstrumentation()
                                   .AddSource("MyHomeRamen.Activity.Cache");
                           });

        builder.AddOpenTelemetryExporters();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static TBuilder AddBlazorServiceDefaults<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
    {
        builder.WithHttpMetrics()
               .WithConfiguredLogging();

        builder.Services.AddOpenTelemetry()
                        .WithTracing(tracing =>
                        {
                            tracing.AddSource(builder.Environment.ApplicationName)
                                .AddAspNetCoreInstrumentation(tracing =>
                                    tracing.Filter = context =>
                                        !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                                        && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                                )
                                .AddHttpClientInstrumentation()
                                .AddRedisInstrumentation();
                        });
        ;
        builder.AddOpenTelemetryExporters();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static TBuilder AddWorkerServiceDefaults<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
    {
        builder.WithHttpMetrics()
               .WithConfiguredLogging();

        builder.Services.AddOpenTelemetry()
                        .WithTracing(tracing =>
                        {
                            tracing.AddSource(builder.Environment.ApplicationName)
                                .AddAspNetCoreInstrumentation(tracing =>
                                    tracing.Filter = context =>
                                        !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                                        && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                                )
                                .AddHttpClientInstrumentation()
                                .AddEntityFrameworkCoreInstrumentation()
                                .AddSqlClientInstrumentation()
                                .AddQuartzInstrumentation();
                        });

        builder.AddOpenTelemetryExporters();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }

    private static TBuilder WithConfiguredLogging<TBuilder>(this TBuilder builder)
         where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        return builder;
    }

    private static TBuilder WithHttpMetrics<TBuilder>(this TBuilder builder)
             where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddOpenTelemetry()
                        .WithMetrics(metrics =>
                        {
                            metrics.AddAspNetCoreInstrumentation()
                                   .AddHttpClientInstrumentation()
                                   .AddRuntimeInstrumentation();
                        });
        return builder;
    }

    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder)
             where TBuilder : IHostApplicationBuilder
    {
        bool useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());
        }

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()

            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapHealthChecks(HealthEndpointPath);

            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}
