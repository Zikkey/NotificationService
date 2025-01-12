using Dapper;
using Gateway.Domain.Entities;
using Gateway.Domain.Extensions;
using Gateway.Domain.Persistence;
using MediatR;

namespace Gateway.Domain.Handlers.SavedPackets;

public record SavePacket(SavedPacket Packet) : IRequest;

public class SavePacketHandler(IConnectionProvider connectionProvider) : IRequestHandler<SavePacket>
{
    private const string Query = @"INSERT INTO public.saved_packet (
                                   id,
                                   payload,
                                   created_on,
                                   state,
                                   error,
                                   priority,
                                   type,
                                   event_id,
                                   channel)
                                   VALUES (
                                   :id, 
                                   :payload::jsonb, 
                                   :createdOn, 
                                   :stateString::saved_packet_state, 
                                   :error, 
                                   :priority, 
                                   :type, 
                                   :eventId, 
                                   :channelString::notification_channel)";

    public async Task Handle(SavePacket request, CancellationToken cancellationToken)
    {
        request.Packet.HandleIdentifiedEntity();
        request.Packet.HandleCreatedAuditedEntity();
        using var cn = connectionProvider.GetConnection();
        await cn.ExecuteAsync(Query, request.Packet);
    }
}
