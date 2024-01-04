using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Settings.Configuration;
using ILogger = Serilog.ILogger;

namespace ScanIta.Crawler.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static ILogger CreateBootstrapLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .WriteTo.Console()
            .Enrich.FromLogContext()
            .CreateBootstrapLogger();
    }
    
    public static WebApplicationBuilder AddSerilog(
        this WebApplicationBuilder builder,
        IConfiguration configuration,
        string applicationname = "ScanIta.Extractor.API")
    {
        builder.Host.UseSerilog(
            (context, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(configuration);

                loggerConfiguration.Enrich
                    .WithProperty("Application", applicationname)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.ApplicationInsights(
                        new TelemetryConfiguration
                        {
                            ConnectionString = configuration.GetConnectionString("ApplicationInsights")
                        }, TelemetryConverter.Traces
                    )
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails();
            });

        return builder;
    }
}