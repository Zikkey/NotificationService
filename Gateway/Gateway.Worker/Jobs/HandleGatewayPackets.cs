using System.Text.Json;
using Bridge.Shared.Constants;
using Bridge.Shared.Models.Requests;
using Gateway.Domain.Entities;
using Gateway.Domain.Handlers.SavedPackets;
using Gateway.Worker.Attributes;
using Gateway.Worker.Jobs.Base;
using Hangfire;
using MassTransit;
using MediatR;

namespace Gateway.Worker.Jobs;

[AutomaticRetry(Attempts = 0)]
[SkipWhenPreviousJobIsRunning]
public sealed class HandleGatewayPackets(
    IMediator mediator,
    IPublishEndpoint publishEndpoint,
    ILogger<HandleGatewayPackets> logger) : BaseJob
{
    private const int BatchLimit = 100;

    public override async Task Execute(IJobCancellationToken token)
    {
        var pending = await mediator.Send(new GetNewPackets(BatchLimit), token.ShutdownToken);
        foreach (var savedPacket in pending)
        {
            try
            {
                var packet = ConvertFromSaved(savedPacket);
                var correlationId = Guid.NewGuid();
                await SetPending(savedPacket, correlationId);
                await publishEndpoint.Publish(packet,
                    context => context.Headers.Set(EventHeaders.CorrelationId, correlationId));
                logger.LogInformation("Sent packet with type {PacketType}", savedPacket.Type);
            }
            catch (Exception ex)
            {
                await SetError(savedPacket, ex.Message);
            }
        }
    }

    private async Task SetError(SavedPacket packet, string error)
        => await mediator.Send(new SetPacketState(packet.Id!.Value, SavedPacketState.Error, Error: error));

    private async Task SetPending(SavedPacket packet, Guid? eventId)
        => await mediator.Send(new SetPacketState(packet.Id!.Value, SavedPacketState.Pending, eventId));

    private static object ConvertFromSaved(SavedPacket savedPacket)
    {
        var packetType = Type.GetType(savedPacket.Type);

        if (packetType == null)
        {
            throw new Exception($"Packet type {savedPacket.Type} not found");
        }

        return savedPacket.Payload.Deserialize(packetType)!;
    }
}
