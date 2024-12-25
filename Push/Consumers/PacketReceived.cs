using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Bridge.Shared.Models;
using MassTransit;

namespace NS.Push.Consumers;

public class PacketReceived : IConsumer<PushPacket>
{
    public async Task Consume(ConsumeContext<PushPacket> context)
    {
        var jsonMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        });
        Console.WriteLine($"Push packet received message: {jsonMessage}");
    }
}