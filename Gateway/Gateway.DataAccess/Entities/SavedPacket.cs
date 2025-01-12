using System.Text.Json;
using Bridge.Shared.Models;
using Gateway.Domain.Entities.Base;
using Gateway.Domain.Extensions;

namespace Gateway.Domain.Entities;

public sealed class SavedPacket : IIdentifiedEntity, ICreateAuditedEntity
{
    public Guid? Id { get; set; }
    public JsonDocument Payload { get; private set; } = null!;
    public DateTimeOffset? CreatedOn { get; set; }
    public SavedPacketState State { get; private set; } = SavedPacketState.New;
    public string StateString => State.GetDbFriendlyValue();
    public string? Error { get; private set; }
    public int Priority { get; private set; } = 2; // 2 - default priority, 0 - most priority
    public string Type { get; private set; } = null!;
    public Guid? EventId { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string ChannelString => Channel.GetDbFriendlyValue();

    private SavedPacket() { }

    public static SavedPacket CreatePacket<TPacketType>(JsonDocument payload, NotificationChannel channel) where TPacketType : class
    {
        var packet = new SavedPacket();
        packet.SetType<TPacketType>();
        packet.Channel = channel;
        packet.Payload = payload;
        return packet;
    }

    public void SetError(string error)
    {
        Error = error;
        State = SavedPacketState.Error;
    }

    public void SetSent()
    {
        State = SavedPacketState.Sent;
    }

    public static void SetPriority(int priority)
    {
        if (priority < 0)
            throw new ArgumentOutOfRangeException(nameof(priority), priority,
                "Priority must be greater than or equal to zero.");
    }

    public void SetType<TPacketType>() where TPacketType : class
    {
        Type = typeof(TPacketType).AssemblyQualifiedName!;
    }
}

public enum SavedPacketState
{
    New,
    Pending,
    Sent,
    Error
}
