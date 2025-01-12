using Gateway.Worker.Jobs;
using Gateway.Worker.Jobs.Base;
using Hangfire;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gateway.Worker;

public static class JobRegister
{
    public static void Register()
    {
        AddJob<HandleGatewayPackets>("*/1 * * * *");
    }

    private static void AddJob<TJob>(string cron) where TJob : BaseJob
    {
        RecurringJob.AddOrUpdate<HandleGatewayPackets>(typeof(TJob).Name, job => job.Execute(JobCancellationToken.Null), cron);
    }
}
