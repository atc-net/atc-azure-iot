namespace Atc.Azure.IoTEdge.DeviceEmulator.Wrappers;

public class RegistryManagerWrapper : IRegistryManagerWrapper
{
    private readonly RegistryManager registryManager;
    private bool disposedValue; // To detect redundant calls

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryManagerWrapper"/> class.
    /// </summary>
    /// <param name="registryManager">The RegistryManager</param>
    public RegistryManagerWrapper(
        RegistryManager registryManager)
    {
        this.registryManager = registryManager;
    }

    public Task OpenAsync()
        => registryManager.OpenAsync();

    public Task CloseAsync()
        => registryManager.CloseAsync();

    public Task<Device> AddDeviceAsync(
        Device device)
        => registryManager.AddDeviceAsync(device);

    public Task<Device> AddDeviceAsync(
        Device device,
        CancellationToken cancellationToken)
        => registryManager.AddDeviceAsync(device, cancellationToken);

    public Task<Module> AddModuleAsync(
        Module module)
        => registryManager.AddModuleAsync(module);

    public Task<Module> AddModuleAsync(
        Module module,
        CancellationToken cancellationToken)
        => registryManager.AddModuleAsync(module, cancellationToken);

    public Task<BulkRegistryOperationResult> AddDeviceWithTwinAsync(
        Device device,
        Twin twin)
        => registryManager.AddDeviceWithTwinAsync(device, twin);

    public Task<BulkRegistryOperationResult> AddDeviceWithTwinAsync(
        Device device,
        Twin twin,
        CancellationToken cancellationToken)
        => registryManager.AddDeviceWithTwinAsync(device, twin, cancellationToken);

    public Task<BulkRegistryOperationResult> AddDevices2Async(
        IEnumerable<Device> devices)
        => registryManager.AddDevices2Async(devices);

    public Task<BulkRegistryOperationResult> AddDevices2Async(
        IEnumerable<Device> devices,
        CancellationToken cancellationToken)
        => registryManager.AddDevices2Async(devices, cancellationToken);

    public Task<Device> UpdateDeviceAsync(
        Device device)
        => registryManager.UpdateDeviceAsync(device);

    public Task<Device> UpdateDeviceAsync(
        Device device,
        bool forceUpdate)
        => registryManager.UpdateDeviceAsync(device, forceUpdate);

    public Task<Device> UpdateDeviceAsync(
        Device device,
        CancellationToken cancellationToken)
        => registryManager.UpdateDeviceAsync(device, cancellationToken);

    public Task<Device> UpdateDeviceAsync(
        Device device,
        bool forceUpdate,
        CancellationToken cancellationToken)
        => registryManager.UpdateDeviceAsync(device, forceUpdate, cancellationToken);

    public Task<Module> UpdateModuleAsync(
        Module module)
        => registryManager.UpdateModuleAsync(module);

    public Task<Module> UpdateModuleAsync(
        Module module,
        bool forceUpdate)
        => registryManager.UpdateModuleAsync(module, forceUpdate);

    public Task<Module> UpdateModuleAsync(
        Module module,
        CancellationToken cancellationToken)
        => registryManager.UpdateModuleAsync(module, cancellationToken);

    public Task<Module> UpdateModuleAsync(
        Module module,
        bool forceUpdate,
        CancellationToken cancellationToken)
        => registryManager.UpdateModuleAsync(module, forceUpdate, cancellationToken);

    public Task<BulkRegistryOperationResult> UpdateDevices2Async(
        IEnumerable<Device> devices)
        => registryManager.UpdateDevices2Async(devices);

    public Task<BulkRegistryOperationResult> UpdateDevices2Async(
        IEnumerable<Device> devices,
        bool forceUpdate,
        CancellationToken cancellationToken)
        => registryManager.UpdateDevices2Async(devices, forceUpdate, cancellationToken);

    public Task RemoveDeviceAsync(
        string deviceId)
        => registryManager.RemoveDeviceAsync(deviceId);

