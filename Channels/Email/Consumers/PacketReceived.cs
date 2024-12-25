using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Bridge.Shared.Models;
using MassTransit;

namespace Email.Consumers;

public class PacketReceived : IConsumer<EmailPacket>
{
    public async Task Consume(ConsumeContext<EmailPacket> context)
    {
        var jsonMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        });
        Console.WriteLine($"Email packet received message: {jsonMessage}");
    }
}