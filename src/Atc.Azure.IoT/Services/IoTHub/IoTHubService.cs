// ReSharper disable GCSuppressFinalizeForTypeWithoutDestructor
namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// Represents a service for interacting with Azure IoT Hub, providing functionality to manage devices and modules,
/// including creating, updating, and deleting devices or modules, retrieving device or module twins, applying configurations,
/// and invoking direct methods. It encapsulates operations such as device registry statistics retrieval, device twin
/// management, module twin updates, and edge device configurations, aiming to facilitate comprehensive IoT Hub management.
/// </summary>
public sealed partial class IoTHubService : ServiceBase, IIoTHubService, IDisposable
{
    private const int MaxPageSize = 100;
    private const string QueryPrefix = "SELECT * FROM devices";

    private readonly IIoTHubModuleService iotHubModuleService;
    private RegistryManager? registryManager;
    private string? ioTHubHostName;

    public IoTHubService(
        ILoggerFactory loggerFactory,
        IIoTHubModuleService iotHubModuleService,
        IotHubOptions options)
    {
        logger = loggerFactory.CreateLogger<IoTHubService>();
        this.iotHubModuleService = iotHubModuleService;
        ValidateAndAssign(options.ConnectionString, Assign);
    }

    public async Task<RegistryStatistics?> GetDeviceRegistryStatistics(
        CancellationToken cancellationToken = default)
    {
        try
        {
            LogRetrievingRegistryStatistics(ioTHubHostName!);
            return await registryManager!.GetRegistryStatisticsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return null;
        }
    }

    public async Task<(bool Succeeded, Device? Device)> CreateDevice(
        string deviceId,
        bool edgeEnabled,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        try
        {
            var device = await registryManager!.AddDeviceAsync(
                new Device(deviceId)
                {
                    Capabilities = new DeviceCapabilities
                    {
                        IotEdge = edgeEnabled,
                    },
                },
                cancellationToken);

            return (true, device);
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return (false, null);
        }
    }

    public async Task<Device?> GetDevice(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        try
        {
            LogRetrievingDevice(
                ioTHubHostName!,
                deviceId);

            var device = await registryManager!.GetDeviceAsync(
                deviceId,
                cancellationToken);

            if (device is null)
            {
                LogIotEdgeDeviceNotFound(
                    ioTHubHostName!,
                    deviceId);

                return null;
            }

            LogRetrieveIotDeviceSucceeded(
                ioTHubHostName!,
                deviceId);

            return device;
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return null;
        }
    }

    public async Task<string?> GetDeviceConnectionString(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        try
        {
            LogRetrievingDeviceConnectionString(
                ioTHubHostName!,
                deviceId);

            var device = await registryManager!.GetDeviceAsync(
                deviceId,
                cancellationToken);

            if (device is null)
            {
                LogIotEdgeDeviceNotFound(
                    ioTHubHostName!,
                    deviceId);

                return null;
            }

            LogRetrieveIotEdgeDeviceConnectionStringSucceeded(
                ioTHubHostName!,
                deviceId);

            return $"HostName={ioTHubHostName!};DeviceId={deviceId};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return null;
        }
    }

