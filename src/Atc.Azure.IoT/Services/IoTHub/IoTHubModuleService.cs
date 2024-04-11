// ReSharper disable GCSuppressFinalizeForTypeWithoutDestructor
namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// The main IoTHubModuleService - Handles call execution.
/// </summary>
public sealed partial class IoTHubModuleService : IotHubServiceBase, IIoTHubModuleService, IDisposable
{
    private ServiceClient? serviceClient;

    public IoTHubModuleService(
        ILogger<IoTHubModuleService> logger,
        IotHubOptions options)
    {
        this.logger = logger;

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