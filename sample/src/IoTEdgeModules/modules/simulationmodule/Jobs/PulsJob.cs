namespace SimulationModule.Jobs;

/// <summary>
/// PulsJob - Handles job execution.
/// </summary>
[DisallowConcurrentExecution]
public sealed partial class PulsJob : IJob
{
    private readonly NumberGenerator numberGenerator;

    public PulsJob(
        NumberGenerator numberGenerator,
        ILogger<PulsJob> logger)
    {
        this.logger = logger;
        this.numberGenerator = numberGenerator;
    }

    public async Task Execute(
        IJobExecutionContext context)
    {
        LogJobStarted(nameof(PulsJob));

        try
        {
            var moduleClient = (IModuleClientWrapper?)context.JobDetail.JobDataMap[SimulationModuleConstants.ModuleClient];
            if (moduleClient is null)
            {
                LogFailedToAcquireModuleClient(nameof(PulsJob));
                return;
            }

            var temperature = numberGenerator.GetNextValue();
            var json = $$"""{"temperature":{{temperature}}}""";

            using var message = new Message(Encoding.UTF8.GetBytes(json));
            message.ContentEncoding = Encoding.UTF8.WebName;
            message.ContentType = MediaTypeNames.Application.Json;

            await moduleClient.SendEventAsync("temperature", message, context.CancellationToken);
        }
        catch (Exception ex)
        {
            LogUnhandledExceptionInJob(
                nameof(PulsJob),
                ex.GetLastInnerMessage());
        }
        finally
        {
            LogJobEnded(nameof(PulsJob));
        }
    }
}