// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

/// <summary>
/// Extension methods for adding an emulated IoT Edge device to an Aspire application.
/// </summary>
public static class IoTEdgeDeviceBuilderExtensions
{
    private const string DeviceResourceType = "IoTEdgeDevice";

    /// <summary>
    /// Adds an emulated IoT Edge device. On start, the device and a module identity for every resource attached with
    /// <see cref="IoTEdgeModuleBuilderExtensions.AsIoTEdgeModule{T}"/> are created in the IoT Hub (or reused if they exist).
    /// Without <see cref="WithEdgeHub"/>, modules connect directly to IoT Hub.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The resource name.</param>
    /// <param name="iotHubConnectionString">
    /// A parameter holding an IoT Hub service connection string with registry read/write permissions
    /// (and service connect for the dashboard commands), e.g. the iothubowner policy.
    /// </param>
    /// <param name="deviceId">The device id in IoT Hub. Defaults to the resource name followed by the machine name.</param>
    /// <returns>The resource builder.</returns>
    public static IResourceBuilder<IoTEdgeDeviceResource> AddIoTEdgeDevice(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        IResourceBuilder<ParameterResource> iotHubConnectionString,
        string? deviceId = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(iotHubConnectionString);

        deviceId = string.IsNullOrWhiteSpace(deviceId)
            ? SanitizeDeviceId($"{name}-{Environment.MachineName}")
            : deviceId;

        var certificateDirectory = Path.Combine(
            Path.GetTempPath(),
            "atc-aspire-iotedge",
            SanitizeDeviceId($"{builder.Environment.ApplicationName}-{name}"));

        Directory.CreateDirectory(certificateDirectory);

        var resource = new IoTEdgeDeviceResource(name, iotHubConnectionString.Resource, deviceId, certificateDirectory);

        return builder
            .AddResource(resource)
            .WithReferenceRelationship(iotHubConnectionString)
            .WithInitialState(new CustomResourceSnapshot
            {
                ResourceType = DeviceResourceType,
                State = KnownResourceStates.NotStarted,
                Properties =
                [
                    new ResourcePropertySnapshot(CustomResourceKnownProperties.Source, deviceId),
                    new ResourcePropertySnapshot("iotedge.deviceId", deviceId),
                    new ResourcePropertySnapshot("iotedge.certificates", certificateDirectory),
                ],
            })
            .OnInitializeResource(InitializeDevice)
            .WithDeleteDeviceCommand()
            .ExcludeFromManifest();
    }

