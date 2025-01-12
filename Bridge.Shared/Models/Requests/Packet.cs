namespace Bridge.Shared.Models.Requests;

public abstract class Packet
{
    public string Contact { get; set; } = null!;
    public abstract NotificationChannel Channel { get; }
}