    public Task RemoveDeviceAsync(
        string deviceId,
        CancellationToken cancellationToken)
        => registryManager.RemoveDeviceAsync(deviceId, cancellationToken);

    public Task RemoveDeviceAsync(
        Device device)
        => registryManager.RemoveDeviceAsync(device);

    public Task RemoveDeviceAsync(
        Device device,
        CancellationToken cancellationToken)
        => registryManager.RemoveDeviceAsync(device, cancellationToken);

    public Task RemoveModuleAsync(
        string deviceId,
        string moduleId)
        => registryManager.RemoveModuleAsync(deviceId, moduleId);

    public Task RemoveModuleAsync(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken)
        => registryManager.RemoveModuleAsync(deviceId, moduleId, cancellationToken);

    public Task RemoveModuleAsync(
        Module module)
        => registryManager.RemoveModuleAsync(module);

    public Task RemoveModuleAsync(
        Module module,
        CancellationToken cancellationToken)
        => registryManager.RemoveModuleAsync(module, cancellationToken);

    public Task<BulkRegistryOperationResult> RemoveDevices2Async(
        IEnumerable<Device> devices)
        => registryManager.RemoveDevices2Async(devices);

    public Task<BulkRegistryOperationResult> RemoveDevices2Async(
        IEnumerable<Device> devices,
        bool forceRemove,
        CancellationToken cancellationToken)
        => registryManager.RemoveDevices2Async(devices, forceRemove, cancellationToken);

    public Task<RegistryStatistics> GetRegistryStatisticsAsync()
        => registryManager.GetRegistryStatisticsAsync();

    public Task<RegistryStatistics> GetRegistryStatisticsAsync(
        CancellationToken cancellationToken)
        => registryManager.GetRegistryStatisticsAsync(cancellationToken);

    public Task<Device?> GetDeviceAsync(
        string deviceId)
        => registryManager.GetDeviceAsync(deviceId);

    public Task<Device?> GetDeviceAsync(
        string deviceId,
        CancellationToken cancellationToken)
        => registryManager.GetDeviceAsync(deviceId, cancellationToken);

    public Task<Module> GetModuleAsync(
        string deviceId,
        string moduleId)
        => registryManager.GetModuleAsync(deviceId, moduleId);

    public Task<Module> GetModuleAsync(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken)
        => registryManager.GetModuleAsync(deviceId, moduleId, cancellationToken);

    public Task<IEnumerable<Module>> GetModulesOnDeviceAsync(
        string deviceId)
        => registryManager.GetModulesOnDeviceAsync(deviceId);

    public Task<IEnumerable<Module>> GetModulesOnDeviceAsync(
        string deviceId,
        CancellationToken cancellationToken)
        => registryManager.GetModulesOnDeviceAsync(deviceId, cancellationToken);

    public IQuery CreateQuery(
        string sqlQueryString)
        => registryManager.CreateQuery(sqlQueryString);

    public IQuery CreateQuery(
        string sqlQueryString,
        int? pageSize)
        => registryManager.CreateQuery(sqlQueryString, pageSize);

    public Task ExportRegistryAsync(
        string storageAccountConnectionString,
        string containerName)
        => registryManager.ExportRegistryAsync(storageAccountConnectionString, containerName);

    public Task ExportRegistryAsync(
        string storageAccountConnectionString,
        string containerName,
        CancellationToken cancellationToken)
        => registryManager.ExportRegistryAsync(storageAccountConnectionString, containerName, cancellationToken);

    public Task ImportRegistryAsync(
        string storageAccountConnectionString,
        string containerName)
        => registryManager.ImportRegistryAsync(storageAccountConnectionString, containerName);

    public Task ImportRegistryAsync(
        string storageAccountConnectionString,
        string containerName,
        CancellationToken cancellationToken)
        => registryManager.ImportRegistryAsync(storageAccountConnectionString, containerName, cancellationToken);

