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
        IHostApplicationLifetime hostApplication,
        IModuleClientWrapper moduleClientWrapper,
        IScheduler scheduler,
        ILoggerFactory? loggerFactory = null)
    {
        this.hostApplication = hostApplication;
        this.moduleClientWrapper = moduleClientWrapper;
        this.scheduler = scheduler;
        this.logger = loggerFactory is not null
            ? loggerFactory.CreateLogger<SimulationModuleService>()
            : NullLogger<SimulationModuleService>.Instance;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        hostApplication.ApplicationStarted.Register(OnStarted);
        hostApplication.ApplicationStopping.Register(OnStopping);
        hostApplication.ApplicationStopped.Register(OnStopped);

        moduleClientWrapper.SetConnectionStatusChangesHandler((status, reason) => LogConnectionStatusChange(status, reason));

        await moduleClientWrapper.OpenAsync(cancellationToken);
        LogModuleClientStarted(SimulationModuleConstants.ModuleId);

        await scheduler.Start(cancellationToken);

        var jobKey = new JobKey(nameof(PulsJob));

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
                    .WithInterval(TimeSpan.FromSeconds(5))
                    .RepeatForever();
            })
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await scheduler.Shutdown(waitForJobsToComplete: true, cancellationToken: cancellationToken);

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