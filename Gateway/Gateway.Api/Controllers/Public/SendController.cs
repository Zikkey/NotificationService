using System.Text.Json;
using Bridge.Shared.Models;
using Bridge.Shared.Models.Requests;
using Gateway.Domain.Entities;
using Gateway.Domain.Handlers;
using Gateway.Domain.Handlers.SavedPackets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Controllers.Public;

[Controller]
[Route("api/v1/internal/[controller]")]
public class SendController(IMediator mediator, ILogger<SendController> logger) : ControllerBase
{
    [HttpPost("email")]
    public async Task Email(EmailPacket packet, CancellationToken token)
    {
        await mediator.Send(new SavePacket(ConvertToSaved(packet)), token);
        logger.LogInformation("Sent email packet");
    }

    [HttpPost("sms")]
    public async Task Sms(SmsPacket packet, CancellationToken token)
    {
        await mediator.Send(new SavePacket(ConvertToSaved(packet)), token);
        logger.LogInformation("Sent sms packet");
    }

    [HttpPost("push")]
    public async Task Push(PushPacket packet, CancellationToken token)
    {
        await mediator.Send(new SavePacket(ConvertToSaved(packet)), token);
        logger.LogInformation("Sent push packet");
    }

    private static SavedPacket ConvertToSaved<T>(T packet) where T : Packet
    {
        var payload = JsonSerializer.SerializeToDocument(packet);
        var savedPacket = SavedPacket.CreatePacket<T>(payload, packet.Channel);
        return savedPacket;
    }
}
