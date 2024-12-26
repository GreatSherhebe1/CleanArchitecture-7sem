using ApiGateway.WebApi;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Testcontainers.RabbitMq;

namespace ApiGateway.IntegrationTests;

public sealed class ApiGatewayFactory : WebApplicationFactory<IApiMarker>
{
    public RabbitMqContainer RabbitMqContainer { get; set; }
    public IContainer EmailNotificationContainer { get; set; }

    public async Task StartContainersAsync()
    {
        var network = new NetworkBuilder().Build();
        await network.CreateAsync();
        
        RabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:management-alpine")
            .WithNetwork(network)
            .Build();

        await RabbitMqContainer.StartAsync();
        
        const string testcontainer = "testcontainers/email-service:dev";

        var emailNotificationImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "")
            .WithDockerfile("EmailNotifications.WebApi/Dockerfile")
            .WithName(testcontainer)
            .WithImageBuildPolicy(PullPolicy.Missing)
            .WithCleanUp(true)
            .WithDeleteIfExists(true)
            .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
            .Build();

        await emailNotificationImage.CreateAsync();
        
        EmailNotificationContainer = new ContainerBuilder()
            .WithImage(emailNotificationImage.FullName)
            .WithImagePullPolicy(PullPolicy.Never)
            .WithNetwork(network)
            // .WithEnvironment("ASPNETCORE_RabbitMQ__ConnectionString", RabbitMqContainer.GetConnectionString())
            .DependsOn(RabbitMqContainer)
            .Build();
        
        await EmailNotificationContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var massTransitHostedService = services
                .FirstOrDefault(d => d.ServiceType == typeof(IHostedService) &&
                                     d.ImplementationFactory != null &&
                                     d.ImplementationFactory.Method.ReturnType ==
                                     typeof(MassTransitHostedService));

            services.Remove(massTransitHostedService);

            var descriptors = services.Where(d =>
                    d.ServiceType.Namespace != null &&
                    d.ServiceType.Namespace.Contains("MassTransit", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);

            services.AddMassTransit(x =>
            {
                var connectionString = new Uri(RabbitMqContainer.GetConnectionString());

                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((ctx, cfg) =>
                    cfg.Host(connectionString));
            });
        });
    }

    private async ValueTask DisposeAsyncCore()
    {
        await EmailNotificationContainer.StopAsync();
        await RabbitMqContainer.StopAsync();

        await RabbitMqContainer.DisposeAsync();
        await EmailNotificationContainer.DisposeAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        await base.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}