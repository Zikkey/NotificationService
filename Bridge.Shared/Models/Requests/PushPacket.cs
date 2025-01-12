namespace Bridge.Shared.Models.Requests;

public class PushPacket : Packet
{
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;

    public override NotificationChannel Channel => NotificationChannel.Push;
}
