using MassTransit;
using Serilog;
using SharedLibraries;

namespace EmailNotifications.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    var connectionString = builder.Configuration["RabbitMQ:ConnectionString"];
                    cfg.Host(connectionString != null ? new Uri(connectionString) : null);
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();
            app.MapDefaultEndpoints();

            app.Run();
        }
    }
}
