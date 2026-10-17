namespace Atc.Azure.IoTEdge.DeviceEmulator.Provisioning;

/// <summary>
/// Provisions the IoT Hub identities an emulated IoT Edge device needs, using an IoT Hub service connection string
/// with registry read/write permissions.
/// </summary>
public sealed partial class IoTEdgeDeviceProvisioner : IIoTEdgeDeviceProvisioner
{
    private readonly IIoTHubService iotHubService;
    private readonly string iotHubHostName;

    public IoTEdgeDeviceProvisioner(
        IIoTHubService iotHubService,
        string iotHubHostName,
        ILoggerFactory? loggerFactory = null)
    {
        ArgumentNullException.ThrowIfNull(iotHubService);
        ArgumentException.ThrowIfNullOrEmpty(iotHubHostName);

        this.iotHubService = iotHubService;
        this.iotHubHostName = iotHubHostName;
        logger = loggerFactory is not null
            ? loggerFactory.CreateLogger<IoTEdgeDeviceProvisioner>()
            : NullLogger<IoTEdgeDeviceProvisioner>.Instance;
    }

    public async Task<IoTEdgeDeviceIdentity> Provision(
        string deviceId,
        IEnumerable<string> moduleIds,
        DeploymentManifest? deploymentManifest,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentNullException.ThrowIfNull(moduleIds);

        await EnsureEdgeDevice(deviceId, cancellationToken);

        var moduleKeys = await EnsureModules(
            deviceId,
            [.. moduleIds.Distinct(StringComparer.Ordinal)],
            cancellationToken);

        if (deploymentManifest is not null)
        {
            await ApplyModuleDesiredProperties(deviceId, moduleKeys.Keys, deploymentManifest, cancellationToken);
        }

        LogDeviceProvisioned(iotHubHostName, deviceId, moduleKeys.Count);
        return new IoTEdgeDeviceIdentity(iotHubHostName, deviceId, moduleKeys);
    }

    public async Task<bool> Deprovision(
        string deviceId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        var deleted = await iotHubService.DeleteDevice(deviceId, cancellationToken);
        if (deleted)
        {
            LogDeviceDeprovisioned(iotHubHostName, deviceId);
        }

        return deleted;
    }

    private async Task EnsureEdgeDevice(
        string deviceId,
        CancellationToken cancellationToken)
    {
        var device = await iotHubService.GetDevice(deviceId, cancellationToken);
        if (device is null)
        {
            var (succeeded, _) = await iotHubService.CreateDevice(deviceId, edgeEnabled: true, cancellationToken);
            if (!succeeded)
            {
                throw new InvalidOperationException($"Could not create IoT Edge device '{deviceId}' in IoT Hub '{iotHubHostName}'. Check that the connection string has registry write permissions.");
            }

            LogDeviceCreated(iotHubHostName, deviceId);
            return;
        }

        if (device.Capabilities?.IotEdge != true)
        {
            throw new InvalidOperationException($"Device '{deviceId}' already exists in IoT Hub '{iotHubHostName}' but is not an IoT Edge device. Delete it or choose another device id.");
        }
    }

    private async Task<Dictionary<string, string>> EnsureModules(
        string deviceId,
        IReadOnlyCollection<string> moduleIds,
        CancellationToken cancellationToken)
    {
        var existingModules = (await iotHubService.GetModulesOnIotEdgeDevice(deviceId, cancellationToken))
            .ToDictionary(x => x.Id, StringComparer.Ordinal);

        foreach (var moduleId in moduleIds)
        {
            if (existingModules.ContainsKey(moduleId))
            {
                continue;
            }

            var (succeeded, module) = await iotHubService.AddModuleToDevice(deviceId, moduleId, cancellationToken);
            if (!succeeded || module is null)
            {
                throw new InvalidOperationException($"Could not create module identity '{moduleId}' on IoT Edge device '{deviceId}'.");
            }

            existingModules[moduleId] = module;
            LogModuleCreated(iotHubHostName, deviceId, moduleId);
        }

        var moduleKeys = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var moduleId in moduleIds.Append(EdgeHubConstants.ModuleId))
        {
            if (!existingModules.TryGetValue(moduleId, out var module) ||
                module.Authentication?.SymmetricKey?.PrimaryKey is not { Length: > 0 } primaryKey)
            {
                throw new InvalidOperationException($"Module identity '{moduleId}' on IoT Edge device '{deviceId}' has no symmetric key.");
            }

            moduleKeys[moduleId] = primaryKey;
        }

        return moduleKeys;
    }

    private async Task ApplyModuleDesiredProperties(
        string deviceId,
        IEnumerable<string> provisionedModuleIds,
        DeploymentManifest deploymentManifest,
        CancellationToken cancellationToken)
    {
        foreach (var moduleId in provisionedModuleIds)
        {
            if (!deploymentManifest.ModuleDesiredProperties.TryGetValue(moduleId, out var desiredProperties))
            {
                continue;
            }

            var twin = await iotHubService.UpdateDesiredProperties(
                deviceId,
                moduleId,
                new TwinCollection(desiredProperties),
                cancellationToken);

            if (twin is null)
            {
                LogModuleDesiredPropertiesNotApplied(iotHubHostName, deviceId, moduleId);
            }
            else
            {
                LogModuleDesiredPropertiesApplied(iotHubHostName, deviceId, moduleId);
            }
        }
    }
}