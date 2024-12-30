using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Elasticsearch;

namespace SharedLibraries;

public static class DefaultApiLogger
{
    public static Logger CreateLogger(IConfigurationRoot configuration, IWebHostEnvironment environment)
    {
        return new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
            .Enrich.WithProperty("Environment", environment.EnvironmentName)
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    public static ElasticsearchSinkOptions ConfigureElasticSink(
        IConfigurationRoot configuration,
        IWebHostEnvironment environment)
    {
        var formatedAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name?.ToLower().Replace(".", "-");
        var formatedEnvironment = environment.EnvironmentName.ToLower().Replace(".", "-");

        return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"] ?? string.Empty))
        {
            AutoRegisterTemplate = true,
            IndexFormat =
                $"{formatedAssemblyName}-{formatedEnvironment}-{DateTime.UtcNow:yyyy-MM}"
        };
    }
}