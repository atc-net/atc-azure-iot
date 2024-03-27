namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// The IIoTHubModuleService interface provides a contract for services that
/// execute calls to methods on IoT Hub Modules. These methods enable
/// interaction and control of IoT devices in an Azure IoT Hub.
/// </summary>
public interface IIoTHubModuleService
{
    /// <summary>
    /// Asynchronously invokes a method on a specific IoT Hub module. The method to be called
    /// is specified in the parameters. This operation can be cancelled through the
    /// CancellationToken parameter. The result of the method call is wrapped in a
    /// MethodResultModel, which includes the status of the call and a JSON payload.
    /// </summary>
    /// <param name="deviceId">The device identifier.</param>
    /// <param name="moduleId">The module identifier.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cancellationToken">The CancellationToken.</param>
    /// <returns>MethodResultModel, which includes the Status of the Call and the JsonPayload.</returns>
    Task<MethodResultModel> CallMethod(
        string deviceId,
        string moduleId,
        MethodParameterModel parameters,
        CancellationToken cancellationToken = default);
}