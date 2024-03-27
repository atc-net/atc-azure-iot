namespace Atc.Azure.IotEdge.Client.Wrappers;

public class ModuleClientWrapper : IModuleClientWrapper
{
    private readonly ModuleClient moduleClient;
    private bool disposedValue; // To detect redundant calls

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleClientWrapper"/> class.
    /// </summary>
    /// <param name="moduleClient">The moduleClient</param>
    public ModuleClientWrapper(
        ModuleClient moduleClient)
    {
        this.moduleClient = moduleClient;
    }

    public int DiagnosticSamplingPercentage
    {
        get => this.moduleClient.DiagnosticSamplingPercentage;
        set => this.moduleClient.DiagnosticSamplingPercentage = value;
    }

    public uint OperationTimeoutInMilliseconds
    {
        get => this.moduleClient.OperationTimeoutInMilliseconds;
        set => this.moduleClient.OperationTimeoutInMilliseconds = value;
    }

    public string ProductInfo
    {
        get => this.moduleClient.ProductInfo;
        set => this.moduleClient.ProductInfo = value;
    }

    public Task AbandonAsync(
        string lockToken)
        => moduleClient.AbandonAsync(lockToken);

    public Task AbandonAsync(
        string lockToken,
        CancellationToken cancellationToken)
        => moduleClient.AbandonAsync(lockToken, cancellationToken);

    public Task AbandonAsync(
        Message message)
        => moduleClient.AbandonAsync(message);

    public Task AbandonAsync(
        Message message,
        CancellationToken cancellationToken)
        => moduleClient.AbandonAsync(message, cancellationToken);

    public Task CloseAsync()
        => moduleClient.CloseAsync();

    public Task CloseAsync(
        CancellationToken cancellationToken)
        => moduleClient.CloseAsync(cancellationToken);

    public Task CompleteAsync(
        string lockToken)
        => moduleClient.CompleteAsync(lockToken);

    public Task CompleteAsync(
        string lockToken,
        CancellationToken cancellationToken)
        => moduleClient.CompleteAsync(lockToken, cancellationToken);

    public Task CompleteAsync(
        Message message)
        => moduleClient.CompleteAsync(message);

    public Task CompleteAsync(
        Message message, CancellationToken cancellationToken)
        => moduleClient.CompleteAsync(message, cancellationToken);

    public Task<Twin> GetTwinAsync()
        => moduleClient.GetTwinAsync();

    public Task<Twin> GetTwinAsync(
        CancellationToken cancellationToken)
        => moduleClient.GetTwinAsync(cancellationToken);

