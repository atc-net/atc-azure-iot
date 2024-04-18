namespace SimulationModule;

/// <summary>
/// The main SimulationModuleService.
/// </summary>
public sealed partial class SimulationModuleService : IHostedService
{
    private readonly IHostApplicationLifetime hostApplication;
    private readonly IModuleClientWrapper moduleClientWrapper;
    private readonly IScheduler scheduler;

    public SimulationModuleService(
        ILoggerFactory loggerFactory,
        IHostApplicationLifetime hostApplication,
        IModuleClientWrapper moduleClientWrapper,
        ISchedulerFactory schedulerFactory)
    {
        this.logger = loggerFactory.CreateLogger<SimulationModuleService>();
        this.hostApplication = hostApplication;
        this.moduleClientWrapper = moduleClientWrapper;
        this.scheduler = schedulerFactory.GetScheduler(hostApplication.ApplicationStopping).Result;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        hostApplication.ApplicationStarted.Register(OnStarted);
        hostApplication.ApplicationStopping.Register(OnStopping);
        hostApplication.ApplicationStopped.Register(OnStopped);

        moduleClientWrapper.SetConnectionStatusChangesHandler((status, reason) => LogConnectionStatusChange(status, reason));

        await moduleClientWrapper.OpenAsync(cancellationToken);
        LogModuleClientStarted(SimulationModuleConstants.ModuleId);

        await scheduler.Start(cancellationToken);

        var jobKey = JobKey.Create(nameof(PulsJob));

        var job = JobBuilder.Create<PulsJob>()
            .WithIdentity(jobKey)
            .Build();

        job.JobDataMap[SimulationModuleConstants.ModuleClient] = moduleClientWrapper;

        var trigger = TriggerBuilder.Create()
            .ForJob(jobKey)
            .StartNow()
            .WithSimpleSchedule(schedule =>
            {
                schedule
                    .WithIntervalInSeconds(5)
                    .RepeatForever();
            })
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public async Task StopAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await scheduler.Shutdown(waitForJobsToComplete: true, cancellationToken);

            await moduleClientWrapper.CloseAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Swallow OperationCanceledException
        }

        LogModuleClientStopped(SimulationModuleConstants.ModuleId);
    }

    private void OnStarted()
        => LogModuleStarted(SimulationModuleConstants.ModuleId);

    private void OnStopping()
        => LogModuleStopping(SimulationModuleConstants.ModuleId);

    private void OnStopped()
        => LogModuleStopped(SimulationModuleConstants.ModuleId);
}