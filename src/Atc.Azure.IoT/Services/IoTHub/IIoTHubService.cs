namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// Represents a service for interacting with Azure IoT Hub, providing functionality to manage devices and modules,
/// including creating, updating, and deleting devices or modules, retrieving device or module twins, applying configurations,
/// and invoking direct methods. It encapsulates operations such as device registry statistics retrieval, device twin
/// management, module twin updates, and edge device configurations, aiming to facilitate comprehensive IoT Hub management.
/// </summary>
public interface IIoTHubService
{
    /// <summary>
    /// Asynchronously retrieves the statistics of the device registry in the IoT Hub.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The registry statistics of the iot hub if the operation is successful; otherwise, null.</returns>
    Task<RegistryStatistics?> GetDeviceRegistryStatistics(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously creates a new device in the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The identifier for the new device to create.</param>
    /// <param name="edgeEnabled">Specifies whether the device is edge enabled.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns><see langword="true" /> if the device was successfully created with the newly created device; otherwise, <see langword="false" /> and null.</returns>
    Task<(bool Succeeded, Device? Device)> CreateDevice(
        string deviceId,
        bool edgeEnabled,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a specific device from the IoT Hub using its deviceId.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to retrieve.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The device if found; otherwise, null.</returns>
    Task<Device?> GetDevice(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a specific device connection-string from the IoT Hub using its deviceId.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to retrieve.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The device connection-string if found; otherwise, null.</returns>
    Task<string?> GetDeviceConnectionString(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves all device twins in the IoT Hub.
    /// </summary>
    /// <param name="onlyIncludeEdgeDevices">Indicates if only edge device twins should be returned.</param>
    /// <returns>A collection of device twins.</returns>
    Task<IReadOnlyCollection<Twin>> GetDeviceTwins(
        bool onlyIncludeEdgeDevices);

    /// <summary>
    /// Asynchronously retrieves a device twin from the IoT Hub using its deviceId.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to retrieve its twin.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The device twin if found; otherwise, null.</returns>
    Task<Twin?> GetDeviceTwin(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates a device twin in the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The deviceId of the device to update its twin.</param>
    /// <param name="twin">The device twin with updated data.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A tuple indicating whether the update was successful and the updated device twin if it was; otherwise, null.</returns>
    Task<(bool Succeeded, Twin? UpdatedTwin)> UpdateDeviceTwin(
        string deviceId,
        Twin twin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves all module twins on an IoT Edge device using its deviceId.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT Edge device to retrieve its module twins.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A collection of module twins if any; otherwise, an empty collection.</returns>
    Task<IEnumerable<Microsoft.Azure.Devices.Module>> GetModulesOnIotEdgeDevice(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates desired properties on a module twin in the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The deviceId of the device to update its twin.</param>
    /// <param name="moduleId">The identifier of the module to retrieve its twin.</param>
    /// <param name="twinCollection">Represents a collection of properties for a Twin</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A tuple indicating whether the update was successful and the updated device twin if it was; otherwise, null.</returns>
    Task<Twin?> UpdateDesiredProperties(
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
    /// <returns>The module twin if found; otherwise, null.</returns>
    Task<Twin?> GetModuleTwin(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously removes a module from a device in the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The identifier of the iot edge device from which to remove the module.</param>
    /// <param name="moduleId">The identifier of the module to remove.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>True if the operation is successful; otherwise, false.</returns>
    Task<bool> RemoveModuleFromDevice(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously attempts to restart specified module on a device in the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The identifier of the iot edge device on which the module is to be restarted.</param>
    /// <param name="moduleId">The identifier of the module to be restarted.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A tuple containing a boolean value that is true if the operation is successful, and false otherwise; an integer representing the status code; and a string message containing a JsonPayload detailing the operation's outcome.</returns>
    Task<(bool Succeeded, int StatusCode, string JsonPayload)> RestartModuleOnDevice(
        string deviceId,
        string moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Use the UploadSupportBundle direct method to bundle and upload a zip file of IoT Edge module logs to an available Azure Blob Storage container.
    /// This direct method runs the `iotedge support-bundle command` on your IoT Edge device to obtain the logs.
    /// </summary>
    /// <param name="deviceId">The identifier of the iot edge device on which the logs should be fetched from</param>
    /// <param name="request">The request parameters</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A response containing status code and correlation ID of the started task</returns>
    Task<Response<LogResponse>> UploadSupportBundle(
        string deviceId,
        UploadSupportBundleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Use the GetTaskStatus direct method to check the status of an upload logs request.
    /// The GetTaskStatus request payload uses the correlationId from the upload logs request to get the task status.
    /// The correlationId comes from the response to the UploadModuleLogs or UploadSupportBundle direct method call.
    /// </summary>
    /// <param name="deviceId">The identifier of the iot edge device on which the task is running on</param>
    /// <param name="request">The request parameters</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A response containing the state of the task</returns>
    Task<Response<LogResponse>> GetTaskStatus(
        string deviceId,
        GetTaskStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously applies a given configuration content to a specified IoT device.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT device to which the configuration is to be applied.</param>
    /// <param name="manifestContent">The configuration content to be applied. This includes the desired properties and settings for the device, encapsulated within a ConfigurationContent object.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A tuple containing a boolean value that is true if the operation is successful, and false otherwise; and a nullable errorMessage with details in case of failure.</returns>
    Task<(bool Succeeded, string? ErrorMessage)> ApplyConfigurationContentOnDevice(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds new modules to a specified IoT device.
    /// </summary>
    /// <param name="deviceId">The identifier of the IoT device to which the modules are to be added.</param>
    /// <param name="manifestContent">The configuration content for the new modules, encapsulated within a ConfigurationContent object.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A tuple containing a boolean value that is true if the operation is successful, and false otherwise; and a nullable errorMessage with details in case of failure.</returns>
    Task<(bool Succeeded, string? ErrorMessage)> AddNewModules(
        string deviceId,
        ConfigurationContent manifestContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes a device from the IoT Hub.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to delete.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>True if the operation is successful; otherwise, false.</returns>
    Task<bool> DeleteDevice(
        string deviceId,
        CancellationToken cancellationToken = default);
}