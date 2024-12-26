using System.Net;
using System.Net.Http.Json;
using ApiGateway.WebApi.DTOs;

namespace ApiGateway.IntegrationTests;

[TestFixture]
public class Tests
{
    private readonly ApiGatewayFactory _apiGatewayFactory = new();

    [SetUp]
    public async Task Setup()
    {
        await _apiGatewayFactory.StartContainersAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _apiGatewayFactory.DisposeAsync();
    }

    [Test]
    public async Task CreateNotification_EmailNotification_NotificationRetrievedToEmailService()
    {
        var client = _apiGatewayFactory.CreateClient();

        var response = await client.PostAsJsonAsync("notifications", new CreateNotificationRequest
        {
            Channels = ["email"],
            Text = "some text",
            Recipients = ["example@email.com"]
        });

        var emailServiceLogs = await _apiGatewayFactory.EmailNotificationContainer.GetLogsAsync();
        
        Console.WriteLine(emailServiceLogs.Stdout);

        Assert.Multiple(() =>
        {
            Assert.That(emailServiceLogs.Stdout, Is.Not.Empty);
            Assert.That(emailServiceLogs.Stderr, Is.Empty);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }
}