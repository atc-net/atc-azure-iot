namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// IoTHubService is a class responsible for managing IoT devices
/// and their interactions with an Azure IoT Hub.
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
    /// Asynchronously retrieves a specific device from the IoT Hub using its deviceId.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to retrieve.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The device if found; otherwise, null.</returns>
    Task<Device?> GetDevice(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves all IoT Edge device twins in the IoT Hub.
    /// </summary>
    /// <returns>A collection of device twins.</returns>
    Task<IReadOnlyCollection<Twin>> GetIoTEdgeDeviceTwins();

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
    Task<(bool IsSuccessful, Twin? UpdatedTwin)> UpdateTwin(
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
        CancellationToken cancellationToken);

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
        CancellationToken cancellationToken);

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
        CancellationToken cancellationToken);

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