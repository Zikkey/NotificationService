using Bridge.Shared;
using Bridge.Shared.Models;
using Bridge.Shared.Models.Requests;
using Gateway.DataAccess.Migrations;
using Gateway.Infrastructure;
using MassTransit;

var builder = WebApplication.CreateBuilder(args).UseCustomLogging();

var services = builder.Services;

builder.ConfigureNotificationPublisher();

services.AddDataAccess();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MigrateDatabaseByType<Migration_2025_01_10_17_28_13>();
app.Run();
