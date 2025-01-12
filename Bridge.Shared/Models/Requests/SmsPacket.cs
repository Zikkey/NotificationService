namespace Bridge.Shared.Models.Requests;

public class SmsPacket : Packet
{
    public string Body { get; set; }

    public override NotificationChannel Channel => NotificationChannel.Sms;
}
