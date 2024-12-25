using Bridge.Shared.Models;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();
services.AddMassTransit(x =>
{
    x.UsingRabbitMq((_, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://localhost:63685"), h =>
        {
            h.Username("username");
            h.Password("password");
        });
        
        cfg.Message<EmailPacket>(m => 
        {
            m.SetEntityName("email-packet");
        });
        
        cfg.Message<SmsPacket>(m => 
        {
            m.SetEntityName("sms-packet");
        });
        
        cfg.Message<PushPacket>(m => 
        {
            m.SetEntityName("push-packet");
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();