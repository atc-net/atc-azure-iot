// ReSharper disable GCSuppressFinalizeForTypeWithoutDestructor
namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// Provides services for direct interaction with IoT devices and modules through Azure IoT Hub, enabling the invocation of direct methods on devices.
/// This service facilitates communication between the cloud and IoT devices by sending direct method requests and processing the responses.
/// </summary>
public sealed partial class IoTHubModuleService : IotHubServiceBase, IIoTHubModuleService, IDisposable
{
    private ServiceClient? serviceClient;

    public IoTHubModuleService(
        ILoggerFactory loggerFactory,
        IotHubOptions options)
    {
        logger = loggerFactory.CreateLogger<IoTHubModuleService>();
        ValidateAndAssign(options.ConnectionString, Assign);
    }

    public async Task<MethodResultModel> CallMethod(
        string deviceId,
        string moduleId,
        MethodParameterModel parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);
        ArgumentNullException.ThrowIfNull(parameters);

        try
        {
            var methodInfo = new CloudToDeviceMethod(parameters.Name);
            methodInfo.SetPayloadJson(parameters.JsonPayload);
            var result = await (string.IsNullOrEmpty(moduleId)
                ? serviceClient!.InvokeDeviceMethodAsync(deviceId, methodInfo, cancellationToken)
                : serviceClient!.InvokeDeviceMethodAsync(deviceId, moduleId, methodInfo, cancellationToken));

            return new MethodResultModel(
                Status: result.Status,
                JsonPayload: result.GetPayloadAsJson());
        }
        catch (DeviceNotFoundException ex)
        {
            LogMethodCallFailedDeviceNotFound(
                deviceId,
                parameters.Name,
                parameters.JsonPayload,
                ex.GetLastInnerMessage());

            return new MethodResultModel(
                Status: StatusCodes.Status404NotFound,
                JsonPayload: $"{{\"message\":\"Device not found by id '{deviceId}'\"}}");
        }
        catch (Exception ex)
        {
            LogMethodCallFailed(
                deviceId,
                parameters.Name,
                parameters.JsonPayload,
                ex.GetLastInnerMessage());

            return new MethodResultModel(
                Status: StatusCodes.Status500InternalServerError,
                JsonPayload: JsonSerializer.Serialize(new MethodResultErrorModel(
                    Status: StatusCodes.Status500InternalServerError,
                    Title: nameof(HttpStatusCode.InternalServerError),
                    Detail: ex.GetLastInnerMessage())));
        }
    }

    protected override void Assign(
        string iotHubConnectionString)
    {
        serviceClient = ServiceClient.CreateFromConnectionString(iotHubConnectionString);
    }

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

        if (serviceClient is null)
        {
            return;
        }

        serviceClient.Dispose();
        serviceClient = null;
    }
}