using Dapper;
using Gateway.Domain.Entities;
using Gateway.Domain.Extensions;
using Gateway.Domain.Persistence;
using MediatR;

namespace Gateway.Domain.Handlers.SavedPackets;

public record SetPacketState(Guid Id, SavedPacketState State, Guid? EventId = null, string? Error = null) : IRequest;

public sealed class SetPacketStateHandler(IConnectionProvider connectionProvider) : IRequestHandler<SetPacketState>
{
    private const string Query = "UPDATE public.saved_packet SET state = :state::saved_packet_state, error = :error, event_id = :eventId WHERE id = :id";
    public async Task Handle(SetPacketState request, CancellationToken cancellationToken)
    {
        using var cn = connectionProvider.GetConnection();
        await cn.ExecuteAsync(Query, new
        {
            state = request.State.GetDbFriendlyValue(),
            error = request.Error,
            eventId = request.EventId,
            id = request.Id
        });
    }
}
