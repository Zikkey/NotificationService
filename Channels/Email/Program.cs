using Bridge.Shared;
using Bridge.Shared.Models;
using Email.Consumers;

var builder = WebApplication.CreateBuilder(args).UseCustomLogging();
builder.ConfigurePacketConsumer<PacketReceived>(NotificationChannel.Email);
var app = builder.Build();
app.Run();
