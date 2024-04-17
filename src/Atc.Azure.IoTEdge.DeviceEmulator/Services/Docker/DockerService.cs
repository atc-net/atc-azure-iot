namespace Atc.Azure.IoTEdge.DeviceEmulator.Services.Docker;

/// <summary>
/// The main DockerService - Handles call execution.
/// </summary>
public partial class DockerService : IDockerService
{
    private const string DeviceContainerImage = "toolboc/azure-iot-edge-device-container";

    public string ContainerId { get; set; } = string.Empty;

    public string ContainerName { get; set; } = string.Empty;

    private readonly ISystemEnvironmentService systemEnvironmentService;
    private readonly DockerClient dockerClient;

    public DockerService(
        ILogger logger,
        ISystemEnvironmentService systemEnvironmentService)
    {
        this.logger = logger;
        this.systemEnvironmentService = systemEnvironmentService;
        this.dockerClient = CreateDockerClient();
    }

    public async Task<bool> StartContainer(
        string containerName,
        string deviceConnectionString,
        CancellationToken cancellationToken)
    {
        this.ContainerName = containerName;
        var exposedPorts = new[] { 15580, 15581, 443, 8883, 5671 };

        var pullAndCreateDockerImageSucceeded = await PullAndCreateDockerImage(cancellationToken);
        if (!pullAndCreateDockerImageSucceeded)
        {
            return false;
        }

        var stopAndRemoveExistingContainersSucceeded = await StopAndRemoveExistingContainers(cancellationToken);
        if (!stopAndRemoveExistingContainersSucceeded)
        {
            return false;
        }

        var createContainerSucceeded = await CreateContainer(deviceConnectionString, exposedPorts, cancellationToken);
        if (!createContainerSucceeded)
        {
            return false;
        }

        return await StartContainer(cancellationToken);
    }

    public async Task<bool> StopContainer(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(ContainerId))
        {
            LogContainerStopMissingContainerId(ContainerName);
            return false;
        }

        var stopAndRemoveExistingContainersSucceeded = await StopAndRemoveExistingContainers(cancellationToken);
        if (!stopAndRemoveExistingContainersSucceeded)
        {
            return false;
        }

        ContainerId = string.Empty;
        ContainerName = string.Empty;