    public Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        MethodRequest methodRequest)
        => moduleClient.InvokeMethodAsync(deviceId, methodRequest);

    public Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        MethodRequest methodRequest,
        CancellationToken cancellationToken)
        => moduleClient.InvokeMethodAsync(deviceId, methodRequest, cancellationToken);

    public Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        string moduleId,
        MethodRequest methodRequest)
        => moduleClient.InvokeMethodAsync(deviceId, moduleId, methodRequest);

    public Task<MethodResponse> InvokeMethodAsync(
        string deviceId,
        string moduleId,
        MethodRequest methodRequest,
        CancellationToken cancellationToken)
        => moduleClient.InvokeMethodAsync(deviceId, moduleId, methodRequest, cancellationToken);

    public Task OpenAsync()
        => moduleClient.OpenAsync();

    public Task OpenAsync(
        CancellationToken cancellationToken)
        => moduleClient.OpenAsync(cancellationToken);

    public Task SendEventAsync(
        Message message)
        => moduleClient.SendEventAsync(message);

    public Task SendEventAsync(
        Message message,
        CancellationToken cancellationToken)
        => moduleClient.SendEventAsync(message, cancellationToken);

    public Task SendEventAsync(
        string outputName,
        Message message)
        => moduleClient.SendEventAsync(outputName, message);

    public Task SendEventAsync(
        string outputName,
        Message message,
        CancellationToken cancellationToken)
        => moduleClient.SendEventAsync(outputName, message, cancellationToken);

    public Task SendEventBatchAsync(
        IEnumerable<Message> messages)
        => moduleClient.SendEventBatchAsync(messages);

    public Task SendEventBatchAsync(
        IEnumerable<Message> messages,
        CancellationToken cancellationToken)
        => moduleClient.SendEventBatchAsync(messages, cancellationToken);

    public Task SendEventBatchAsync(
        string outputName,
        IEnumerable<Message> messages)
        => moduleClient.SendEventBatchAsync(outputName, messages);

    public Task SendEventBatchAsync(
        string outputName,
        IEnumerable<Message> messages,
        CancellationToken cancellationToken)
        => moduleClient.SendEventBatchAsync(outputName, messages, cancellationToken);

    public void SetConnectionStatusChangesHandler(
        ConnectionStatusChangesHandler statusChangesHandler)
        => moduleClient.SetConnectionStatusChangesHandler(statusChangesHandler);

    public Task SetDesiredPropertyUpdateCallbackAsync(
        DesiredPropertyUpdateCallback callback,
        object userContext)
        => moduleClient.SetDesiredPropertyUpdateCallbackAsync(callback, userContext);

    public Task SetDesiredPropertyUpdateCallbackAsync(
        DesiredPropertyUpdateCallback callback,
        object userContext,
        CancellationToken cancellationToken)
        => moduleClient.SetDesiredPropertyUpdateCallbackAsync(callback, userContext, cancellationToken);

    public Task SetInputMessageHandlerAsync(
        string inputName,
        MessageHandler messageHandler,
        object userContext)
        => moduleClient.SetInputMessageHandlerAsync(inputName, messageHandler, userContext);

    public Task SetInputMessageHandlerAsync(
        string inputName,
        MessageHandler messageHandler,
        object userContext,
        CancellationToken cancellationToken)
        => moduleClient.SetInputMessageHandlerAsync(inputName, messageHandler, userContext, cancellationToken);

    public Task SetMessageHandlerAsync(
        MessageHandler messageHandler,
        object userContext)
        => moduleClient.SetMessageHandlerAsync(messageHandler, userContext);

    public Task SetMessageHandlerAsync(
        MessageHandler messageHandler,
        object userContext,
        CancellationToken cancellationToken)
        => moduleClient.SetMessageHandlerAsync(messageHandler, userContext, cancellationToken);

    public Task SetMethodDefaultHandlerAsync(
        MethodCallback methodHandler,
        object userContext)
        => moduleClient.SetMethodDefaultHandlerAsync(methodHandler, userContext);

    public Task SetMethodDefaultHandlerAsync(
        MethodCallback methodHandler,
        object userContext,
        CancellationToken cancellationToken)
        => moduleClient.SetMethodDefaultHandlerAsync(methodHandler, userContext, cancellationToken);

    public Task SetMethodHandlerAsync(
        string methodName,
        MethodCallback methodHandler,
        object userContext)
        => moduleClient.SetMethodHandlerAsync(methodName, methodHandler, userContext);

    public Task SetMethodHandlerAsync(
        string methodName,
        MethodCallback methodHandler,
        object userContext,
        CancellationToken cancellationToken)
        => moduleClient.SetMethodHandlerAsync(methodName, methodHandler, userContext, cancellationToken);

    public void SetRetryPolicy(
        IRetryPolicy retryPolicy)
        => moduleClient.SetRetryPolicy(retryPolicy);

    public Task UpdateReportedPropertiesAsync(
        TwinCollection reportedProperties)
        => moduleClient.UpdateReportedPropertiesAsync(reportedProperties);

    public Task UpdateReportedPropertiesAsync(
        TwinCollection reportedProperties,
        CancellationToken cancellationToken)
        => moduleClient.UpdateReportedPropertiesAsync(reportedProperties, cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(
        bool disposing)
    {
        if (disposedValue)
        {
            return;
        }

        if (disposing)
        {
            this.moduleClient.Dispose();
        }

        disposedValue = true;
    }
}