    public Task<JobProperties> ExportDevicesAsync(
        string exportBlobContainerUri,
        bool excludeKeys)
        => registryManager.ExportDevicesAsync(exportBlobContainerUri, excludeKeys);

    public Task<JobProperties> ExportDevicesAsync(
        string exportBlobContainerUri,
        bool excludeKeys,
        CancellationToken cancellationToken)
        => registryManager.ExportDevicesAsync(exportBlobContainerUri, excludeKeys, cancellationToken);

    public Task<JobProperties> ExportDevicesAsync(
        string exportBlobContainerUri,
        string outputBlobName,
        bool excludeKeys)
        => registryManager.ExportDevicesAsync(exportBlobContainerUri, outputBlobName, excludeKeys);

    public Task<JobProperties> ExportDevicesAsync(
        string exportBlobContainerUri,
        string outputBlobName,
        bool excludeKeys,
        CancellationToken cancellationToken)
        => registryManager.ExportDevicesAsync(exportBlobContainerUri, outputBlobName, excludeKeys, cancellationToken);

    public Task<JobProperties> ExportDevicesAsync(
        JobProperties jobParameters,
        CancellationToken cancellationToken = default)
        => registryManager.ExportDevicesAsync(jobParameters, cancellationToken);

    public Task<JobProperties> ImportDevicesAsync(
        string importBlobContainerUri,
        string outputBlobContainerUri)
        => registryManager.ImportDevicesAsync(importBlobContainerUri, outputBlobContainerUri);

    public Task<JobProperties> ImportDevicesAsync(
        string importBlobContainerUri,
        string outputBlobContainerUri,
        CancellationToken cancellationToken)
        => registryManager.ImportDevicesAsync(importBlobContainerUri, outputBlobContainerUri, cancellationToken);

    public Task<JobProperties> ImportDevicesAsync(
        string importBlobContainerUri,
        string outputBlobContainerUri,
        string inputBlobName)
        => registryManager.ImportDevicesAsync(importBlobContainerUri, outputBlobContainerUri, inputBlobName);

    public Task<JobProperties> ImportDevicesAsync(
        string importBlobContainerUri,
        string outputBlobContainerUri,
        string inputBlobName,
        CancellationToken cancellationToken)
        => registryManager.ImportDevicesAsync(importBlobContainerUri, outputBlobContainerUri, inputBlobName, cancellationToken);

    public Task<JobProperties> ImportDevicesAsync(
        JobProperties jobParameters,
        CancellationToken cancellationToken = default)
        => registryManager.ImportDevicesAsync(jobParameters, cancellationToken);

    public Task<JobProperties> GetJobAsync(
        string jobId)
        => registryManager.GetJobAsync(jobId);

    public Task<JobProperties> GetJobAsync(
        string jobId,
        CancellationToken cancellationToken)
        => registryManager.GetJobAsync(jobId, cancellationToken);

    public Task<IEnumerable<JobProperties>> GetJobsAsync()
        => registryManager.GetJobsAsync();

    public Task<IEnumerable<JobProperties>> GetJobsAsync(
        CancellationToken cancellationToken)
        => registryManager.GetJobsAsync(cancellationToken);

    public Task CancelJobAsync(
        string jobId)
        => registryManager.CancelJobAsync(jobId);

    public Task CancelJobAsync(
        string jobId,
        CancellationToken cancellationToken)
        => registryManager.CancelJobAsync(jobId, cancellationToken);

    public Task<Twin> GetTwinAsync(
        string deviceId)
        => registryManager.GetTwinAsync(deviceId);

    public Task<Twin> GetTwinAsync(
        string deviceId,
        CancellationToken cancellationToken)
        => registryManager.GetTwinAsync(deviceId, cancellationToken);

    public Task<Twin> GetTwinAsync(
        string deviceId,
        string moduleId)
        => registryManager.GetTwinAsync(deviceId, moduleId);

    public Task<Twin> GetTwinAsync(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken)
        => registryManager.GetTwinAsync(deviceId, moduleId, cancellationToken);

