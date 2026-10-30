namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// Represents a service for interacting with Azure IoT Hub, providing functionality to manage devices and modules,
/// including creating, updating, and deleting devices or modules, retrieving device or module twins, applying configurations,
/// and invoking direct methods. It encapsulates operations such as device registry statistics retrieval, device twin
/// management, module twin updates, and edge device configurations, aiming to facilitate comprehensive IoT Hub management.
/// </summary>
/// <remarks>
/// <para>
/// A method that looks something up returns <see langword="null"/> (or <see langword="false"/> for a delete) only when
/// the device, module or twin doesn't exist. Every other failure is thrown as the IoT Hub SDK's exception, for example
/// <see cref="UnauthorizedException"/> for a wrong key or <see cref="ThrottlingException"/> when IoT Hub throttles.
/// </para>
/// <para>
/// A cancelled <see cref="CancellationToken"/> throws <see cref="OperationCanceledException"/>. The IoT Hub SDK reports
/// a cancelled request as an <see cref="IotHubException"/>, which becomes the <see cref="Exception.InnerException"/>.
/// </para>
/// <para>
/// The direct methods on edgeAgent (<see cref="UploadModuleLogs"/>, <see cref="UploadSupportBundle"/> and
/// <see cref="GetTaskStatus"/>) are the exception: they return an IoT Hub error, such as an offline device, as the
/// response's status code. A cancelled <see cref="CancellationToken"/> still throws <see cref="OperationCanceledException"/>.
/// </para>
/// </remarks>
public interface IIoTHubService
{
    /// <summary>
    /// Asynchronously retrieves the statistics of the device registry in the IoT Hub.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The registry statistics of the IoT Hub.</returns>
    Task<RegistryStatistics> GetDeviceRegistryStatistics(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously creates a new device in the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The identifier for the new device to create.</param>
    /// <param name="edgeEnabled">Specifies whether the device is edge enabled.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The created device, including its keys.</returns>
    /// <exception cref="DeviceAlreadyExistsException">A device with the identifier already exists.</exception>
    Task<Device> CreateDevice(
        string deviceId,
        bool edgeEnabled,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a specific device from the IoT Hub using its deviceId.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to retrieve.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The device, or <see langword="null"/> if it doesn't exist.</returns>
    Task<Device?> GetDevice(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a specific device connection-string from the IoT Hub using its deviceId.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to retrieve.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The device connection-string with its primary key, or <see langword="null"/> if the device doesn't exist.</returns>
    Task<string?> GetDeviceConnectionString(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves all device twins in the IoT Hub.
    /// </summary>
    /// <param name="onlyIncludeEdgeDevices">Indicates if only edge device twins should be returned.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. It is checked between pages.</param>
    /// <returns>All device twins; empty if there are none.</returns>
    Task<IReadOnlyCollection<Twin>> GetDeviceTwins(
        bool onlyIncludeEdgeDevices,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a device twin from the IoT Hub using its deviceId.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to retrieve its twin.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The device twin, or <see langword="null"/> if the device doesn't exist.</returns>
    Task<Twin?> GetDeviceTwin(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates a device twin in the IoT Hub, as a merge of the given twin into the current one.
    /// </summary>
    /// <param name="deviceId">The identifier of the device whose twin is updated.</param>
    /// <param name="twin">The device twin with updated data. Its ETag must match the current twin's.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The updated device twin.</returns>
    /// <exception cref="PreconditionFailedException">The twin was changed since it was read (the ETag doesn't match).</exception>
    Task<Twin> UpdateDeviceTwin(
        string deviceId,
        Twin twin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the module identities on an IoT Edge device using its deviceId.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT Edge device.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The module identities (including their keys); empty if the device has no modules.</returns>
    /// <exception cref="DeviceNotFoundException">The device doesn't exist.</exception>
    Task<IReadOnlyCollection<Microsoft.Azure.Devices.Module>> GetModulesOnIotEdgeDevice(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a module identity to a device in the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to which the module is added.</param>
    /// <param name="moduleId">The identifier of the module to add.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The created module identity, including its keys.</returns>
    /// <exception cref="ModuleAlreadyExistsException">The module already exists on the device.</exception>
    Task<Microsoft.Azure.Devices.Module> AddModuleToDevice(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates a module identity in the IoT Hub, e.g. its authentication.
    /// </summary>
    /// <param name="moduleIdentity">The module identity to update, as retrieved from the IoT Hub (including its ETag).</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The updated module identity, including its keys.</returns>
    Task<Microsoft.Azure.Devices.Module> UpdateModule(
        Microsoft.Azure.Devices.Module moduleIdentity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates desired properties on a module twin in the IoT Hub, as a merge: properties that aren't
    /// given keep their value, and a property set to <see langword="null"/> is removed.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT Edge device the module belongs to.</param>
    /// <param name="moduleId">The identifier of the module whose twin is updated.</param>
    /// <param name="twinCollection">The desired properties to merge into the module twin.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The updated module twin.</returns>
    /// <exception cref="ModuleNotFoundException">The device or the module doesn't exist.</exception>
    Task<Twin> UpdateDesiredProperties(
        string deviceId,
        string moduleId,
        TwinCollection twinCollection,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a module twin from the IoT Hub using the deviceId and moduleId.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT Edge device.</param>
    /// <param name="moduleId">The identifier of the module to retrieve its twin.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The module twin, or <see langword="null"/> if the device or the module doesn't exist.</returns>
    Task<Twin?> GetModuleTwin(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously removes a module from a device in the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT Edge device from which to remove the module.</param>
    /// <param name="moduleId">The identifier of the module to remove.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// <see langword="true"/> if the module was removed; <see langword="false"/> if the device or the module doesn't exist
    /// (for example because it was already removed).
    /// </returns>
    Task<bool> RemoveModuleFromDevice(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously attempts to restart specified module on a device in the IoT Hub, through edgeAgent's RestartModule direct method.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT Edge device on which the module is to be restarted.</param>
    /// <param name="moduleId">The identifier of the module to be restarted.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// Whether edgeAgent answered with status 200, its status code, and a JSON payload: a success message, or edgeAgent's answer.
    /// An IoT Hub error, such as an offline device, is thrown (see <see cref="IIoTHubModuleService.CallMethod"/>).
    /// </returns>
    Task<(bool Succeeded, int StatusCode, string JsonPayload)> RestartModuleOnDevice(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Use the UploadModuleLogs direct method to bundle and upload a zip file of IoT Edge module logs to an available Azure Blob Storage container.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT Edge device on which the logs should be fetched from</param>
    /// <param name="request">The request parameters</param>
    /// <param name="requestOptions">IoT Hub API request options, such as retry strategy on transient errors</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// A response containing status code and correlation ID of the started task. An IoT Hub error, such as an offline
    /// device, is returned as the response's status code instead of being thrown.
    /// </returns>
    Task<Response<LogResponse>> UploadModuleLogs(
        string deviceId,
        UploadModuleLogsRequest request,
        IoTHubRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Use the UploadSupportBundle direct method to bundle and upload a zip file of IoT Edge module logs to an available Azure Blob Storage container.
    /// This direct method runs the `iotedge support-bundle command` on your IoT Edge device to obtain the logs.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT Edge device on which the logs should be fetched from</param>
    /// <param name="request">The request parameters</param>
    /// <param name="requestOptions">IoT Hub API request options, such as retry strategy on transient errors</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// A response containing status code and correlation ID of the started task. An IoT Hub error, such as an offline
    /// device, is returned as the response's status code instead of being thrown.
    /// </returns>
    Task<Response<LogResponse>> UploadSupportBundle(
        string deviceId,
        UploadSupportBundleRequest request,
        IoTHubRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Use the GetTaskStatus direct method to check the status of an upload logs request.
    /// The GetTaskStatus request payload uses the correlationId from the upload logs request to get the task status.
    /// The correlationId comes from the response to the UploadModuleLogs or UploadSupportBundle direct method call.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT Edge device on which the task is running on</param>
    /// <param name="request">The request parameters</param>
    /// <param name="requestOptions">IoT Hub API request options, such as retry strategy on transient errors</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// A response containing the state of the task. An IoT Hub error, such as an offline device, is returned as the
    /// response's status code instead of being thrown.
    /// </returns>
    Task<Response<LogResponse>> GetTaskStatus(
        string deviceId,
        GetTaskStatusRequest request,
        IoTHubRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously applies a given configuration content to a specified IoT device, as a single-device deployment.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT device to which the configuration is to be applied.</param>
    /// <param name="manifestContent">The configuration content to be applied. This includes the desired properties and settings for the device, encapsulated within a ConfigurationContent object.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that completes when the configuration content is applied.</returns>
    Task ApplyConfigurationContentOnDevice(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a module identity for each module in the configuration content (except the system modules,
    /// whose names start with '$') to a specified IoT device.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT device to which the modules are to be added.</param>
    /// <param name="manifestContent">The configuration content for the new modules, encapsulated within a ConfigurationContent object.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that completes when the module identities are added.</returns>
    /// <exception cref="ModuleAlreadyExistsException">
    /// A module already exists on the device. The modules before it in the configuration content have been added.
    /// </exception>
    Task AddNewModules(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes a device from the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to delete.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// <see langword="true"/> if the device was deleted; <see langword="false"/> if it doesn't exist
    /// (for example because it was already deleted).
    /// </returns>
    Task<bool> DeleteDevice(
        string deviceId,
        CancellationToken cancellationToken = default);
}