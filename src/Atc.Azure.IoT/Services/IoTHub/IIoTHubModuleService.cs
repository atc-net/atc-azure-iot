namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// Provides services for direct interaction with IoT devices and modules through Azure IoT Hub, enabling the invocation of direct methods on devices.
/// This service facilitates communication between the cloud and IoT devices by sending direct method requests and processing the responses.
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
    /// <param name="requestOptions">IoT Hub API request options, such as retry strategy on transient errors</param>
    /// <param name="cancellationToken">The CancellationToken.</param>
    /// <returns>MethodResultModel, which includes the Status of the Call and the JsonPayload.</returns>
    Task<MethodResultModel> CallMethod(
        string deviceId,
        string moduleId,
        MethodParameterModel parameters,
        IoTHubRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);
}