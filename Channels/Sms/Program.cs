using MassTransit;
using Sms.Consumers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddMassTransit(mt => mt.UsingRabbitMq((_, cfg) => {
    cfg.Host(new Uri("rabbitmq://localhost:63685"), h =>
    {
        h.Username("username");
        h.Password("password");
    });
    cfg.ReceiveEndpoint("sms-packet", e =>
    {
        e.Consumer<PacketReceived>();
    });
}));

var app = builder.Build();
app.Run();