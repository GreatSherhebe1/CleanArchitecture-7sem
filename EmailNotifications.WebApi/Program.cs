using MassTransit;
using Serilog;
using SharedLibraries;

var configuration = DefaultApiConfiguration.BuildDefaultConfiguration();

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Configuration.AddConfiguration(configuration);

Log.Logger = DefaultApiLogger.CreateLogger(configuration, builder.Environment);
builder.Services.AddSerilog();

builder.Services.AddControllers();

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
        cfg.ConfigureEndpoints(ctx);
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