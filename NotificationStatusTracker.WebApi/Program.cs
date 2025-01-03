using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationStatusTracker.DAL;
using NotificationStatusTracker.DAL.Repositories;
using NotificationStatusTracker.WebApi;
using SharedLibraries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddDbContext<NotificationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INotificationRepository, DbNotificationRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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
        // cfg.ConfigureEndpoints(ctx);
        cfg.ReceiveEndpoint("notification-requested-status-tracker", e =>
        {
            e.Consumer<NotificationRequestedConsumer>(ctx);
        });
    });
});

var app = builder.Build();

// Apply migrations at runtime
using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<NotificationContext>().Database.MigrateAsync();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();