    /// <summary>
    /// Sets the device id used in IoT Hub.
    /// </summary>
    /// <param name="builder">The device resource builder.</param>
    /// <param name="deviceId">The device id.</param>
    /// <returns>The resource builder.</returns>
    public static IResourceBuilder<IoTEdgeDeviceResource> WithDeviceId(
        this IResourceBuilder<IoTEdgeDeviceResource> builder,
        string deviceId)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        builder.Resource.DeviceId = deviceId;
        return builder;
    }

    /// <summary>
    /// Uses an IoT Edge deployment manifest (or deployment template): its edgeHub routes are applied to the local edgeHub,
    /// and the desired properties of each module twin in it are applied to the matching module identities.
    /// </summary>
    /// <param name="builder">The device resource builder.</param>
    /// <param name="path">The manifest path, relative to the AppHost directory or absolute.</param>
    /// <returns>The resource builder.</returns>
    public static IResourceBuilder<IoTEdgeDeviceResource> WithDeploymentManifest(
        this IResourceBuilder<IoTEdgeDeviceResource> builder,
        string path)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(path);

        var fullPath = Path.GetFullPath(path, builder.ApplicationBuilder.AppHostDirectory);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"The deployment manifest '{fullPath}' does not exist.", fullPath);
        }

        builder.Resource.DeploymentManifestPath = fullPath;
        return builder;
    }

    /// <summary>
    /// Runs a local edgeHub container that the modules connect through, like on a real IoT Edge device.
    /// Test certificates are generated for it, and its MQTT, AMQP and HTTPS ports are published on the host,
    /// because the IoT device SDK always connects on the standard ports.
    /// </summary>
    /// <param name="builder">The device resource builder.</param>
    /// <param name="configure">An optional callback to configure the edgeHub container.</param>
    /// <param name="mqttPort">The host port for MQTT. Must be 8883 for modules running on the host to connect over MQTT.</param>
    /// <param name="amqpPort">The host port for AMQP. Must be 5671 for modules running on the host to connect over AMQP.</param>
    /// <param name="httpsPort">
    /// The host port for MQTT/AMQP over WebSockets. Must be 443 for modules running on the host to connect over WebSockets.
    /// Set it to <see langword="null"/> to not publish it, e.g. when port 443 is taken.
    /// </param>
    /// <returns>The resource builder.</returns>
    public static IResourceBuilder<IoTEdgeDeviceResource> WithEdgeHub(
        this IResourceBuilder<IoTEdgeDeviceResource> builder,
        Action<IResourceBuilder<IoTEdgeHubResource>>? configure = null,
        int mqttPort = IoTEdgeHubResource.MqttPort,
        int amqpPort = IoTEdgeHubResource.AmqpPort,
        int? httpsPort = IoTEdgeHubResource.HttpsPort)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var device = builder.Resource;
        if (device.EdgeHub is not null)
        {
            throw new InvalidOperationException($"IoT Edge device '{device.Name}' already has an edgeHub.");
        }

        var name = $"{device.Name}-edgehub";
        var certificates = EdgeHubCertificateGenerator.EnsureCertificates(
            device.CertificateDirectory,
            [name, "localhost", "host.docker.internal", Environment.MachineName.ToLowerInvariant()]);

        var edgeHub = new IoTEdgeHubResource(name, device, certificates);
        device.EdgeHub = edgeHub;

        var healthCheckKey = $"{name}_check";
        builder.ApplicationBuilder.Services
            .AddHealthChecks()
            .Add(new HealthCheckRegistration(healthCheckKey, _ => new EdgeHubHealthCheck(mqttPort), failureStatus: null, tags: null));

        var edgeHubBuilder = builder.ApplicationBuilder
            .AddResource(edgeHub)
            .WithImage(IoTEdgeHubResource.DefaultImage, IoTEdgeHubResource.DefaultTag)
            .WithImageRegistry(IoTEdgeHubResource.DefaultRegistry)
            .WithContainerNetworkAlias(edgeHub.ContainerHostName)
            .WithBindMount(certificates.DirectoryPath, IoTEdgeHubResource.ContainerCertificateDirectory, isReadOnly: true)
            .WithEndpoint(port: mqttPort, targetPort: IoTEdgeHubResource.MqttPort, scheme: "tcp", name: IoTEdgeHubResource.MqttEndpointName, isProxied: false)
            .WithEndpoint(port: amqpPort, targetPort: IoTEdgeHubResource.AmqpPort, scheme: "tcp", name: IoTEdgeHubResource.AmqpEndpointName, isProxied: false)
            .WithEnvironment(EdgeHubEnvironmentVariableNames.EdgeModuleHubServerCertificateFile, $"{IoTEdgeHubResource.ContainerCertificateDirectory}/{EdgeHubCertificateFiles.ServerCertificateFileName}")
            .WithEnvironment(EdgeHubEnvironmentVariableNames.EdgeModuleHubServerCaChainCertificateFile, $"{IoTEdgeHubResource.ContainerCertificateDirectory}/{EdgeHubCertificateFiles.ChainCertificateFileName}")
            .WithEnvironment(EdgeHubEnvironmentVariableNames.ConfigSource, EdgeHubEnvironmentVariableNames.ConfigSourceLocal)
            .WithEnvironment(async context =>
            {
                if (!context.ExecutionContext.IsRunMode)
                {
                    return;
                }

                var identity = await device.WaitForProvisioningAsync(context.CancellationToken);
                context.EnvironmentVariables[EdgeHubEnvironmentVariableNames.IotHubConnectionString] = identity.GetEdgeHubConnectionString();

                // The edgeHub image ships a default route that sends everything upstream, and environment variables
                // can only add to it. An empty value crashes edgeHub, so it is replaced with a route that never matches.
                context.EnvironmentVariables[$"{EdgeHubEnvironmentVariableNames.RoutePrefix}{EdgeHubEnvironmentVariableNames.DefaultRouteName}"] =
                    EdgeHubEnvironmentVariableNames.DisabledRoute;

                var routes = device.DeploymentManifest?.Routes;
                if (routes is null || routes.Count == 0)
                {
                    context.EnvironmentVariables[$"{EdgeHubEnvironmentVariableNames.RoutePrefix}upstream"] = "FROM /messages/* INTO $upstream";
                    return;
                }

                foreach (var (routeName, route) in routes)
                {
                    context.EnvironmentVariables[$"{EdgeHubEnvironmentVariableNames.RoutePrefix}{routeName}"] = route;
                }
            })
            .WithHealthCheck(healthCheckKey)
            .WithParentRelationship(builder)
            .WaitFor(builder);

        if (httpsPort is not null)
        {
            edgeHubBuilder.WithEndpoint(port: httpsPort, targetPort: IoTEdgeHubResource.HttpsPort, scheme: "tcp", name: IoTEdgeHubResource.HttpsEndpointName, isProxied: false);
        }

        foreach (var module in device.Modules)
        {
            module.ModuleResource.Annotations.Add(new WaitAnnotation(edgeHub, WaitType.WaitUntilHealthy));
        }

        configure?.Invoke(edgeHubBuilder);

        return builder;
    }

    /// <summary>
    /// Sets the protocol edgeHub uses to connect to IoT Hub.
    /// </summary>
    /// <param name="builder">The edgeHub resource builder.</param>
    /// <param name="protocol">The upstream protocol.</param>
    /// <returns>The resource builder.</returns>
    public static IResourceBuilder<IoTEdgeHubResource> WithUpstreamProtocol(
        this IResourceBuilder<IoTEdgeHubResource> builder,
        EdgeHubUpstreamProtocol protocol)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithEnvironment(EdgeHubEnvironmentVariableNames.UpstreamProtocol, protocol.ToString());
    }

    private static async Task InitializeDevice(
        IoTEdgeDeviceResource resource,
        InitializeResourceEvent initializeEvent,
        CancellationToken cancellationToken)
    {
        var logger = initializeEvent.Logger;
        var notifications = initializeEvent.Notifications;

        await notifications.PublishUpdateAsync(resource, snapshot => snapshot with
        {
            State = KnownResourceStates.Starting,
            StartTimeStamp = DateTime.UtcNow,
        });

        try
        {
            var connectionString = await resource.IotHubConnectionString.GetValueAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"The parameter '{resource.IotHubConnectionString.Name}' has no IoT Hub connection string.");
            }

            var loggerFactory = initializeEvent.Services.GetRequiredService<ILoggerFactory>();
            var options = new IotHubOptions { ConnectionString = connectionString };
            resource.IoTHubModuleService = new IoTHubModuleService(options, loggerFactory);
            resource.IoTHubService = new IoTHubService(resource.IoTHubModuleService, options, loggerFactory);

            if (resource.DeploymentManifestPath is not null)
            {
                resource.DeploymentManifest = await DeploymentManifest.LoadAsync(resource.DeploymentManifestPath, cancellationToken);
                logger.LogDeploymentManifestLoaded(
                    resource.DeploymentManifestPath,
                    resource.DeploymentManifest.Routes.Count,
                    resource.DeploymentManifest.ModuleDesiredProperties.Count);
            }

            var iotHubHostName = IotHubConnectionStringBuilder.Create(connectionString).HostName;
            logger.LogProvisioningDevice(resource.DeviceId, iotHubHostName);

            var provisioner = new IoTEdgeDeviceProvisioner(resource.IoTHubService, iotHubHostName, loggerFactory);
            var identity = await provisioner.Provision(
                resource.DeviceId,
                resource.Modules.Select(x => x.ModuleId),
                resource.DeploymentManifest,
                cancellationToken);

            resource.SetProvisioned(identity);

            var mode = resource.EdgeHub is null
                ? "Direct to IoT Hub"
                : $"Through edgeHub ({resource.EdgeHub.Name})";
            var moduleIds = string.Join(", ", resource.Modules.Select(x => x.ModuleId));

            logger.LogDeviceReady(identity.DeviceId, moduleIds, mode);

            await notifications.PublishUpdateAsync(resource, snapshot => snapshot with
            {
                State = KnownResourceStates.Running,
                Properties =
                [
                    .. snapshot.Properties.Where(x => !x.Name.StartsWith("iothub.", StringComparison.Ordinal) && !string.Equals(x.Name, "iotedge.mode", StringComparison.Ordinal)),
                    new ResourcePropertySnapshot("iothub.hostName", identity.IotHubHostName),
                    new ResourcePropertySnapshot("iotedge.mode", mode),
                    new ResourcePropertySnapshot("iotedge.modules", moduleIds),
                ],
            });
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogProvisioningFailed(ex, resource.DeviceId, ex.Message);
            resource.SetProvisioningFailed(ex);

            await notifications.PublishUpdateAsync(resource, snapshot => snapshot with
            {
                State = KnownResourceStates.FailedToStart,
            });
        }
    }

    private static IResourceBuilder<IoTEdgeDeviceResource> WithDeleteDeviceCommand(
        this IResourceBuilder<IoTEdgeDeviceResource> builder)
        => builder.WithCommand(
            name: "iotedge-delete-device",
            displayName: "Delete device from IoT Hub",
            executeCommand: async context =>
            {
                var resource = builder.Resource;
                if (resource.IoTHubService is null)
                {
                    return CommandResults.Failure("The device has not been provisioned.");
                }

                var deleted = await resource.IoTHubService.DeleteDevice(resource.DeviceId, context.CancellationToken);
                return deleted
                    ? CommandResults.Success($"Deleted device '{resource.DeviceId}'. Restart the AppHost to provision it again.")
                    : CommandResults.Failure($"Could not delete device '{resource.DeviceId}'.");
            },
            commandOptions: new CommandOptions
            {
                Description = "Deletes the device and its module identities from IoT Hub.",
                ConfirmationMessage = "Delete the device and all its module identities from IoT Hub?",
                IconName = "Delete",
                UpdateState = context => builder.Resource.IoTHubService is null
                    ? ResourceCommandState.Disabled
                    : ResourceCommandState.Enabled,
            });

    private static string SanitizeDeviceId(string value)
        => Regex.Replace(value.ToLowerInvariant(), "[^a-z0-9\\-._:]", "-", RegexOptions.None, TimeSpan.FromSeconds(1));
}