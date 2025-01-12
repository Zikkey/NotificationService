using Bridge.Shared.Constants;
using Bridge.Shared.Models.Responses;
using Gateway.Domain.Entities;
using Gateway.Domain.Handlers.SavedPackets;
using MassTransit;
using MediatR;

namespace Gateway.Consumer.Consumers;

public sealed class PacketProcessedConsumer(IMediator mediator, ILogger<PacketProcessedConsumer> logger) : IConsumer<PacketProcessed>
{
    public async Task Consume(ConsumeContext<PacketProcessed> context)
    {
        var packet = context.Message;
        if (!context.TryGetHeader(EventHeaders.CorrelationId, out Guid? correlationId))
        {
            logger.LogError("Correlation id not found");
            return;
        }

        if (packet.IsProcessed)
        {
            await SetSent(correlationId!.Value);
        }
        else
        {
            await SetError(correlationId!.Value, packet.ErrorMessage);
        }
    }

    private async Task SetError(Guid eventId, string? error)
        => await mediator.Send(new SetPacketStateByEventId(eventId, SavedPacketState.Error, Error: error));

    private async Task SetSent(Guid eventId)
        => await mediator.Send(new SetPacketStateByEventId(eventId, SavedPacketState.Sent));
}