    public Task<Twin> UpdateTwinAsync(
        string deviceId,
        Twin twinPatch,
        string etag)
        => registryManager.UpdateTwinAsync(deviceId, twinPatch, etag);

    public Task<Twin> UpdateTwinAsync(
        string deviceId,
        Twin twinPatch,
        string etag,
        CancellationToken cancellationToken)
        => registryManager.UpdateTwinAsync(deviceId, twinPatch, etag, cancellationToken);

    public Task<Twin> UpdateTwinAsync(
        string deviceId,
        string jsonTwinPatch,
        string etag)
        => registryManager.UpdateTwinAsync(deviceId, jsonTwinPatch, etag);

    public Task<Twin> UpdateTwinAsync(
        string deviceId,
        string jsonTwinPatch,
        string etag,
        CancellationToken cancellationToken)
        => registryManager.UpdateTwinAsync(deviceId, jsonTwinPatch, etag, cancellationToken);

    public Task<Twin> UpdateTwinAsync(
        string deviceId,
        string moduleId,
        Twin twinPatch,
        string etag)
        => registryManager.UpdateTwinAsync(deviceId, moduleId, twinPatch, etag);

    public Task<Twin> UpdateTwinAsync(
        string deviceId,
        string moduleId,
        Twin twinPatch,
        string etag,
        CancellationToken cancellationToken)
        => registryManager.UpdateTwinAsync(deviceId, moduleId, twinPatch, etag, cancellationToken);

    public Task<Twin> UpdateTwinAsync(
        string deviceId,
        string moduleId,
        string jsonTwinPatch,
        string etag)
        => registryManager.UpdateTwinAsync(deviceId, moduleId, jsonTwinPatch, etag);

    public Task<Twin> UpdateTwinAsync(
        string deviceId,
        string moduleId,
        string jsonTwinPatch,
        string etag,
        CancellationToken cancellationToken)
        => registryManager.UpdateTwinAsync(deviceId, moduleId, jsonTwinPatch, etag, cancellationToken);

    public Task<BulkRegistryOperationResult> UpdateTwins2Async(
        IEnumerable<Twin> twins)
        => registryManager.UpdateTwins2Async(twins);

    public Task<BulkRegistryOperationResult> UpdateTwins2Async(
        IEnumerable<Twin> twins,
        CancellationToken cancellationToken)
        => registryManager.UpdateTwins2Async(twins, cancellationToken);

    public Task<BulkRegistryOperationResult> UpdateTwins2Async(
        IEnumerable<Twin> twins,
        bool forceUpdate)
        => registryManager.UpdateTwins2Async(twins, forceUpdate);

    public Task<BulkRegistryOperationResult> UpdateTwins2Async(
        IEnumerable<Twin> twins,
        bool forceUpdate,
        CancellationToken cancellationToken)
        => registryManager.UpdateTwins2Async(twins, forceUpdate, cancellationToken);

    public Task<Twin> ReplaceTwinAsync(
        string deviceId,
        Twin newTwin,
        string etag)
        => registryManager.ReplaceTwinAsync(deviceId, newTwin, etag);

    public Task<Twin> ReplaceTwinAsync(
        string deviceId,
        Twin newTwin,
        string etag,
        CancellationToken cancellationToken)
        => registryManager.ReplaceTwinAsync(deviceId, newTwin, etag, cancellationToken);

    public Task<Twin> ReplaceTwinAsync(
        string deviceId,
        string newTwinJson,
        string etag)
        => registryManager.ReplaceTwinAsync(deviceId, newTwinJson, etag);

    public Task<Twin> ReplaceTwinAsync(
        string deviceId,
        string newTwinJson,
        string etag,
        CancellationToken cancellationToken)
        => registryManager.ReplaceTwinAsync(deviceId, newTwinJson, etag, cancellationToken);