        return true;
    }

    public async Task<bool> EnsureIotEdgeEmulatorIsReady(
        string moduleName,
        int numberOfRetries = 15,
        int secondsBetweenRetries = 3)
    {
        var dockerCommand = $"docker exec {ContainerName} bash -c \"docker ps\"";
        var cmdPath = systemEnvironmentService.GetCommandFilePath();

        LogIotEdgeEmulatorCheck(moduleName, ContainerName, ContainerId);
        for (var i = 1; i <= numberOfRetries; i++)
        {
            var commandPrefix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/C" : "-c";
            var (isSuccessful, output) = await ProcessHelper.Execute(cmdPath, $"{commandPrefix} {dockerCommand}");
            if (!isSuccessful)
            {
                LogDockerProcessCommandFailure(output);
                return false;
            }

            if (output.Contains(moduleName, StringComparison.Ordinal))
            {
                LogIotEdgeEmulatorReady(moduleName, ContainerName, ContainerId);
                return true;
            }

            LogIotEdgeEmulatorWaiting(moduleName, ContainerName, ContainerId, secondsBetweenRetries, i, numberOfRetries);
            await Task.Delay(TimeSpan.FromSeconds(secondsBetweenRetries));
        }

        return false;
    }

    public async Task<(bool Succeeded, IDictionary<string, string>? IotEdgeVariables)> GetIotEdgeVariablesFromContainer(
        string moduleName)
    {
        var dockerCommand = $"docker exec {ContainerName} bash -c \"docker exec {moduleName} env | grep IOTEDGE_\"";

        var cmdPath = systemEnvironmentService.GetCommandFilePath();
        var (isSuccessful, output) = await ProcessHelper.Execute(cmdPath, $"/C {dockerCommand}");
        if (!isSuccessful)
        {
            LogDockerProcessCommandFailure(output);
            return (false, null);
        }

        var variables = output
            .EnsureEnvironmentNewLines()
            .Split(System.Environment.NewLine)
            .Where(x => x.Contains('=', StringComparison.Ordinal))
            .ToDictionary(e => e.Split('=')[0], e => e.Split('=')[1], StringComparer.Ordinal);

        LogIotEdgeEmulatorFetchingVariables(moduleName);
        return (true, variables);
    }

    private DockerClient CreateDockerClient()
    {
        var localDockerSocketUri = GetLocalDockerSocketUri();
        using var dockerClientConfiguration = new DockerClientConfiguration(localDockerSocketUri);
        var client = dockerClientConfiguration.CreateClient();
        LogDockerClientCreated();
        return client;
    }

    private async Task<bool> PullAndCreateDockerImage(
        CancellationToken cancellationToken)
    {
        LogDockerImageDownloadStarted(DeviceContainerImage);

        try
        {
            await dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = DeviceContainerImage, Tag = "latest", },
                new AuthConfig(),
                new Progress<JSONMessage>(e =>
                {
                    if (!string.IsNullOrEmpty(e.Status))
                    {
                        LogDockerImageDownloadStatus(e.Status);
                    }

                    if (!string.IsNullOrEmpty(e.ProgressMessage))
                    {
                        LogDockerImageDownloadStatus(e.ProgressMessage);
                    }

                    if (!string.IsNullOrEmpty(e.ErrorMessage))
                    {
                        LogDockerImageDownloadStatus(e.ErrorMessage);
                    }
                }),
                cancellationToken);

            LogDockerImageDownloadSucceeded(DeviceContainerImage);
            return true;
        }
        catch (DockerApiException ex)
        {
            LogDockerImageDownloadFailed(DeviceContainerImage, ex.Message);
            return false;
        }
    }

    private async Task<bool> CreateContainer(
        string deviceConnectionString,
        int[] exposedPorts,
        CancellationToken cancellationToken)
    {
        LogContainerCreationStarted(ContainerName);

        try
        {
            var containerResponse = await dockerClient.Containers.CreateContainerAsync(
                new CreateContainerParameters
                {
                    AttachStderr = true,
                    AttachStdin = true,
                    AttachStdout = true,
                    Tty = true,
                    Env = new List<string> { $"connectionString={deviceConnectionString}" },
                    Name = ContainerName,
                    Image = DeviceContainerImage,
                    ExposedPorts = GetExposedPortsDictionary(exposedPorts),
                    HostConfig = new HostConfig
                    {
                        Privileged = true,
                        PortBindings = GetPortBindingsDictionary(exposedPorts),
                        PublishAllPorts = true,
                    },
                },
                cancellationToken);

            ContainerId = containerResponse.ID;
            LogContainerCreationSucceeded(ContainerName);
            return true;
        }
        catch (DockerApiException ex)
        {
            LogContainerCreationFailed(ContainerName, ex.Message);
            return false;
        }
    }

    private async Task<bool> StartContainer(
        CancellationToken cancellationToken)
    {
        LogContainerStarting(ContainerName, ContainerId);

        try
        {
            var startContainerSucceeded = await dockerClient.Containers.StartContainerAsync(
                ContainerId,
                parameters: null,
                cancellationToken);

            if (!startContainerSucceeded)
            {
                LogContainerStartFailed(ContainerName, ContainerId, "Could not start the container from DockerClient.");
                return startContainerSucceeded;
            }

            LogContainerStartSucceeded(ContainerName, ContainerId);
            return startContainerSucceeded;
        }
        catch (DockerApiException ex)
        {
            LogContainerStartFailed(ContainerName, ContainerId, ex.Message);
            return false;
        }
    }

    private async Task<bool> StopAndRemoveExistingContainers(
        CancellationToken cancellationToken)
    {
        try
        {
            var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true }, cancellationToken);
            foreach (var container in containers)
            {
                if (!container.Names.Contains(ContainerName, StringComparer.Ordinal) &&
                    !container.Names.Contains($@"/{ContainerName}", StringComparer.Ordinal))
                {
                    continue;
                }

                if (container.State == "running")
                {
                    LogContainerStopping(ContainerName, container.ID);
                    await dockerClient.Containers.StopContainerAsync(container.ID, new ContainerStopParameters(), cancellationToken);
                    LogContainerStopSucceeded(ContainerName, container.ID);
                }

                LogContainerRemovalStarting(ContainerName, ContainerId);
                await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters(), cancellationToken);
                LogContainerRemovalSucceeded(ContainerName, ContainerId);
                break;
            }

            return true;
        }
        catch (DockerApiException ex)
        {
            LogContainerStopOrRemovalFailed(ContainerName, ContainerId, ex.Message);
            return false;
        }
    }

    private static Uri GetLocalDockerSocketUri()
    {
        var localDockerSocket = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? @"npipe://./pipe/docker_engine"
            : @"unix:/var/run/docker.sock";

        return new Uri(localDockerSocket);
    }

    private static Dictionary<string, IList<PortBinding>> GetPortBindingsDictionary(
        IEnumerable<int> exposedPorts)
        => exposedPorts.ToDictionary(
            x => x.ToString(GlobalizationConstants.EnglishCultureInfo),
            x => (IList<PortBinding>)new List<PortBinding>
            {
                new() { HostPort = x.ToString(GlobalizationConstants.EnglishCultureInfo), },
            },
            StringComparer.Ordinal);

    private static Dictionary<string, EmptyStruct> GetExposedPortsDictionary(
        IEnumerable<int> exposedPorts)
        => exposedPorts.ToDictionary(
            x => x.ToString(GlobalizationConstants.EnglishCultureInfo),
            _ => default(EmptyStruct),
            StringComparer.Ordinal);
}