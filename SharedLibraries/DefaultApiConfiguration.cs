using Microsoft.Extensions.Configuration;

namespace SharedLibraries;

public static class DefaultApiConfiguration
{
    public static IConfigurationRoot BuildDefaultConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();
    }
}