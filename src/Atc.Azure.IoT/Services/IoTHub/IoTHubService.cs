// ReSharper disable GCSuppressFinalizeForTypeWithoutDestructor
namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// The main IoTHubService - Handles call execution.
/// </summary>
public sealed partial class IoTHubService : IotHubServiceBase, IIoTHubService, IDisposable
{
    private const int MaxPageSize = 100;
    private const string QueryPrefix = "SELECT * FROM devices";

    private readonly IIoTHubModuleService iotHubModuleService;
    private RegistryManager? registryManager;
    private string? ioTHubHostName;

    public IoTHubService(
        ILogger<IoTHubService> logger,
        IIoTHubModuleService iotHubModuleService,
        IotHubOptions options)
    {
        this.logger = logger;
        this.iotHubModuleService = iotHubModuleService;

        ValidateAndAssign(options.ConnectionString, Assign);
    }

    public async Task<RegistryStatistics?> GetDeviceRegistryStatistics(
        CancellationToken cancellationToken = default)
    {
        try
        {
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

    public async Task<IReadOnlyCollection<Twin>> GetIoTEdgeDeviceTwins()
    {
        var result = new List<Twin>();

        try
        {
            LogRetrievingIotEdgeDeviceTwins(ioTHubHostName!);

            var query = registryManager!.CreateQuery(
                $"{QueryPrefix} WHERE capabilities.iotEdge = true",
                MaxPageSize);

            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsTwinAsync();
                result.AddRange(page);
            }

            LogRetrieveIotEdgeDeviceTwinsSucceeded(
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

    public async Task<(bool IsSuccessful, Twin? UpdatedTwin)> UpdateTwin(
        string deviceId,
        Twin twin,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

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

    public async Task<IEnumerable<Microsoft.Azure.Devices.Module>> GetModuleTwinsOnIotEdgeDevice(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        try
        {
            LogRetrievingIotEdgeDeviceTwinModules(
                ioTHubHostName!,
                deviceId);

            var modulesOnDevice = await registryManager!.GetModulesOnDeviceAsync(
                deviceId,
                cancellationToken);

            if (modulesOnDevice is null ||
                (modulesOnDevice.TryGetNonEnumeratedCount(out var modulesOnDeviceCount) &&
                 modulesOnDeviceCount == 0))
            {
                LogIotEdgeDeviceTwinModulesNotFound(
                    ioTHubHostName!,
                    deviceId);

                return Enumerable.Empty<Microsoft.Azure.Devices.Module>();
            }

            LogRetrieveIotEdgeDeviceTwinModulesSucceeded(
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

            return Enumerable.Empty<Microsoft.Azure.Devices.Module>();
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
        CancellationToken cancellationToken)
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

    public async Task<(bool Succeeded, string? ErrorMessage)> ApplyConfigurationContentOnDevice(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken)
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
        CancellationToken cancellationToken)
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
        string iotHubConnectionString)
    {
        registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
        ioTHubHostName = IotHubConnectionStringBuilder.Create(iotHubConnectionString).HostName;
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