    public async Task<IReadOnlyCollection<Twin>> GetDeviceTwins(
        bool onlyIncludeEdgeDevices)
    {
        var result = new List<Twin>();

        try
        {
            LogRetrievingIotDeviceTwins(ioTHubHostName!);

            var queryString = onlyIncludeEdgeDevices
                ? $"{QueryPrefix} WHERE capabilities.iotEdge = true"
                : QueryPrefix;

            var query = registryManager!.CreateQuery(
                queryString,
                MaxPageSize);

            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsTwinAsync();
                result.AddRange(page);
            }

            LogRetrieveIotDeviceTwinsSucceeded(
                ioTHubHostName!,
                result.Count);
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());
        }

        return result.AsReadOnly();
    }

    public async Task<Twin?> GetDeviceTwin(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        try
        {
            LogRetrievingDeviceTwin(
                ioTHubHostName!,
                deviceId);

            var deviceTwin = await registryManager!.GetTwinAsync(deviceId, cancellationToken);
            if (deviceTwin is null)
            {
                LogIotDeviceTwinNotFound(
                    ioTHubHostName!,
                    deviceId);

                return null;
            }

            LogRetrieveIotDeviceTwinSucceeded(
                ioTHubHostName!,
                deviceId);

            return deviceTwin;
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return null;
        }
    }

    public async Task<(bool Succeeded, Twin? UpdatedTwin)> UpdateDeviceTwin(
        string deviceId,
        Twin twin,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentNullException.ThrowIfNull(twin);

        try
        {
            LogUpdatingDeviceTwin(
                ioTHubHostName!,
                deviceId);

            var updatedTwin = await registryManager!.UpdateTwinAsync(
                deviceId,
                twin,
                twin.ETag,
                cancellationToken);

            if (updatedTwin is null)
            {
                LogIotDeviceTwinNotUpdated(
                    ioTHubHostName!,
                    deviceId);

                return (false, null);
            }

            LogUpdateIotDeviceTwinSucceeded(
                ioTHubHostName!,
                deviceId);

            return (true, updatedTwin);
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return (false, null);
        }
    }

    public async Task<IEnumerable<Microsoft.Azure.Devices.Module>> GetModulesOnIotEdgeDevice(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        try
        {
            LogRetrievingIotEdgeDeviceModules(
                ioTHubHostName!,
                deviceId);

            var modulesOnDevice = await registryManager!.GetModulesOnDeviceAsync(
                deviceId,
                cancellationToken);

            if (modulesOnDevice is null ||
                (modulesOnDevice.TryGetNonEnumeratedCount(out var modulesOnDeviceCount) &&
                 modulesOnDeviceCount == 0))
            {
                LogIotEdgeDeviceModulesNotFound(
                    ioTHubHostName!,
                    deviceId);

                return [];
            }

            LogRetrieveIotEdgeDeviceModulesSucceeded(
                ioTHubHostName!,
                deviceId);

            return modulesOnDevice;
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return [];
        }
    }

    public async Task<Twin?> UpdateDesiredProperties(
        string deviceId,
        string moduleId,
        TwinCollection twinCollection,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);

        try
        {
            LogRetrievingIotEdgeDeviceTwinModule(
                ioTHubHostName!,
                deviceId,
                moduleId);

            var moduleTwin = await registryManager!.GetTwinAsync(
                deviceId,
                moduleId,
                cancellationToken);

            if (moduleTwin is null)
            {
                LogIotEdgeDeviceTwinModuleNotFound(
                    ioTHubHostName!,
                    deviceId,
                    moduleId);

                return null;
            }

            moduleTwin.Properties.Desired = twinCollection;

            LogUpdatingDeviceTwinDesiredProperties(
                ioTHubHostName!,
                deviceId,
                moduleId);

            var updatedTwin = await registryManager.UpdateTwinAsync(
                deviceId,
                moduleId,
                moduleTwin,
                moduleTwin.ETag,
                cancellationToken);

            if (updatedTwin is null)
            {
                LogModuleTwinDesiredPropertiesNotUpdated(
                    ioTHubHostName!,
                    deviceId,
                    moduleId);

                return null;
            }

            LogUpdateModuleTwinDesiredPropertiesSucceeded(
                ioTHubHostName!,
                deviceId,
                moduleId);

            return updatedTwin;
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return null;
        }
    }

    public async Task<Twin?> GetModuleTwin(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);

        try
        {
            LogRetrievingIotEdgeDeviceTwinModule(
                ioTHubHostName!,
                deviceId,
                moduleId);

            var moduleTwin = await registryManager!.GetTwinAsync(
                deviceId,
                moduleId,
                cancellationToken);

            if (moduleTwin is null)
            {
                LogIotEdgeDeviceTwinModuleNotFound(
                    ioTHubHostName!,
                    deviceId,
                    moduleId);

                return null;
            }

            LogRetrieveIotEdgeDeviceTwinModuleSucceeded(
                ioTHubHostName!,
                deviceId,
                moduleId);

            return moduleTwin;
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return null;
        }
    }

    public async Task<bool> RemoveModuleFromDevice(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);

        try
        {
            await registryManager!.RemoveModuleAsync(
                deviceId,
                moduleId,
                cancellationToken);

            LogIotDeviceModuleRemoved(
                ioTHubHostName!,
                deviceId,
                moduleId);

            return true;
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return false;
        }
    }

    public async Task<(bool Succeeded, int StatusCode, string JsonPayload)> RestartModuleOnDevice(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);

        var request = BuildRestartModuleRequest(moduleId);
        var param = GetMethodParameterModel(EdgeAgentConstants.DirectMethodRestartModule, request);
        var result = await iotHubModuleService.CallMethod(
            deviceId,
            EdgeAgentConstants.ModuleId,
            param,
            cancellationToken);

        return result.Status == StatusCodes.Status200OK
            ? (Succeeded: true, result.Status, $"{{\"message\":\"Successfully restarted module '{moduleId}' on device '{deviceId}'.\"}}")
            : (Succeeded: false, result.Status, result.JsonPayload);
    }

    /// <inheritdoc />
    public Task<Response<LogResponse>> UploadSupportBundle(
        string deviceId,
        UploadSupportBundleRequest request,
        CancellationToken cancellationToken = default)
        => CallLogMethod(
            deviceId,
            EdgeAgentConstants.DirectMethodUploadSupportBundle,
            request,
            cancellationToken);

    /// <inheritdoc />
    public Task<Response<LogResponse>> GetTaskStatus(
        string deviceId,
        GetTaskStatusRequest request,
        CancellationToken cancellationToken = default)
        => CallLogMethod(
            deviceId,
            EdgeAgentConstants.DirectMethodGetTaskStatus,
            request,
            cancellationToken);

    public async Task<(bool Succeeded, string? ErrorMessage)> ApplyConfigurationContentOnDevice(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        try
        {
            await registryManager!.ApplyConfigurationContentOnDeviceAsync(
                deviceId,
                manifestContent,
                cancellationToken);

            LogAppliedConfigurationContent(
                ioTHubHostName!,
                deviceId);
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return (false, ex.GetLastInnerMessage());
        }

        return (true, null);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> AddNewModules(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var module in manifestContent.ModulesContent.Keys)
            {
                if (!module.StartsWith('$'))
                {
                    await registryManager!.AddModuleAsync(
                        new Microsoft.Azure.Devices.Module(deviceId, module),
                        cancellationToken);
                }
            }

            LogAddedModulesToIotEdgeDevice(
                ioTHubHostName!,
                deviceId);
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return (false, ex.GetLastInnerMessage());
        }

        return (true, null);
    }

    public async Task<bool> DeleteDevice(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        try
        {
            await registryManager!.RemoveDeviceAsync(
                deviceId,
                cancellationToken);

            LogIotDeviceDeleted(
                ioTHubHostName!,
                deviceId);

            return true;
        }
        catch (Exception ex)
        {
            LogFailure(
                ioTHubHostName!,
                ex.GetType().ToString(),
                ex.GetLastInnerMessage());

            return false;
        }
    }

    protected override void Assign(
        string connectionString)
    {
        registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        ioTHubHostName = IotHubConnectionStringBuilder.Create(connectionString).HostName;
    }

    private async Task<Response<LogResponse>> CallLogMethod(
        string deviceId,
        string methodName,
        object request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentNullException.ThrowIfNull(request);

        var result = await iotHubModuleService.CallMethod(
            deviceId,
            EdgeAgentConstants.ModuleId,
            new MethodParameterModel(
                methodName,
                JsonSerializer.Serialize(request)),
            cancellationToken);

        var payload = string.IsNullOrEmpty(result.JsonPayload)
            ? new LogResponse(BackgroundTaskRunStatus.Unknown, string.Empty, string.Empty)
            : JsonSerializer.Deserialize<LogResponse>(
                result.JsonPayload,
                options: new() { Converters = { new JsonStringEnumConverter() } });

        return new Response<LogResponse>(result.Status, payload!);
    }

    private static RestartModuleRequest BuildRestartModuleRequest(
        string moduleId)
        => new(Id: moduleId);

    private static MethodParameterModel GetMethodParameterModel(
        string directMethodName,
        object request)
        => new(
            Name: directMethodName,
            JsonPayload: JsonSerializer.Serialize(request));

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(
        bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (registryManager is null)
        {
            return;
        }

        registryManager.Dispose();
        registryManager = null;
    }
}