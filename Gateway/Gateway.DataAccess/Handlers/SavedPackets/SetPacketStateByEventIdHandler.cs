using Dapper;
using Gateway.Domain.Entities;
using Gateway.Domain.Extensions;
using Gateway.Domain.Persistence;
using MediatR;

namespace Gateway.Domain.Handlers.SavedPackets;

public record SetPacketStateByEventId(Guid EventId, SavedPacketState State, string? Error = null) : IRequest;

public sealed class SetPacketStateByEventIdHandler(IConnectionProvider connectionProvider) : IRequestHandler<SetPacketStateByEventId>
{
    private const string Query = "UPDATE public.saved_packet SET state = :state::saved_packet_state, error = :error WHERE event_id = :eventId";
    public async Task Handle(SetPacketStateByEventId request, CancellationToken cancellationToken)
    {
        using var cn = connectionProvider.GetConnection();
        await cn.ExecuteAsync(Query, new
        {
            state = request.State.GetDbFriendlyValue(),
            error = request.Error,
            eventId = request.EventId
        });
    }
}
