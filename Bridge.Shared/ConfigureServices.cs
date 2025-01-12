using Bridge.Shared.Models;
using Bridge.Shared.Models.Requests;
using Bridge.Shared.Options;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Bridge.Shared;

public static class ConfigureServices
{
    public static WebApplicationBuilder UseCustomLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, conf) =>
        {
            conf.MinimumLevel.Information().MinimumLevel
                .Override("Microsoft", LogEventLevel.Information).MinimumLevel
                .Override("Hangfire", LogEventLevel.Warning).MinimumLevel
                .Override("EdjCase", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console();
        });

        return builder;
    }

    public static WebApplicationBuilder ConfigureNotificationPublisher(this WebApplicationBuilder builder)
    {
        return builder.AddNotificationMassTransit(
            configureRabbitMq: (_, cfg) =>
            {
                var packetTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => typeof(Packet).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToList();

                foreach (var packetType in packetTypes)
                {
                    if (Activator.CreateInstance(packetType) is not Packet instance)
                    {
                        throw new InvalidOperationException($"Failed to create instance of {packetType.Name}");
                    }

                    var channel = instance.Channel.ToString().ToLowerInvariant();

                    var method = typeof(IRabbitMqBusFactoryConfigurator)
                        .GetMethod(nameof(IRabbitMqBusFactoryConfigurator.Message))
                        ?.MakeGenericMethod(packetType);

                    method?.Invoke(cfg,
                    [
                        new Action<object>(m =>
                        {
                            dynamic configurator = m;
                            configurator.SetEntityName($"{channel}-packet");
                            Console.WriteLine(
                                $"Registered packet type: {packetType.Name}, Channel: {channel}, EntityName: {channel}-packet");
                        })
                    ]);
                }
            });
    }

    public static WebApplicationBuilder ConfigurePacketConsumer<TConsumer>(this WebApplicationBuilder builder,
        NotificationChannel channel) where TConsumer : class, IConsumer, new()
    {
        return builder.AddNotificationMassTransit(
            configureRabbitMq: (_, cfg) =>
            {
                cfg.ReceiveEndpoint($"{channel.ToString().ToLowerInvariant()}-packet",
                    e => { e.Consumer<TConsumer>(); });
            });
    }

    public static WebApplicationBuilder AddNotificationMassTransit(
        this WebApplicationBuilder builder,
        Action<IBusRegistrationConfigurator>? configureBus = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configureRabbitMq = null)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        var options = new RabbitOptions();
        configuration.GetSection("RabbitOptions").Bind(options);
        services.AddMassTransit(mt =>
        {
            configureBus?.Invoke(mt);
            mt.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(options!.Host), h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });
                configureRabbitMq?.Invoke(context, cfg);
            });
        });

        return builder;
    }
}
