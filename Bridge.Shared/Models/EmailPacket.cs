namespace Bridge.Shared.Models;

public class EmailPacket : Packet
{
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
}