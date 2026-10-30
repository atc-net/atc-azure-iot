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
        IIoTHubModuleService iotHubModuleService,
        IotHubOptions options,
        ILoggerFactory? loggerFactory = null)
    {
        logger = loggerFactory is not null
            ? loggerFactory.CreateLogger<IoTHubService>()
            : NullLogger<IoTHubService>.Instance;

        this.iotHubModuleService = iotHubModuleService;
        ValidateAndAssign(options.ConnectionString, Assign);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IoTHubService"/> class with a given registry manager, for tests.
    /// </summary>
    /// <param name="iotHubModuleService">The service that invokes direct methods.</param>
    /// <param name="registryManager">The registry manager to use.</param>
    /// <param name="iotHubHostName">The IoT Hub host name, used in connection strings and logs.</param>
    /// <param name="loggerFactory">The logger factory, or <see langword="null"/> for no logging.</param>
    internal IoTHubService(
        IIoTHubModuleService iotHubModuleService,
        RegistryManager registryManager,
        string iotHubHostName,
        ILoggerFactory? loggerFactory = null)
    {
        logger = loggerFactory is not null
            ? loggerFactory.CreateLogger<IoTHubService>()
            : NullLogger<IoTHubService>.Instance;

        this.iotHubModuleService = iotHubModuleService;
        this.registryManager = registryManager;
        ioTHubHostName = iotHubHostName;
    }

    public Task<RegistryStatistics> GetDeviceRegistryStatistics(
        CancellationToken cancellationToken = default)
    {
        LogRetrievingRegistryStatistics(ioTHubHostName!);

        return registryManager!.GetRegistryStatisticsAsync(cancellationToken)
            .NormalizeCancellation(cancellationToken);
    }

    public Task<Device> CreateDevice(
        string deviceId,
        bool edgeEnabled,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        return registryManager!.AddDeviceAsync(
            new Device(deviceId)
            {
                Capabilities = new DeviceCapabilities
                {
                    IotEdge = edgeEnabled,
                },
            },
            cancellationToken)
            .NormalizeCancellation(cancellationToken);
    }

    public async Task<Device?> GetDevice(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        LogRetrievingDevice(
            ioTHubHostName!,
            deviceId);

        var device = await registryManager!.GetDeviceAsync(
            deviceId,
            cancellationToken)
            .NormalizeCancellation(cancellationToken);

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

    public async Task<string?> GetDeviceConnectionString(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        LogRetrievingDeviceConnectionString(
            ioTHubHostName!,
            deviceId);

        var device = await registryManager!.GetDeviceAsync(
            deviceId,
            cancellationToken)
            .NormalizeCancellation(cancellationToken);

        if (device is null)
        {
            LogIotEdgeDeviceNotFound(
                ioTHubHostName!,
                deviceId);

            return null;
        }

        var primaryKey = GetPrimaryKey(
            device.Authentication,
            $"Device '{deviceId}'");

        LogRetrieveIotEdgeDeviceConnectionStringSucceeded(
            ioTHubHostName!,
            deviceId);

        return $"HostName={ioTHubHostName!};DeviceId={deviceId};SharedAccessKey={primaryKey}";
    }

    public async Task<string?> GetModuleConnectionString(
        string deviceId,
        string moduleId,
        string? gatewayHostName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);

        LogRetrievingModuleConnectionString(
            ioTHubHostName!,
            deviceId,
            moduleId);

        var module = await registryManager!.GetModuleAsync(
            deviceId,
            moduleId,
            cancellationToken)
            .NormalizeCancellation(cancellationToken);

        if (module is null)
        {
            LogIotEdgeDeviceModuleNotFound(
                ioTHubHostName!,
                deviceId,
                moduleId);

            return null;
        }

        var primaryKey = GetPrimaryKey(
            module.Authentication,
            $"Module '{moduleId}' on device '{deviceId}'");

        LogRetrieveModuleConnectionStringSucceeded(
            ioTHubHostName!,
            deviceId,
            moduleId);

        return string.IsNullOrEmpty(gatewayHostName)
            ? $"HostName={ioTHubHostName!};DeviceId={deviceId};ModuleId={moduleId};SharedAccessKey={primaryKey}"
            : $"HostName={ioTHubHostName!};GatewayHostName={gatewayHostName};DeviceId={deviceId};ModuleId={moduleId};SharedAccessKey={primaryKey}";
    }

    public async Task<IReadOnlyCollection<Twin>> GetDeviceTwins(
        bool onlyIncludeEdgeDevices,
        CancellationToken cancellationToken = default)
    {
        LogRetrievingIotDeviceTwins(ioTHubHostName!);

        var queryString = onlyIncludeEdgeDevices
            ? $"{QueryPrefix} WHERE capabilities.iotEdge = true"
            : QueryPrefix;

        var query = registryManager!.CreateQuery(
            queryString,
            MaxPageSize);

        var result = new List<Twin>();
        while (query.HasMoreResults)
        {
            // The SDK's query takes no cancellation token, so the token is checked between pages.
            cancellationToken.ThrowIfCancellationRequested();

            var page = await query.GetNextAsTwinAsync();
            result.AddRange(page);
        }

        LogRetrieveIotDeviceTwinsSucceeded(
            ioTHubHostName!,
            result.Count);

        return result.AsReadOnly();
    }

    public async Task<Twin?> GetDeviceTwin(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        LogRetrievingDeviceTwin(
            ioTHubHostName!,
            deviceId);

        var deviceTwin = await registryManager!.GetTwinAsync(
            deviceId,
            cancellationToken)
            .NormalizeCancellation(cancellationToken);

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

    public async Task<Twin> UpdateDeviceTwin(
        string deviceId,
        Twin twin,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentNullException.ThrowIfNull(twin);

        LogUpdatingDeviceTwin(
            ioTHubHostName!,
            deviceId);

        var updatedTwin = await registryManager!.UpdateTwinAsync(
            deviceId,
            twin,
            twin.ETag,
            cancellationToken)
            .NormalizeCancellation(cancellationToken);

        LogUpdateIotDeviceTwinSucceeded(
            ioTHubHostName!,
            deviceId);

        return updatedTwin;
    }

    public async Task<IReadOnlyCollection<Microsoft.Azure.Devices.Module>> GetModulesOnIotEdgeDevice(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);

        LogRetrievingIotEdgeDeviceModules(
            ioTHubHostName!,
            deviceId);

        var modulesOnDevice = await registryManager!.GetModulesOnDeviceAsync(
            deviceId,
            cancellationToken)
            .NormalizeCancellation(cancellationToken);

        var modules = modulesOnDevice?.ToList() ?? [];
        if (modules.Count == 0)
        {
            LogIotEdgeDeviceModulesNotFound(
                ioTHubHostName!,
                deviceId);

            return modules;
        }

        LogRetrieveIotEdgeDeviceModulesSucceeded(
            ioTHubHostName!,
            deviceId);

        return modules;
    }

    public async Task<Twin> UpdateDesiredProperties(
        string deviceId,
        string moduleId,
        TwinCollection twinCollection,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);
        ArgumentNullException.ThrowIfNull(twinCollection);

        LogUpdatingDeviceTwinDesiredProperties(
            ioTHubHostName!,
            deviceId,
            moduleId);

        // A twin update is a merge (PATCH): only the given desired properties change, and a property set to null is removed.
        // The wildcard ETag applies the patch whatever the twin's current version is.
        var patch = new Twin
        {
            Properties = new TwinProperties
            {
                Desired = twinCollection,
            },
        };

        var updatedTwin = await registryManager!.UpdateTwinAsync(
            deviceId,
            moduleId,
            patch,
            "*",
            cancellationToken)
            .NormalizeCancellation(cancellationToken);

        LogUpdateModuleTwinDesiredPropertiesSucceeded(
            ioTHubHostName!,
            deviceId,
            moduleId);

        return updatedTwin;
    }

    public async Task<Twin?> GetModuleTwin(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);

        LogRetrievingIotEdgeDeviceTwinModule(
            ioTHubHostName!,
            deviceId,
            moduleId);

        var moduleTwin = await registryManager!.GetTwinAsync(
            deviceId,
            moduleId,
            cancellationToken)
            .NormalizeCancellation(cancellationToken);

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

    public Task<Microsoft.Azure.Devices.Module> AddModuleToDevice(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);

        return registryManager!.AddModuleAsync(
            new Microsoft.Azure.Devices.Module(deviceId, moduleId),
            cancellationToken)
            .NormalizeCancellation(cancellationToken);
    }

    public Task<Microsoft.Azure.Devices.Module> UpdateModule(
        Microsoft.Azure.Devices.Module moduleIdentity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(moduleIdentity);

        return registryManager!.UpdateModuleAsync(
            moduleIdentity,
            cancellationToken)
            .NormalizeCancellation(cancellationToken);
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
                cancellationToken)
                .NormalizeCancellation(cancellationToken);
        }
        catch (Exception ex) when (ex is DeviceNotFoundException or ModuleNotFoundException)
        {
            // IoT Hub reports a module that doesn't exist as DeviceNotFoundException.
            LogIotDeviceModuleNotFoundOnRemove(
                ioTHubHostName!,
                deviceId,
                moduleId);

            return false;
        }

        LogIotDeviceModuleRemoved(
            ioTHubHostName!,
            deviceId,
            moduleId);

        return true;
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
            requestOptions: null,
            cancellationToken);

        return result.Status == StatusCodes.Status200OK
            ? (Succeeded: true, result.Status, $"{{\"message\":\"Successfully restarted module '{moduleId}' on device '{deviceId}'.\"}}")
            : (Succeeded: false, result.Status, result.JsonPayload);
    }

    /// <inheritdoc />
    public Task<Response<LogResponse>> UploadModuleLogs(
        string deviceId,
        UploadModuleLogsRequest request,
        IoTHubRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
        => CallLogMethod(
            deviceId,
            EdgeAgentConstants.DirectMethodUploadModuleLogs,
            request,
            requestOptions,
            cancellationToken);

    /// <inheritdoc />
    public Task<Response<LogResponse>> UploadSupportBundle(
        string deviceId,
        UploadSupportBundleRequest request,
        IoTHubRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
        => CallLogMethod(
            deviceId,
            EdgeAgentConstants.DirectMethodUploadSupportBundle,
            request,
            requestOptions,
            cancellationToken);

    /// <inheritdoc />
    public Task<Response<LogResponse>> GetTaskStatus(
        string deviceId,
        GetTaskStatusRequest request,
        IoTHubRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
        => CallLogMethod(
            deviceId,
            EdgeAgentConstants.DirectMethodGetTaskStatus,
            request,
            requestOptions,
            cancellationToken);

    public async Task ApplyConfigurationContentOnDevice(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentNullException.ThrowIfNull(manifestContent);

        await registryManager!.ApplyConfigurationContentOnDeviceAsync(
            deviceId,
            manifestContent,
            cancellationToken)
            .NormalizeCancellation(cancellationToken);

        LogAppliedConfigurationContent(
            ioTHubHostName!,
            deviceId);
    }

    public async Task AddNewModules(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentNullException.ThrowIfNull(manifestContent);

        foreach (var module in manifestContent.ModulesContent.Keys)
        {
            if (!module.StartsWith('$'))
            {
                await registryManager!.AddModuleAsync(
                    new Microsoft.Azure.Devices.Module(deviceId, module),
                    cancellationToken)
                    .NormalizeCancellation(cancellationToken);
            }
        }

        LogAddedModulesToIotEdgeDevice(
            ioTHubHostName!,
            deviceId);
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
                cancellationToken)
                .NormalizeCancellation(cancellationToken);
        }
        catch (DeviceNotFoundException)
        {
            LogIotDeviceNotFoundOnDelete(
                ioTHubHostName!,
                deviceId);

            return false;
        }

        LogIotDeviceDeleted(
            ioTHubHostName!,
            deviceId);

        return true;
    }

    protected override void Assign(string connectionString)
    {
        registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        ioTHubHostName = IotHubConnectionStringBuilder.Create(connectionString).HostName;
    }

    private async Task<Response<LogResponse>> CallLogMethod(
        string deviceId,
        string methodName,
        object request,
        IoTHubRequestOptions? requestOptions,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentNullException.ThrowIfNull(request);

        var serializerOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };

        MethodResultModel result;

        try
        {
            result = await iotHubModuleService.CallMethod(
                deviceId,
                EdgeAgentConstants.ModuleId,
                new MethodParameterModel(
                    methodName,
                    JsonSerializer.Serialize(request, serializerOptions)),
                requestOptions,
                cancellationToken);
        }
        catch (IotHubException ex)
        {
            return new Response<LogResponse>(
                ex.Code.ToHttpStatusCode(),
                new LogResponse(
                    BackgroundTaskRunStatus.Unknown,
                    ex.Message,
                    ex.TrackingId,
                    ex.Code.ToString()));
        }

        var payload = string.IsNullOrEmpty(result.JsonPayload)
            ? new LogResponse(BackgroundTaskRunStatus.Unknown, string.Empty, string.Empty, ErrorCode: null)
            : JsonSerializer.Deserialize<LogResponse>(result.JsonPayload, serializerOptions);

        return new Response<LogResponse>(result.Status, payload!);
    }

    /// <summary>
    /// Gets the primary symmetric key of a device or module identity.
    /// </summary>
    /// <exception cref="InvalidOperationException">The identity authenticates with X.509 certificates, so it has no key.</exception>
    private static string GetPrimaryKey(
        AuthenticationMechanism? authentication,
        string identityDescription)
        => authentication?.SymmetricKey?.PrimaryKey
           ?? throw new InvalidOperationException(
               $"{identityDescription} has no symmetric key, because it doesn't use shared access key authentication, so it has no connection string with a key.");

    private static RestartModuleRequest BuildRestartModuleRequest(string moduleId)
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

    private void Dispose(bool disposing)
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