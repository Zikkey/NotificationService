using Bridge.Shared;
using Bridge.Shared.Models;
using Sms.Consumers;

var builder = WebApplication.CreateBuilder(args).UseCustomLogging();
builder.ConfigurePacketConsumer<PacketReceived>(NotificationChannel.Sms);
var app = builder.Build();
app.Run();
