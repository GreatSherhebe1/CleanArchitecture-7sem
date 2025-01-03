using EmailNotifications.WebApi;
using MassTransit;
using SharedLibraries;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    var assembly = typeof(Program).Assembly;

    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumers(assembly);

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var connectionString = builder.Configuration["RabbitMQ:ConnectionString"];
        cfg.Host(connectionString != null ? new Uri(connectionString) : null);
        cfg.ReceiveEndpoint("notification-requested-email-service", e =>
        {
            e.Consumer<NotificationRequestedConsumer>(ctx);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();