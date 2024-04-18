namespace OpcPublisherNodeManagerModule;

/// <summary>
/// The main OpcPublisherNodeManagerModuleService.
/// </summary>
public sealed partial class OpcPublisherNodeManagerModuleService : IHostedService
{
    private readonly IHostApplicationLifetime hostApplication;
    private readonly IModuleClientWrapper moduleClientWrapper;
    private readonly IMethodResponseFactory methodResponseFactory;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly JsonSerializerOptions fileSerializerOptions;

    public OpcPublisherNodeManagerModuleService(
        ILoggerFactory loggerFactory,
        IHostApplicationLifetime hostApplication,
        IModuleClientWrapper moduleClientWrapper,
        IMethodResponseFactory methodResponseFactory)
    {
        this.logger = loggerFactory.CreateLogger<OpcPublisherNodeManagerModuleService>();
        this.hostApplication = hostApplication;
        this.moduleClientWrapper = moduleClientWrapper;
        this.methodResponseFactory = methodResponseFactory;

        jsonSerializerOptions = JsonSerializerOptionsFactory.Create(new JsonSerializerFactorySettings
        {
            UseConverterDatetimeOffsetMinToNull = true,
        });

        fileSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        hostApplication.ApplicationStarted.Register(OnStarted);
        hostApplication.ApplicationStopping.Register(OnStopping);
        hostApplication.ApplicationStopped.Register(OnStopped);

        moduleClientWrapper.SetConnectionStatusChangesHandler(LogConnectionStatusChange);

        await moduleClientWrapper.OpenAsync(cancellationToken);
        LogModuleClientStarted(OpcPublisherNodeManagerModuleConstants.ModuleId);

        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpoints, GetEndpoints, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointWithNodes, GetEndpointWithNodes, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodGetEndpointsWithEmptyOpcNodesList, GetEndpointsWithEmptyOpcNodesList, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodAddEndpoint, AddEndpoint, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodAddNodeToEndpoint, AddNodeToEndpoint, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodAddNodesToEndpoint, AddNodesToEndpoint, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodUpdateNodeOnEndpoint, UpdateNodeOnEndpoint, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodUpdateNodesOnEndpoint, UpdateNodesOnEndpoint, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveNodeFromEndpoint, RemoveNodeFromEndpoint, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveNodesFromEndpoint, RemoveNodesFromEndpoint, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveAllNodesFromEndpoint, RemoveAllNodesFromEndpoint, string.Empty, cancellationToken);
        await moduleClientWrapper.SetMethodHandlerAsync(OpcPublisherNodeManagerModuleConstants.DirectMethodRemoveEndpoint, RemoveEndpoint, string.Empty, cancellationToken);
    }

    public async Task StopAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await moduleClientWrapper.CloseAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Swallow OperationCanceledException
        }

        LogModuleClientStopped(OpcPublisherNodeManagerModuleConstants.ModuleId);
    }

    private void OnStarted()
        => LogModuleStarted(OpcPublisherNodeManagerModuleConstants.ModuleId);

    private void OnStopping()
        => LogModuleStopping(OpcPublisherNodeManagerModuleConstants.ModuleId);

    private void OnStopped()
        => LogModuleStopped(OpcPublisherNodeManagerModuleConstants.ModuleId);
}