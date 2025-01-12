using Bridge.Shared;
using Bridge.Shared.Models;
using Bridge.Shared.Models.Requests;
using Gateway.DataAccess.Migrations;
using Gateway.Infrastructure;
using Gateway.Worker;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;

var builder = WebApplication.CreateBuilder(args).UseCustomLogging();
var services = builder.Services;
builder.Services.AddHangfire(configuration => configuration
    .UsePostgreSqlStorage(opts =>
        opts.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Db"))));
builder.Services.AddHangfireServer();

services.AddDataAccess();

services.AddMassTransit(x =>
{
    x.UsingRabbitMq((_, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://localhost:63685"), h =>
        {
            h.Username("user");
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

app.UseHttpsRedirection();
app.UseHangfireDashboard();

JobRegister.Register();

app.MigrateDatabaseByType<Migration_2025_01_10_17_28_13>();
app.Run();
