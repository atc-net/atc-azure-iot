// ReSharper disable GCSuppressFinalizeForTypeWithoutDestructor
namespace Atc.Azure.IoT.Services.IoTHub;

/// <summary>
/// Provides services for direct interaction with IoT devices and modules through Azure IoT Hub, enabling the invocation of direct methods on devices.
/// This service facilitates communication between the cloud and IoT devices by sending direct method requests and processing the responses.
/// </summary>
public sealed partial class IoTHubModuleService : ServiceBase, IIoTHubModuleService, IDisposable
{
    private ServiceClient? serviceClient;

    public IoTHubModuleService(
        IotHubOptions options,
        ILoggerFactory? loggerFactory = null)
    {
        logger = loggerFactory is not null
            ? loggerFactory.CreateLogger<IoTHubModuleService>()
            : NullLogger<IoTHubModuleService>.Instance;

        ValidateAndAssign(options.ConnectionString, Assign);
    }

    public async Task<MethodResultModel> CallMethod(
        string deviceId,
        string moduleId,
        MethodParameterModel parameters,
        IoTHubRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentException.ThrowIfNullOrEmpty(moduleId);
        ArgumentNullException.ThrowIfNull(parameters);

        requestOptions ??= new IoTHubRequestOptions();
        var pipeline = new ResiliencePipelineBuilder<CloudToDeviceMethodResult>()
            .AddIoTHubRequestOptions(
                requestOptions,
                shouldHandle: args => ValueTask.FromResult(args.Outcome.Exception is IotHubException { IsTransient: true }),
                onRetry: args =>
                {
                    LogMethodCallTransientError(
                        ((IotHubException)args.Outcome.Exception!).Code,
                        parameters.Name,
                        deviceId,
                        args.AttemptNumber,
                        requestOptions.MaxRetryAttempts,
                        args.RetryDelay.TotalSeconds);

                    return ValueTask.CompletedTask;
                })
            .Build();

        var methodInfo = new CloudToDeviceMethod(parameters.Name, requestOptions.Timeout);
        methodInfo.SetPayloadJson(parameters.JsonPayload);

        var result = await pipeline.ExecuteAsync(
            async ct => await (string.IsNullOrEmpty(moduleId)
                ? serviceClient!.InvokeDeviceMethodAsync(deviceId, methodInfo, ct)
                : serviceClient!.InvokeDeviceMethodAsync(deviceId, moduleId, methodInfo, ct)),
            cancellationToken);

        return new MethodResultModel(
            Status: result.Status,
            JsonPayload: result.GetPayloadAsJson());
    }

    protected override void Assign(
        string connectionString)
    {
        serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
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