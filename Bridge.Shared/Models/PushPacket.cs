namespace Bridge.Shared.Models;

public class PushPacket : Packet
{
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
}