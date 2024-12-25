using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Bridge.Shared.Models;
using MassTransit;

namespace Sms.Consumers;

public class PacketReceived : IConsumer<SmsPacket>
{
    public async Task Consume(ConsumeContext<SmsPacket> context)
    {
        var jsonMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        });
        Console.WriteLine($"Sms packet received message: {jsonMessage}");
    }
}