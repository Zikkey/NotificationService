using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHangfire(configuration => configuration
    .UsePostgreSqlStorage(opts =>
        opts.UseNpgsqlConnection(builder.Configuration.GetConnectionString("gatewaydb"))));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseHangfireDashboard();
app.Run();