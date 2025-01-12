using Hangfire;

namespace Gateway.Worker.Jobs.Base;

public abstract class BaseJob
{
    public abstract Task Execute(IJobCancellationToken token);
}