    public Task<Twin> ReplaceTwinAsync(
        string deviceId,
        string moduleId,
        Twin newTwin,
        string etag)
        => registryManager.ReplaceTwinAsync(deviceId, moduleId, newTwin, etag);

    public Task<Twin> ReplaceTwinAsync(
        string deviceId,
        string moduleId,
        Twin newTwin,
        string etag,
        CancellationToken cancellationToken)
        => registryManager.ReplaceTwinAsync(deviceId, moduleId, newTwin, etag, cancellationToken);

    public Task<Twin> ReplaceTwinAsync(
        string deviceId,
        string moduleId,
        string newTwinJson,
        string etag)
        => registryManager.ReplaceTwinAsync(deviceId, moduleId, newTwinJson, etag);

    public Task<Twin> ReplaceTwinAsync(
        string deviceId,
        string moduleId,
        string newTwinJson,
        string etag,
        CancellationToken cancellationToken)
        => registryManager.ReplaceTwinAsync(deviceId, moduleId, newTwinJson, etag, cancellationToken);

    public Task<Configuration> AddConfigurationAsync(
        Configuration configuration)
        => registryManager.AddConfigurationAsync(configuration);

    public Task<Configuration> AddConfigurationAsync(
        Configuration configuration,
        CancellationToken cancellationToken)
        => registryManager.AddConfigurationAsync(configuration, cancellationToken);

    public Task<Configuration> GetConfigurationAsync(
        string configurationId)
        => registryManager.GetConfigurationAsync(configurationId);

    public Task<Configuration> GetConfigurationAsync(
        string configurationId,
        CancellationToken cancellationToken)
        => registryManager.GetConfigurationAsync(configurationId, cancellationToken);

    public Task<IEnumerable<Configuration>> GetConfigurationsAsync(
        int maxCount)
        => registryManager.GetConfigurationsAsync(maxCount);

    public Task<IEnumerable<Configuration>> GetConfigurationsAsync(
        int maxCount,
        CancellationToken cancellationToken)
        => registryManager.GetConfigurationsAsync(maxCount, cancellationToken);

    public Task<Configuration> UpdateConfigurationAsync(
        Configuration configuration)
        => registryManager.UpdateConfigurationAsync(configuration);

    public Task<Configuration> UpdateConfigurationAsync(
        Configuration configuration,
        bool forceUpdate)
        => registryManager.UpdateConfigurationAsync(configuration, forceUpdate);

    public Task<Configuration> UpdateConfigurationAsync(
        Configuration configuration,
        CancellationToken cancellationToken)
        => registryManager.UpdateConfigurationAsync(configuration, cancellationToken);

    public Task<Configuration> UpdateConfigurationAsync(
        Configuration configuration,
        bool forceUpdate,
        CancellationToken cancellationToken)
        => registryManager.UpdateConfigurationAsync(configuration, forceUpdate, cancellationToken);

    public Task RemoveConfigurationAsync(
        string configurationId)
        => registryManager.RemoveConfigurationAsync(configurationId);

    public Task RemoveConfigurationAsync(
        string configurationId,
        CancellationToken cancellationToken)
        => registryManager.RemoveConfigurationAsync(configurationId, cancellationToken);

    public Task RemoveConfigurationAsync(
        Configuration configuration)
        => registryManager.RemoveConfigurationAsync(configuration);

    public Task RemoveConfigurationAsync(
        Configuration configuration,
        CancellationToken cancellationToken)
        => registryManager.RemoveConfigurationAsync(configuration, cancellationToken);

    public Task ApplyConfigurationContentOnDeviceAsync(
        string deviceId,
        ConfigurationContent content)
        => registryManager.ApplyConfigurationContentOnDeviceAsync(deviceId, content);

    public Task ApplyConfigurationContentOnDeviceAsync(
        string deviceId,
        ConfigurationContent content,
        CancellationToken cancellationToken)
        => registryManager.ApplyConfigurationContentOnDeviceAsync(deviceId, content, cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.registryManager.Dispose();
            }

            disposedValue = true;
        }
    }
}