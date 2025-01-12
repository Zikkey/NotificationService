using Bridge.Shared;
using Gateway.Consumer.Consumers;
using Gateway.Infrastructure;
using MassTransit;

var builder = WebApplication.CreateBuilder(args).UseCustomLogging();
var services = builder.Services;

builder.AddNotificationMassTransit(
    mt => mt.AddConsumers(typeof(PacketProcessedConsumer).Assembly),
    (context, cfg) =>
    {
        cfg.ReceiveEndpoint("packet-processed",
            e => { e.ConfigureConsumer<PacketProcessedConsumer>(context); });
    });

services.AddDataAccess();

var app = builder.Build();
app.Run();
