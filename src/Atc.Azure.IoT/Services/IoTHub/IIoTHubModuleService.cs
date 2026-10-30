namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// Provides services for direct interaction with IoT devices and modules through Azure IoT Hub, enabling the invocation of direct methods on devices.
/// This service facilitates communication between the cloud and IoT devices by sending direct method requests and processing the responses.
/// </summary>
public interface IIoTHubModuleService
{
    /// <summary>
    /// Asynchronously invokes a direct method on a module of an IoT Edge device, and returns the module's answer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The module's answer is returned whatever its status code, also when the module reports an error, for example
    /// 501 for a method it has no handler for.
    /// </para>
    /// <para>
    /// A transient IoT Hub error (<see cref="IotHubException.IsTransient"/>, for example throttling) is retried as
    /// <paramref name="requestOptions"/> says (by default 3 retries, 2 seconds apart). Any other IoT Hub error, or a
    /// transient one that remains after the retries, is thrown, for example <see cref="DeviceNotFoundException"/> with
    /// <see cref="ErrorCode.DeviceNotOnline"/> when the device is offline.
    /// </para>
    /// <para>
    /// A cancelled <paramref name="cancellationToken"/> throws <see cref="OperationCanceledException"/> and is not
    /// retried. The IoT Hub SDK reports a cancelled request as an <see cref="IotHubException"/>, which becomes the
    /// <see cref="Exception.InnerException"/>.
    /// </para>
    /// </remarks>
    /// <param name="deviceId">The device identifier.</param>
    /// <param name="moduleId">The module identifier, for example <c>$edgeAgent</c> for the IoT Edge agent.</param>
    /// <param name="parameters">The method name and its JSON payload.</param>
    /// <param name="requestOptions">
    /// IoT Hub API request options: the retries on transient errors, and how long to wait for the module's answer
    /// (<see cref="IoTHubRequestOptions.Timeout"/>; <see cref="TimeSpan.Zero"/> uses IoT Hub's default of 30 seconds).
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The module's answer: its status code and its JSON payload.</returns>
    Task<MethodResultModel> CallMethod(
        string deviceId,
        string moduleId,
        MethodParameterModel parameters,
        IoTHubRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);
}