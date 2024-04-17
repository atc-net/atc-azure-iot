namespace Atc.Azure.IoTEdge.DeviceEmulator.Services;

/// <summary>
/// The main IoTEdgeEmulationService - Handles call execution.
/// </summary>
public partial class IoTEdgeEmulationService : IIoTEdgeEmulationService
{
    private const string IotEdgeWorkLoadUri = "IOTEDGE_WORKLOADURI";
    private const string IotEdgeGatewayHostname = "IOTEDGE_GATEWAYHOSTNAME";

    private readonly IFileService fileService;
    private readonly IDockerService dockerService;
    private readonly IAzureIoTHubService azureIoTHubService;

    public IoTEdgeEmulationService(
        ILogger logger,
        IFileService fileService,
        IDockerService dockerService,
        IAzureIoTHubService azureIoTHubService)
    {
        this.logger = logger;
        this.fileService = fileService;
        this.dockerService = dockerService;
        this.azureIoTHubService = azureIoTHubService;
    }

    public string? IotHubConnectionString { get; set; }

    public string DeviceId { get; set; } = $"{DeviceEmulationConstants.DefaultDeviceId}_{Dns.GetHostName()}";

    public string ContainerName { get; set; } = DeviceEmulationConstants.DefaultContainerName;

    public async Task<bool> StartEmulator(
        string filePath,
        string iotHubConnectionString,
        string? deviceId,
        string? containerName,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(iotHubConnectionString);

        IotHubConnectionString = iotHubConnectionString;

        if (!string.IsNullOrEmpty(deviceId))
        {
            DeviceId = deviceId;
        }

        if (!string.IsNullOrEmpty(containerName))
        {
            ContainerName = containerName;
        }

        LogIotEdgeEmulatorStarting(DeviceId);

        var (getTemplateContentSucceeded, templateContent) = await GetTemplateContent(filePath, cancellationToken);
        if (!getTemplateContentSucceeded)
        {
            return false;
        }

        var (transformSucceeded, emulationManifest) = azureIoTHubService.TransformTemplateToEmulationManifest(templateContent);
        if (!transformSucceeded)
        {
            return false;
        }

        var (deviceProvisioningSucceeded, deviceConnectionString) = await azureIoTHubService
            .ProvisionIotEdgeDevice(
                emulationManifest,
                IotHubConnectionString,
                DeviceId,
                cancellationToken);

        if (!deviceProvisioningSucceeded)
        {
            return false;
        }

        return await dockerService.StartContainer(
            ContainerName,
            deviceConnectionString,
            cancellationToken);
    }

    public async Task<bool> StopEmulator(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(IotHubConnectionString) ||
            string.IsNullOrEmpty(DeviceId))
        {
            return false;
        }

        LogIotEdgeEmulatorStopping(DeviceId);

        var stopContainerSucceeded = await dockerService.StopContainer(cancellationToken);
        if (!stopContainerSucceeded)
        {
            return false;
        }

        var removeIotEdgeDeviceSucceeded = await azureIoTHubService.RemoveIotEdgeDevice(
            DeviceId,
            cancellationToken);

        IotHubConnectionString = string.Empty;
        DeviceId = string.Empty;

        return removeIotEdgeDeviceSucceeded;
    }

    public void EnsureProperIotEdgeEnvironmentVariables(
        IDictionary<string, string> environmentVariables)
    {
        ArgumentNullException.ThrowIfNull(environmentVariables);

        LogIotEdgeEmulatorEnsuringProperVariables();

        if (environmentVariables.ContainsKey(IotEdgeWorkLoadUri))
        {
            environmentVariables[IotEdgeWorkLoadUri] = "http://127.0.0.1:15581/";
        }
        else
        {
            environmentVariables.Add(IotEdgeWorkLoadUri, "http://127.0.0.1:15581/");
        }

        if (environmentVariables.ContainsKey(IotEdgeGatewayHostname))
        {
            environmentVariables[IotEdgeGatewayHostname] = Dns.GetHostName();
        }
        else
        {
            environmentVariables.Add(IotEdgeGatewayHostname, Dns.GetHostName());
        }
    }

    private async Task<(bool Succeeded, string TemplateContent)> GetTemplateContent(
        string filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            var templateContent = await fileService.ReadTemplateContent(filePath, cancellationToken);
            LogIotEdgeEmulatorGetTemplateContentSucceeded();
            return (true, templateContent);
        }
        catch (FileNotFoundException ex)
        {
            LogIotEdgeEmulatorGetTemplateContentFailed(ex.Message);
            return (false, ex.Message);
        }
        catch (FormatException ex)
        {
            LogIotEdgeEmulatorGetTemplateContentFailed(ex.Message);
            return (false, ex.Message);
        }
    }
}