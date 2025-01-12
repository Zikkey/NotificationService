using Bridge.Shared;
using Bridge.Shared.Models;
using NS.Push.Consumers;

var builder = WebApplication.CreateBuilder(args).UseCustomLogging();
builder.ConfigurePacketConsumer<PacketReceived>(NotificationChannel.Push);
var app = builder.Build();
app.Run();
