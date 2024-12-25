using Bridge.Shared.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Controllers.Public;

[Controller]
[Route("api/v1/internal/[controller]")]
public class SendController(IPublishEndpoint publishEndpoint, ILogger<SendController> logger) : ControllerBase
{
    [HttpPost("email")]
    public async Task Email(EmailPacket packet)
    {
        // save packet
        // trying to send packet
        // save state of packet
        // make job with retries
        await publishEndpoint.Publish(packet);
        logger.LogInformation("Sent email packet");
    }
    
    [HttpPost("sms")]
    public async Task Sms(SmsPacket packet)
    {
        await publishEndpoint.Publish(packet);
        logger.LogInformation("Sent sms packet");
    }
    
    [HttpPost("push")]
    public async Task Push(PushPacket packet)
    {
        await publishEndpoint.Publish(packet);
        logger.LogInformation("Sent push packet");
    }
}