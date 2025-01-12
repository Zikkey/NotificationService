using Hangfire.Client;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Gateway.Worker.Attributes
{
    public class SkipWhenPreviousJobIsRunningAttribute : JobFilterAttribute, IClientFilter, IApplyStateFilter
    {
        private const string RecurringJobIdKey = "RecurringJobId";
        private const string RunningKey = "Running";
        private const string RunningYesValue = "yes";
        private const string RunningNoValue = "no";
        private readonly ILogger logger = Log.Logger;

        public void OnCreating(CreatingContext context)
        {
            if (context.Connection is not JobStorageConnection connection) return;
            if (!context.Parameters.TryGetValue(RecurringJobIdKey, out var recurringJobIdValue)) return;
            var recurringJobId = recurringJobIdValue as string;
            if (string.IsNullOrWhiteSpace(recurringJobId)) return;
            logger.Information("Attempt create job instance '{JobId}'", recurringJobId);
            var running = connection.GetValueFromHash($"recurring-job:{recurringJobId}", RunningKey);
            if (string.Equals(RunningYesValue, running, StringComparison.OrdinalIgnoreCase))
            {
                context.Canceled = true;
                logger.Information("Attempt canceled create job instance '{JobId}'", recurringJobId);
            }
            else
            {
                logger.Information("Attempt success create job instance '{JobId}'", recurringJobId);
            }
        }

        public void OnCreated(CreatedContext filterContext)
        {
        }

        private string GetJobId(ApplyStateContext context)
        {
            return SerializationHelper.Deserialize<string>(
                context.Connection.GetJobParameter(context.BackgroundJob.Id, RecurringJobIdKey));
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            if (context.NewState is EnqueuedState)
            {
                WriteJobStatus(context, transaction, RunningYesValue);
            }
            else if (context.NewState.IsFinal || context.NewState is FailedState)
            {
                WriteJobStatus(context, transaction, RunningNoValue);
            }
        }

        private void WriteJobStatus(ApplyStateContext context, IWriteOnlyTransaction transaction, string runningValue)
        {
            var recurringJobId = GetJobId(context);
            if (string.IsNullOrWhiteSpace(recurringJobId)) return;
            transaction.SetRangeInHash($"recurring-job:{recurringJobId}",
                new[] { new KeyValuePair<string, string>(RunningKey, runningValue) });
            logger.Information("Success write running value '{RunningValue}' for job '{JobId}'", runningValue,
                recurringJobId);
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
        }
    }
}
